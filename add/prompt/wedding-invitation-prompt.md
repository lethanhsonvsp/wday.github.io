# Prompt: Xây dựng Website Thiệp Cưới Online — Ngọc Nam & Lan Anh

Bạn là một Senior Blazor Developer và UI/UX Designer chuyên xây dựng website cưới hiện đại.

Hãy xây dựng một **website thiệp cưới online hoàn chỉnh bằng .NET 10 và Blazor WebAssembly** cho cặp đôi **Ngọc Nam & Lan Anh**.

Mục tiêu quan trọng nhất của dự án:

**GIAO DIỆN ĐẸP, LÃNG MẠN, SANG TRỌNG, ANIMATION MƯỢT VÀ TỐI ƯU TRÊN ĐIỆN THOẠI, ĐẶC BIỆT CHÚ TRỌNG VIỆC SẮP XẾP ẢNH (IMAGE LAYOUT) MỘT CÁCH NGHỆ THUẬT VÀ HẤP DẪN.**

Không tập trung xây dựng kiến trúc hệ thống phức tạp. Không dùng Clean Architecture, CQRS, MediatR hoặc DDD. Giữ project đơn giản, dễ đọc, dễ chỉnh sửa.

---

## Thứ tự thực hiện (BẮT BUỘC — làm theo từng phase, không gộp)

Vì khối lượng UI rất lớn (14 section + animation + API), hãy triển khai theo đúng thứ tự sau, hoàn thiện phase trước rồi mới sang phase sau:

1. **Phase 1 — Cấu trúc solution**: tạo 3 project, cấu hình font, palette màu, CSS reset, animation helper (JS + CSS), cấu trúc thư mục ảnh/nhạc placeholder.
2. **Phase 2 — Opening + Hero + Introduction** (section 1–3)
3. **Phase 3 — Couple + Wedding Date + Countdown + Events** (section 4–7)
4. **Phase 4 — Love Story + Gallery + Lightbox** (section 8–9, phần nặng nhất, đầu tư kỹ)
5. **Phase 5 — Cinematic Quote + RSVP + Guest Wishes + Wedding Gift + Final** (section 10–14)
6. **Phase 6 — API + SQLite + kết nối Client-Server**

Không tự ý gộp nhiều phase vào một lần trả lời nếu điều đó khiến output bị cắt ngắn hoặc hời hợt — đặc biệt là Gallery (section 9), đây là phần quan trọng nhất về mặt hình ảnh.

---

## Công nghệ & cấu trúc solution

* .NET 10
* Blazor WebAssembly **Hosted** (mô hình 3 project riêng biệt như dưới đây — không chuyển sang mô hình "Blazor Web App / Auto render mode" một-project của .NET 8+, trừ khi không thể dựng được hosted model trên .NET 10, trong trường hợp đó hãy nói rõ lý do trước khi đổi hướng)
* ASP.NET Core Server (Minimal API là đủ, không cần Controller)
* Entity Framework Core + SQLite
* CSS thuần (xóa toàn bộ Bootstrap CSS mặc định của template)
* JavaScript tối thiểu (chỉ dùng cho: IntersectionObserver, audio control, lightbox swipe, copy-to-clipboard)

```
WeddingInvitation.Client   -> toàn bộ giao diện thiệp cưới
WeddingInvitation.Server   -> API + SQLite
WeddingInvitation.Shared   -> DTO dùng chung
```

Server chỉ xử lý: lấy dữ liệu thiệp cưới, lưu RSVP, lưu lời chúc.
**Không** Admin Dashboard, **không** Authentication/Authorization, **không** Repository Pattern nếu không cần — dùng `WeddingDbContext` trực tiếp trong endpoint hoặc service đơn giản.

---

## Tài nguyên placeholder (ảnh & nhạc)

Vì chưa có ảnh/nhạc thật, hãy dùng tài nguyên tạm sau, đặt đúng cấu trúc để sau này thay bằng ảnh thật dễ dàng, không phải sửa code:

