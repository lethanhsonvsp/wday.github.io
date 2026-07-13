namespace WeddingInvitation.Client.Data;

/// <summary>
/// Toàn bộ đường dẫn ảnh của thiệp cưới đặt tập trung tại đây.
/// Ảnh nằm ở wwwroot/images/gallery — muốn thay ảnh chỉ cần chép đè file
/// cùng tên, hoặc sửa đường dẫn ở đây. KHÔNG hard-code URL ảnh trong component.
/// Đường dẫn tương đối (không có "/" đầu) để chạy được cả trên GitHub Pages.
/// </summary>
public static class WeddingImages
{
    private const string Dir = "images/gallery";

    // ---- Ảnh theo vai trò từng section --------------------------------
    public const string OpeningBackground = $"{Dir}/wedding-05.jpg";  // 1440x900 — nét nhất, làm màn mở thiệp
    public const string Hero              = $"{Dir}/wedding-09.jpg";  // 1265x1933 — dọc, chất lượng cao
    public const string Groom             = $"{Dir}/wedding-02.webp"; // chú rể nổi bật
    public const string Bride             = $"{Dir}/wedding-10.webp"; // cô dâu nổi bật
    public const string Story2019         = $"{Dir}/wedding-01.webp";
    public const string Story2021         = $"{Dir}/wedding-12.jpg";
    public const string Story2025         = $"{Dir}/wedding-11.jpg";  // cảnh quỳ gối — lời cầu hôn
    public const string Story2026         = $"{Dir}/wedding-03.jpg";  // hôn lễ trong nhà thờ
    public const string EventCeremony     = $"{Dir}/wedding-06.jpg";
    public const string EventParty        = $"{Dir}/wedding-04.jpg";
    public const string Cinematic         = $"{Dir}/wedding-08.webp"; // dọc, tông trầm — nền quote
    public const string Final             = $"{Dir}/wedding-07.jpg";

    // ---- Gallery -------------------------------------------------------
    // Shape: hình dạng thật của ảnh (portrait/landscape/square) để layout
    // giữ đúng tỷ lệ. Size: vai trò trong nhịp điệu grid (feature = ảnh
    // highlight chiếm 2 cột). Thứ tự dưới đây đã sắp có chủ đích:
    // lớn–nhỏ–dọc–ngang xen kẽ, không để 2 ảnh cùng tỷ lệ đứng cạnh nhau.
    public record GalleryImage(string Src, string Alt, string Shape, bool Feature = false);

    public static readonly GalleryImage[] Gallery =
    [
        new($"{Dir}/wedding-09.jpg",  "Cô dâu trong váy cưới trắng",      "portrait", Feature: true),
        new($"{Dir}/wedding-05.jpg",  "Khoảnh khắc mùa hoa",              "landscape"),
        new($"{Dir}/wedding-01.webp", "Cô dâu chú rể rời lễ đường",       "square"),
        new($"{Dir}/wedding-08.webp", "Trong thánh đường",                "tall"),
        new($"{Dir}/wedding-06.jpg",  "Cô dâu bên khung cửa sáng",        "landscape"),
        new($"{Dir}/wedding-02.webp", "Chú rể và cô dâu",                 "portrait"),
        new($"{Dir}/wedding-03.jpg",  "Nụ hôn trong hôn lễ",              "landscape", Feature: true),
        new($"{Dir}/wedding-10.webp", "Trao nhau trái tim",               "portrait"),
        new($"{Dir}/wedding-12.jpg",  "Vòng tay hạnh phúc",               "landscape"),
        new($"{Dir}/wedding-11.jpg",  "Lời cầu hôn",                      "portrait"),
        new($"{Dir}/wedding-04.jpg",  "Ngày chung đôi",                   "landscape"),
        new($"{Dir}/wedding-07.jpg",  "Nắm tay nhau đi khắp thế gian",    "portrait"),
    ];
}
