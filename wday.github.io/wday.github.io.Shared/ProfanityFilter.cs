namespace WeddingInvitation.Shared;

/// <summary>
/// Bộ lọc từ ngữ thô tục cho lời chúc / lời nhắn.
/// Dùng chung: Client kiểm tra trước khi gửi (báo lỗi lịch sự),
/// Server kiểm tra lại trước khi lưu (chặn cả request gửi thẳng API).
/// So khớp theo TỪ (tách theo ký tự không phải chữ) để tránh chặn nhầm
/// các từ lành chứa chuỗi giống (vd "vãi" trong "vải").
/// </summary>
public static class ProfanityFilter
{
    // Từ đơn bị chặn (đã lowercase). Muốn thêm/bớt chỉ cần sửa danh sách này.
    private static readonly HashSet<string> BadWords = new(StringComparer.OrdinalIgnoreCase)
    {
        // Tiếng Việt (không thêm các chuỗi không dấu dễ trùng từ lành như "buoi", "lon", "cac")
        "đụ", "địt", "đéo", "cặc", "lồn", "buồi", "đĩ", "điếm",
        "vcl", "vkl", "clgt", "cmm", "ccmm", "dcm", "đcm", "dmm", "đmm",
        "vãi", "cứt", "đách", "đjt", "dit",
        // Tiếng Anh
        "fuck", "fucking", "shit", "bitch", "asshole", "dick", "pussy", "cunt",
    };

    // Cụm từ bị chặn (so khớp chuỗi con sau khi lowercase)
    private static readonly string[] BadPhrases =
    [
        "óc chó", "chó chết", "đồ chó", "thằng chó", "con chó", "súc vật",
        "mẹ mày", "bố mày", "má mày", "con mẹ", "đồ ngu", "ngu như",
    ];

    public static bool ContainsProfanity(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        var lowered = text.ToLowerInvariant();

        foreach (var phrase in BadPhrases)
        {
            if (lowered.Contains(phrase)) return true;
        }

        foreach (var word in Tokenize(lowered))
        {
            if (BadWords.Contains(word)) return true;
        }

        return false;
    }

    private static IEnumerable<string> Tokenize(string text)
    {
        var start = -1;
        for (var i = 0; i <= text.Length; i++)
        {
            var isLetter = i < text.Length && char.IsLetterOrDigit(text[i]);
            if (isLetter && start < 0) start = i;
            else if (!isLetter && start >= 0)
            {
                yield return text[start..i];
                start = -1;
            }
        }
    }
}
