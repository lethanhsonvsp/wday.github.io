using System.ComponentModel.DataAnnotations;

namespace WeddingInvitation.Shared;

/// <summary>Dữ liệu cơ bản của một thiệp cưới (trả về từ GET /api/weddings/{slug}).</summary>
public record WeddingDto(
    string Slug,
    string GroomName,
    string BrideName,
    DateTime WeddingDate);

/// <summary>Xác nhận tham dự — gửi lên POST /api/weddings/{slug}/rsvp.</summary>
public class RsvpRequest
{
    [Required, StringLength(100)]
    public string GuestName { get; set; } = "";

    public bool IsAttending { get; set; } = true;

    [Range(1, 20)]
    public int GuestCount { get; set; } = 1;

    [StringLength(500)]
    public string? Message { get; set; }
}

/// <summary>Lời chúc của khách — GET/POST /api/weddings/{slug}/wishes.</summary>
public class GuestWishDto
{
    [Required, StringLength(100)]
    public string GuestName { get; set; } = "";

    [Required, StringLength(500)]
    public string Content { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
