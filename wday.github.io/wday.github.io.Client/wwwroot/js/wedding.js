// JavaScript tối thiểu cho thiệp cưới:
// IntersectionObserver (scroll reveal), audio control, lightbox swipe/keyboard,
// copy-to-clipboard, QR code. Mọi thứ khác làm bằng Blazor + CSS.
window.wedding = (function () {
    "use strict";

    // ---- Scroll reveal -------------------------------------------------
    let observer = null;

    function ensureObserver() {
        if (observer) return observer;
        observer = new IntersectionObserver(
            (entries) => {
                for (const entry of entries) {
                    if (entry.isIntersecting) {
                        entry.target.classList.add("is-visible");
                        observer.unobserve(entry.target);
                    }
                }
            },
            { threshold: 0.12, rootMargin: "0px 0px -8% 0px" }
        );
        return observer;
    }

    function initReveal() {
        // Nếu người dùng tắt animation (hoặc ?noanim=1 khi test) thì hiện tất cả ngay lập tức
        if (window.matchMedia("(prefers-reduced-motion: reduce)").matches ||
            new URLSearchParams(location.search).has("noanim")) {
            document.querySelectorAll("[data-reveal]").forEach((el) => el.classList.add("is-visible"));
            return;
        }
        const obs = ensureObserver();
        document.querySelectorAll("[data-reveal]:not(.wd-observed)").forEach((el) => {
            el.classList.add("wd-observed");
            obs.observe(el);
        });
    }

    // ---- Audio -----------------------------------------------------------
    function audioPlay(id) {
        const el = document.getElementById(id);
        if (!el) return Promise.resolve(false);
        return el.play().then(() => true).catch(() => false);
    }

    function audioPause(id) {
        const el = document.getElementById(id);
        if (el) el.pause();
    }

    // Báo cho Blazor biết file nhạc có tồn tại không (để ẩn nút nhạc nếu thiếu)
    function audioAvailable(id) {
        return new Promise((resolve) => {
            const el = document.getElementById(id);
            if (!el) return resolve(false);
            if (el.readyState >= 2) return resolve(true);
            const ok = () => { cleanup(); resolve(true); };
            const bad = () => { cleanup(); resolve(false); };
            const cleanup = () => {
                el.removeEventListener("canplay", ok);
                el.removeEventListener("error", bad);
            };
            el.addEventListener("canplay", ok);
            el.addEventListener("error", bad);
            el.load();
            setTimeout(() => { cleanup(); resolve(el.readyState >= 2); }, 4000);
        });
    }

    // ---- Lightbox: swipe trên mobile + phím mũi tên trên desktop ---------
    let lightboxCleanup = null;

    function lightboxAttach(el, dotnetRef) {
        lightboxDetach();
        let startX = 0, startY = 0, tracking = false;

        const onTouchStart = (e) => {
            tracking = true;
            startX = e.touches[0].clientX;
            startY = e.touches[0].clientY;
        };
        const onTouchEnd = (e) => {
            if (!tracking) return;
            tracking = false;
            const dx = e.changedTouches[0].clientX - startX;
            const dy = e.changedTouches[0].clientY - startY;
            if (Math.abs(dx) > 48 && Math.abs(dx) > Math.abs(dy) * 1.5) {
                dotnetRef.invokeMethodAsync("OnLightboxSwipe", dx < 0 ? "next" : "prev");
            }
        };
        const onKey = (e) => {
            if (e.key === "ArrowRight") dotnetRef.invokeMethodAsync("OnLightboxSwipe", "next");
            else if (e.key === "ArrowLeft") dotnetRef.invokeMethodAsync("OnLightboxSwipe", "prev");
            else if (e.key === "Escape") dotnetRef.invokeMethodAsync("OnLightboxSwipe", "close");
        };

        el.addEventListener("touchstart", onTouchStart, { passive: true });
        el.addEventListener("touchend", onTouchEnd, { passive: true });
        document.addEventListener("keydown", onKey);
        document.body.style.overflow = "hidden";

        lightboxCleanup = () => {
            el.removeEventListener("touchstart", onTouchStart);
            el.removeEventListener("touchend", onTouchEnd);
            document.removeEventListener("keydown", onKey);
            document.body.style.overflow = "";
        };
    }

    function lightboxAttachById(elementId, dotnetRef) {
        const el = document.getElementById(elementId);
        if (el) lightboxAttach(el, dotnetRef);
    }

    function lightboxDetach() {
        if (lightboxCleanup) { lightboxCleanup(); lightboxCleanup = null; }
    }

    // ---- Copy to clipboard ------------------------------------------------
    function copyText(text) {
        if (navigator.clipboard && window.isSecureContext) {
            return navigator.clipboard.writeText(text).then(() => true).catch(() => legacyCopy(text));
        }
        return Promise.resolve(legacyCopy(text));
    }

    function legacyCopy(text) {
        const ta = document.createElement("textarea");
        ta.value = text;
        ta.style.position = "fixed";
        ta.style.opacity = "0";
        document.body.appendChild(ta);
        ta.select();
        let ok = false;
        try { ok = document.execCommand("copy"); } catch { ok = false; }
        document.body.removeChild(ta);
        return ok;
    }

    // ---- QR code (qrcode.js qua CDN, có fallback) --------------------------
    function makeQr(elementId, text) {
        const el = document.getElementById(elementId);
        if (!el || typeof window.QRCode === "undefined") return false;
        // qrcode.js ném "code length overflow" với chữ có dấu unicode —
        // bắt mọi lỗi để không làm sập Blazor, fallback hiện thông tin chữ.
        try {
            el.innerHTML = "";
            new window.QRCode(el, {
                text: text,
                width: 180,
                height: 180,
                colorDark: "#4a4038",
                colorLight: "#faf6f0",
                correctLevel: window.QRCode.CorrectLevel.M
            });
            return true;
        } catch (e) {
            console.warn("makeQr failed:", e.message);
            el.innerHTML = "";
            return false;
        }
    }

    // ---- Misc ---------------------------------------------------------------
    function scrollToTop() {
        window.scrollTo({ top: 0, behavior: "instant" in window ? "instant" : "auto" });
    }

    // ---- Cánh hoa hồng nhạt rơi toàn trang -------------------------------
    function initPetals() {
        if (window.matchMedia("(prefers-reduced-motion: reduce)").matches) return;
        if (document.getElementById("wd-petals")) return;

        const host = document.createElement("div");
        host.id = "wd-petals";
        host.setAttribute("aria-hidden", "true");

        const count = Math.min(16, Math.max(10, Math.floor(window.innerWidth / 90)));
        for (let i = 0; i < count; i++) {
            const petal = document.createElement("span");
            petal.className = "wd-petal";
            const size = 9 + Math.random() * 9;
            petal.style.left = (Math.random() * 100).toFixed(1) + "vw";
            petal.style.width = size.toFixed(1) + "px";
            petal.style.height = (size * 1.3).toFixed(1) + "px";
            petal.style.setProperty("--sway", (2 + Math.random() * 5).toFixed(1) + "vw");
            petal.style.animationDuration = (10 + Math.random() * 10).toFixed(1) + "s";
            petal.style.animationDelay = (-Math.random() * 20).toFixed(1) + "s"; // delay âm: rải đều ngay từ đầu
            petal.style.opacity = (0.3 + Math.random() * 0.35).toFixed(2);
            host.appendChild(petal);
        }
        document.body.appendChild(host);
    }

    // Cuộn tới #fragment sau khi Blazor render xong (deep-link vào từng section)
    function scrollToFragment() {
        const id = location.hash.slice(1);
        if (!id) return;
        const el = document.getElementById(id);
        if (el) el.scrollIntoView({ behavior: "auto", block: "start" });
    }

    return {
        initReveal,
        audioPlay,
        audioPause,
        audioAvailable,
        lightboxAttach,
        lightboxAttachById,
        lightboxDetach,
        copyText,
        makeQr,
        scrollToTop,
        scrollToFragment,
        initPetals
    };
})();
