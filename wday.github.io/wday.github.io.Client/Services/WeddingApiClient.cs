using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using WeddingInvitation.Shared;

namespace WeddingInvitation.Client.Services;

/// <summary>
/// Gửi RSVP / lời chúc lên API của Server khi có backend (chạy local hoặc VPS).
/// Khi web được host static trên GitHub Pages (không có API), tự động
/// fallback lưu vào localStorage của trình duyệt — trang vẫn hoạt động đầy đủ.
/// </summary>
public class WeddingApiClient(HttpClient http, IJSRuntime js)
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    private bool? _apiAvailable;

    private async Task<bool> ApiAvailableAsync()
    {
        if (_apiAvailable is { } known) return known;
        try
        {
            using var response = await http.GetAsync($"api/weddings/{Data.WeddingContent.Slug}");
            _apiAvailable = response.IsSuccessStatusCode;
        }
        catch
        {
            _apiAvailable = false;
        }
        return _apiAvailable.Value;
    }

    public async Task<bool> SendRsvpAsync(string slug, RsvpRequest request)
    {
        if (await ApiAvailableAsync())
        {
            var response = await http.PostAsJsonAsync($"api/weddings/{slug}/rsvp", request);
            return response.IsSuccessStatusCode;
        }

        await AppendToLocalListAsync($"wedding-rsvp-{slug}", request);
        return true;
    }

    public async Task<List<GuestWishDto>> GetWishesAsync(string slug)
    {
        if (await ApiAvailableAsync())
        {
            try
            {
                return await http.GetFromJsonAsync<List<GuestWishDto>>($"api/weddings/{slug}/wishes") ?? [];
            }
            catch { return []; }
        }

        return await ReadLocalListAsync<GuestWishDto>($"wedding-wishes-{slug}");
    }

    public async Task<bool> SendWishAsync(string slug, GuestWishDto wish)
    {
        wish.CreatedAt = DateTime.UtcNow;

        if (await ApiAvailableAsync())
        {
            var response = await http.PostAsJsonAsync($"api/weddings/{slug}/wishes", wish);
            return response.IsSuccessStatusCode;
        }

        await AppendToLocalListAsync($"wedding-wishes-{slug}", wish);
        return true;
    }

    // ---- localStorage helpers ------------------------------------------

    private async Task<List<T>> ReadLocalListAsync<T>(string key)
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", key);
        if (string.IsNullOrEmpty(json)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<T>>(json, JsonOpts) ?? [];
        }
        catch { return []; }
    }

    private async Task AppendToLocalListAsync<T>(string key, T item)
    {
        var list = await ReadLocalListAsync<T>(key);
        list.Add(item);
        await js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(list, JsonOpts));
    }
}