* Ảnh: dùng `https://picsum.photos/seed/{tên-riêng}/{width}/{height}` với seed đặt tên gợi nhớ (vd `seed/couple-portrait-1/800/1000`) để ảnh cố định qua các lần load, không random mỗi lần refresh.
* Toàn bộ URL ảnh đặt tập trung trong một file cấu hình duy nhất (vd `wwwroot/data/gallery-images.json` hoặc một `static class WeddingImages`), không hard-code rải rác trong từng component — để sau này thay ảnh thật chỉ cần sửa 1 chỗ.
* Nhạc nền: dùng 1 file mp3 free-license bất kỳ (khai báo rõ nguồn trong comment), đặt tại `wwwroot/audio/wedding-song.mp3`. Nếu không thể nhúng file thật, tạo sẵn thẻ `<audio>` trỏ tới path này kèm ghi chú "thay file thật vào đây".
* QR code ngân hàng (section Wedding Gift): generate QR placeholder bằng thư viện JS nhẹ (vd `qrcode.js` qua CDN) từ chuỗi text giả (tên NH — số TK — tên chủ TK), không cần ảnh QR thật.

---

## Mục tiêu giao diện

Cảm giác **wedding landing page cao cấp**, tuyệt đối không giống trang quản trị hay Blazor mặc định.

Phong cách: Romantic / Editorial Wedding / Luxury / Elegant / Minimal.

**Bảng màu:**
* Ivory `#FFFFF0`
* Warm White `#FAF6F0`
* Cream `#F5EDE1`
* Champagne `#F1E4C3`
* Soft Beige `#E8DCC8`
* Dusty Rose `#C9A0A0`

Không dùng màu xanh mặc định Bootstrap/Blazor. Không dùng card bo góc hàng loạt, không nhốt mọi section trong box. Thiết kế nhiều khoảng trắng.

**Ảnh cưới là thành phần thị giác quan trọng nhất**, phải sắp xếp có chủ đích, tạo nhịp điệu thị giác xuyên suốt trang.

**Typography — chỉ định cụ thể (Google Fonts):**
* Tên cô dâu chú rể / các con số lớn (countdown, ngày 18): **Great Vibes** (script, cho phần mang tính chữ ký/lãng mạn) HOẶC **Playfair Display** weight 700–900 (cho phần editorial, số to). Dùng Playfair Display cho hero title và số ngày; dùng Great Vibes riêng cho chữ ký "Ngọc Nam & Lan Anh" ở Opening/Final section.
* Heading section, quote: **Cormorant Garamond** italic, size lớn.
* Body text, label nhỏ (vd "SAVE THE DATE", "WE ARE GETTING MARRIED"): **Montserrat**, letter-spacing rộng, uppercase, size nhỏ.

Icon (nav lightbox, music toggle, map pin...): dùng **inline SVG tự vẽ** (line-art mảnh, không dùng icon font/thư viện ngoài) để đồng bộ phong cách editorial và tránh phụ thuộc CDN.

---

## Trang thiệp cưới

Route: `/wedding/{slug}` — ví dụ `/wedding/ngoc-nam-lan-anh`

Thiết kế **single-page cinematic scrolling experience**. Các section nối tiếp tự nhiên. Không menu navigation kiểu doanh nghiệp.

### 1. Opening Screen
Màn hình mở thiệp toàn màn hình. Background ảnh cưới, overlay tối nhẹ. Giữa màn hình:
```
SAVE THE DATE
Ngọc Nam & Lan Anh
18 . 10 . 2026
[ MỞ THIỆP ]
```
Nút thanh lịch, tối giản. Khi nhấn: fade out opening → nội dung thiệp xuất hiện → nhạc bắt đầu phát → scroll về đầu trang. Animation mượt, không reload trang.

### 2. Hero Section
Toàn màn hình, ảnh cưới `object-fit: cover`, chủ thể luôn nằm trong safe area (đặc biệt mobile). Overlay gradient nhẹ.
```
THE WEDDING OF
NGỌC NAM & LAN ANH
18 OCTOBER 2026
```
Tên rất lớn, dùng `clamp()` để scale trên mobile. Fade + translate nhẹ khi xuất hiện. Scroll indicator tối giản ở cuối màn hình.

