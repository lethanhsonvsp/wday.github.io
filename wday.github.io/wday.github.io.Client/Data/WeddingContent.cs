namespace WeddingInvitation.Client.Data;

/// <summary>
/// Toàn bộ nội dung chữ của thiệp cưới đặt tập trung tại đây —
/// muốn đổi tên, ngày, địa điểm, câu quote... chỉ cần sửa file này.
/// </summary>
public static class WeddingContent
{
    public const string Slug = "ngoc-nam-lan-anh";

    public const string GroomName = "Ngọc Nam";
    public const string BrideName = "Lan Anh";
    public const string CoupleNames = "Ngọc Nam & Lan Anh";

    /// <summary>Ngày giờ lễ thành hôn — dùng cho countdown.</summary>
    public static readonly DateTime WeddingDate = new(2026, 10, 18, 10, 30, 0);

    public const string DateDisplay = "18 . 10 . 2026";
    public const string DateDisplayEn = "18 OCTOBER 2026";

    public const string IntroQuote =
        "Có những cuộc gặp gỡ rất tình cờ, nhưng lại trở thành câu chuyện của cả một đời.";

    public const string CinematicQuote =
        "Whatever our souls are made of, his and mine are the same.";

    public const string GroomDescription =
        "Một người trầm lặng, ấm áp — luôn tin rằng hạnh phúc là được về nhà đúng giờ ăn tối cùng người mình thương.";

    public const string BrideDescription =
        "Một cô gái yêu hoa, yêu nắng sớm — và tin rằng mọi điều đẹp nhất đều bắt đầu từ một nụ cười.";

    public const string RsvpQuote =
        "Sự hiện diện của bạn là niềm vui lớn nhất trong ngày đặc biệt của chúng tôi.";

    public const string FinalQuote =
        "Cảm ơn bạn đã trở thành một phần trong câu chuyện của chúng tôi.";

    // ---- Sự kiện cưới -------------------------------------------------

    public record WeddingEvent(
        string Title, string Date, string Time,
        string Venue, string Address, string MapUrl, string Image);

    public static readonly WeddingEvent[] Events =
    [
        new("LỄ VU QUY", "18 . 10 . 2026", "08:00",
            "Tư gia nhà gái",
            "Thôn Đông Thịnh, Xã Hoằng Lộc, Tỉnh Thanh Hóa", // TODO: thay địa chỉ nhà gái thật
            "https://maps.google.com/?q=Hoang+Loc+Thanh+Hoa", // TODO: thay link Google Maps thật
            WeddingImages.EventVuQuy),

        new("LỄ THÀNH HÔN", "18 . 10 . 2026", "10:30",
            "Tư gia nhà trai",
            "Thôn Xuân Phú, Xã Xuân Hòa, Tỉnh Thanh Hóa", // TODO: thay địa chỉ nhà trai thật
            "https://maps.google.com/?q=Thanh+Hoa",       // TODO: thay link Google Maps thật
            WeddingImages.EventCeremony),

        new("TIỆC CƯỚI", "18 . 10 . 2026", "17:30",
            "Tổ chức tại tư gia nhà trai",
            "Thôn Xuân Phú, Xã Xuân Hòa, Tỉnh Thanh Hóa", // TODO: cùng địa chỉ nhà trai
            "https://maps.google.com/?q=Thanh+Hoa",       // TODO: thay link Google Maps thật
            WeddingImages.EventParty),
    ];

    // ---- Chuyện tình yêu ----------------------------------------------

    public record StoryMilestone(string Year, string Title, string Text, string Image);

    public static readonly StoryMilestone[] LoveStory =
    [
        new("2019", "Lần đầu gặp nhau",
            "Một buổi chiều rất bình thường, giữa những người xa lạ — chúng tôi nhìn thấy nhau, và thế giới bỗng chậm lại một nhịp.",
            WeddingImages.Story2019),

        new("2021", "Chúng ta bắt đầu",
            "Từ những tin nhắn dài đến những buổi hẹn ngắn, từ \"cậu – tớ\" thành \"anh – em\". Chúng tôi chính thức bước vào câu chuyện của riêng mình.",
            WeddingImages.Story2021),

        new("2025", "Lời cầu hôn",
            "Dưới ánh đèn ấm và trước những người thân yêu nhất, anh quỳ xuống — và em đã nói \"Em đồng ý\".",
            WeddingImages.Story2025),

        new("2026", "Về chung một nhà",
            "Ngày 18 tháng 10 năm 2026 — chúng tôi chọn nhau, một lần và mãi mãi.",
            WeddingImages.Story2026),
    ];

    // ---- Lời chúc mở màn -----------------------------------------------
    // Hiển thị trong dải lời chúc trôi (danmaku) để section luôn sống động,
    // kể cả khi chưa có khách gửi. Lời chúc thật của khách luôn hiện trước.

    public record StarterWish(string Name, string Content);

    public static readonly StarterWish[] StarterWishes =
    [
        new("Gia đình hai bên", "Chúc hai con trăm năm hạnh phúc, sớm có tin vui! 💕"),
        new("Hội bạn thân", "Cuối cùng cũng cưới! Chúc hai bạn mãi ngọt ngào như ngày đầu 🥂"),
        new("Minh Anh", "Chúc anh chị về chung một nhà luôn ngập tiếng cười."),
        new("Quang Huy", "Đầu bạc răng long, con đàn cháu đống nhé hai bạn!"),
        new("Thu Trang", "Chúc mừng đám cưới! Hạnh phúc viên mãn nha 💐"),
        new("Đồng nghiệp chú rể", "Chúc sếp và chị nhà mãi yêu thương, sự nghiệp thăng hoa!"),
        new("Ngọc Hà", "Yêu thương đong đầy, hạnh phúc bền lâu nhé!"),
        new("Bạn học cô dâu", "Cô dâu xinh nhất hôm nay! Chúc hai bạn hạnh phúc mãi mãi ✨"),
        new("Văn Nam", "Chúc hai bạn một đời bình an, một nhà ấm áp."),
        new("Cô chú Út", "Mong hai con luôn nắm chặt tay nhau qua mọi buồn vui."),
    ];

    // ---- Mừng cưới (QR ngân hàng) — TODO: thay bằng thông tin thật ----

    public record BankAccount(string Owner, string Role, string Bank, string AccountNumber);

    public static readonly BankAccount GroomBank =
        new(GroomName, "CHÚ RỂ", "Vietcombank", "0123456789");

    public static readonly BankAccount BrideBank =
        new(BrideName, "CÔ DÂU", "Techcombank", "9876543210");
}