### 3. Wedding Introduction
Nền ivory. Label nhỏ "WE ARE GETTING MARRIED", sau đó câu quote (serif lớn, layout thoáng, không card, có decorative line/floral SVG nhẹ):
> "Có những cuộc gặp gỡ rất tình cờ, nhưng lại trở thành câu chuyện của cả một đời."

### 4. Couple Section
Không dùng card Bootstrap. Phong cách wedding magazine.
* Desktop: hai ảnh chân dung offset lệch nhau, tên/nội dung xen kẽ.
* Mobile: ảnh chiếm gần toàn chiều ngang, tên lớn phía dưới.

```
THE GROOM — Ngọc Nam — [mô tả ngắn]
THE BRIDE — Lan Anh — [mô tả ngắn]
```
Ảnh tỷ lệ portrait, khuôn mặt căn giữa khung để tránh mất chủ thể khi responsive. Hiệu ứng reveal khi scroll.

### 5. Wedding Date
Typography lớn kiểu tạp chí thời trang, không dùng calendar component:
```
OCTOBER
18   (cực lớn, chiếm ~40–50% chiều rộng desktop)
2026
SUNDAY
SAVE THE DATE
```

### 6. Countdown
```
DAYS   HOURS   MINUTES   SECONDS
```
Số lớn, không đặt trong card, phân chia bằng line mảnh. Chạy realtime ở Client, animation số đổi nhẹ.

### 7. Wedding Events
```
LỄ THÀNH HÔN — 18/10/2026 — 10:30 AM — Tư gia nhà trai — [Địa chỉ] — [XEM BẢN ĐỒ]
TIỆC CƯỚI — 18/10/2026 — 17:30 PM — [Tên nhà hàng] — [Địa chỉ] — [XEM BẢN ĐỒ]
```
Layout editorial, ảnh venue/ảnh cưới xen kẽ giữa các block để tạo nhịp thị giác. Nút bản đồ dạng text với underline animation.

### 8. Love Story
Timeline, không dùng timeline component mặc định:
```
2019 — Lần đầu gặp nhau
2021 — Chúng ta bắt đầu
2025 — Lời cầu hôn
2026 — Về chung một nhà
```
Desktop: ảnh + nội dung xen kẽ trái phải. Mobile: vertical storytelling. Ảnh lệch nhẹ (asymmetrical) tạo cảm giác scrapbook/editorial.

### 9. Wedding Gallery (QUAN TRỌNG NHẤT)
Editorial masonry, nhiều tỷ lệ ảnh (portrait/landscape/square), không ép cùng kích thước.
* Desktop: CSS Grid.
* Mobile: 2-column masonry hoặc layout xen kẽ.

Yêu cầu:
* Sắp theo nhịp điệu (rhythm), không random.
* Xen kẽ lớn–nhỏ–dọc–ngang tạo flow tự nhiên.
* Tránh 2 ảnh cùng tỷ lệ cạnh nhau.
* Ảnh highlight lớn hơn, đặt vị trí dễ nhìn.
* Không để khoảng trắng bất thường.

Click ảnh → fullscreen lightbox, nền đen, ảnh giữa màn hình, nút Previous/Next (SVG), swipe trên mobile, lazy loading. Hover desktop zoom nhẹ (không quá mạnh).

### 10. Cinematic Quote Section
Full-screen ảnh cưới, background fixed (nếu thiết bị hỗ trợ), overlay tối. Giữa màn hình, text trắng lớn:
> "Whatever our souls are made of, his and mine are the same."

Tạo khoảng nghỉ thị giác giữa Gallery và RSVP.

### 11. RSVP
```
WILL YOU JOIN US?
"Sự hiện diện của bạn là niềm vui lớn nhất trong ngày đặc biệt của chúng tôi."

Fields: Tên của bạn / Bạn sẽ tham dự? (Tôi sẽ tham dự | Rất tiếc, tôi không thể) / Số người tham dự / Lời nhắn
Button: GỬI XÁC NHẬN
```
Input chỉ có border-bottom, background transparent, không dùng input Bootstrap. Gửi qua API → lưu SQLite → animation success nhẹ (không dùng `alert()` JS).

### 12. Guest Wishes
```
WORDS FROM OUR FAVORITE PEOPLE
```
Hiển thị lời chúc khách kiểu trích dẫn tạp chí (tên nhỏ, nội dung serif italic). Form: Tên / Lời chúc / GỬI LỜI CHÚC → lưu SQLite.

### 13. Wedding Gift
```
GỬI QUÀ MỪNG CƯỚI
[CHÚ RỂ]  [CÔ DÂU]
```
Chọn → mở modal (tự viết, không Bootstrap Modal) hiển thị: QR code placeholder, tên ngân hàng, tên tài khoản, số tài khoản, nút copy số tài khoản.

### 14. Final Section
Full-screen ảnh cưới:
```
THANK YOU
Ngọc Nam & Lan Anh
18 . 10 . 2026
"Cảm ơn bạn đã trở thành một phần trong câu chuyện của chúng tôi."
```
Fade-in, kết thúc nhẹ nhàng.

---

## Animation

CSS: `opacity`, `transform`, `transition`. Dùng `IntersectionObserver` (JS) để thêm class `.is-visible`.

Các animation: `fade-up`, `fade-in`, `reveal-left`, `reveal-right`, `image-reveal`. Duration 600–1200ms. Không lạm dụng, không làm text bay nhảy, không gây lag mobile. **Bắt buộc hỗ trợ `prefers-reduced-motion`.**

---

## Music

Nhạc nền (xem mục Tài nguyên placeholder). Floating music button góc màn hình. Sau khi nhấn MỞ THIỆP → phát nhạc, có Play/Pause. Khi phát: icon xoay rất chậm. Player nhỏ gọn, không chiếm diện tích.

---

## Responsive Mobile

Ưu tiên iPhone/Android/iPad, đặc biệt 360–430px. Không horizontal scroll. Typography dùng `clamp()`. Ảnh crop thông minh giữ chủ thể trên mobile. Spacing section phù hợp mobile. Vùng chạm nút tối thiểu 44px.

Test tại: 375px, 390px, 430px, 768px, 1440px.

---

## Backend

SQLite, model: `Wedding`, `Rsvp`, `GuestWish`. `Wedding` chứa dữ liệu cơ bản của thiệp.

API:
```
GET  /api/weddings/{slug}
POST /api/weddings/{slug}/rsvp
GET  /api/weddings/{slug}/wishes
POST /api/weddings/{slug}/wishes
```

Seed sẵn wedding slug: `ngoc-nam-lan-anh`. Không Repository Pattern nếu không cần. Ưu tiên code dễ hiểu.

---

## Yêu cầu code

Code thực tế, không pseudo-code. Tập trung phần lớn thời gian vào: Razor Component, CSS, Responsive, Typography, Image layout (đặc biệt quan trọng), Animation, UX.

Tách component:
```
OpeningInvitation.razor
WeddingHero.razor
WeddingIntroduction.razor
CoupleSection.razor
WeddingDateSection.razor
WeddingCountdown.razor
WeddingEvents.razor
LoveStory.razor
WeddingGallery.razor
CinematicQuote.razor
RsvpSection.razor
GuestWishes.razor
WeddingGift.razor
WeddingMusicPlayer.razor
WeddingFinal.razor
```

Không tạo giao diện Blazor mặc định, không NavMenu, không sidebar, không layout dashboard. Xóa toàn bộ Bootstrap styling mặc định nếu không cần.

Kết quả cuối: website phải như một **trang wedding editorial cao cấp thiết kế riêng**, không phải app CRUD trang trí lại.

**Bắt đầu từ Phase 1** (cấu trúc solution + font + palette + placeholder resources), sau đó lần lượt các phase tiếp theo theo đúng thứ tự đã nêu.
