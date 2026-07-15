#!/usr/bin/env bash
# ============================================================
# Đưa thiệp cưới ra Internet KHÔNG cần mua tên miền,
# không cần VPS, không cần mở port router.
#
# Dùng Cloudflare Quick Tunnel: miễn phí, không cần tài khoản,
# tự có HTTPS. Chạy xong nhận URL dạng:
#     https://<ten-ngau-nhien>.trycloudflare.com
# → gửi link đó cho khách là ai cũng mở được, mạng nào cũng vào được.
#
# Cách dùng (trên máy Linux, tại thư mục repo):
#   ./tunnel-linux.sh            # tự chạy app (nếu chưa) + mở tunnel
#   PORT=8080 ./tunnel-linux.sh  # đổi port app
#
# LƯU Ý: URL trycloudflare đổi mỗi lần chạy lại tunnel — hãy giữ
# tunnel chạy liên tục trong thời gian mời khách (chạy trong tmux/screen).
# Muốn URL cố định mà vẫn miễn phí: dùng ngrok — tài khoản free được
# 1 domain tĩnh dạng <ten-ban>.ngrok-free.app:
#   ngrok config add-authtoken <token-tu-dashboard>
#   ngrok http --domain=<ten-ban>.ngrok-free.app 8080
# ============================================================
set -euo pipefail

PORT="${PORT:-8080}"
ROOT="$(cd "$(dirname "$0")" && pwd)"
OUT="$ROOT/publish/linux"
APP_PID=""

# Dọn dẹp khi thoát: nếu script tự bật app thì tắt app theo
cleanup() { [[ -n "$APP_PID" ]] && kill "$APP_PID" 2>/dev/null || true; }
trap cleanup EXIT

# ---- 1. Bảo đảm app đang chạy ------------------------------------------------
if curl -sf --max-time 2 "http://localhost:$PORT" >/dev/null 2>&1; then
    echo "==> App đã chạy sẵn ở port $PORT."
else
    if [[ ! -f "$OUT/wday.github.io.dll" && ! -x "$OUT/wday.github.io" ]]; then
        echo "==> Chưa có bản publish — publish trước..."
        dotnet publish "$ROOT/wday.github.io/wday.github.io/wday.github.io.csproj" \
            -c Release -o "$OUT"
    fi
    echo "==> Khởi động app ở port $PORT (log: $OUT/app.log)..."
    (
        cd "$OUT"
        export ASPNETCORE_ENVIRONMENT=Production
        export ASPNETCORE_URLS="http://0.0.0.0:$PORT"
        if [[ -x "./wday.github.io" ]]; then
            exec ./wday.github.io
        else
            exec dotnet wday.github.io.dll
        fi
    ) > "$OUT/app.log" 2>&1 &
    APP_PID=$!

    # Đợi app sẵn sàng (tối đa ~30s)
    for _ in $(seq 1 30); do
        curl -sf --max-time 2 "http://localhost:$PORT" >/dev/null 2>&1 && break
        sleep 1
    done
    if ! curl -sf --max-time 2 "http://localhost:$PORT" >/dev/null 2>&1; then
        echo "❌ App không khởi động được — xem log: $OUT/app.log"
        exit 1
    fi
fi

# ---- 2. Cài cloudflared nếu chưa có -------------------------------------------
CLOUDFLARED="cloudflared"
if ! command -v cloudflared >/dev/null 2>&1; then
    mkdir -p "$ROOT/bin"
    CLOUDFLARED="$ROOT/bin/cloudflared"
    if [[ ! -x "$CLOUDFLARED" ]]; then
        case "$(uname -m)" in
            x86_64)         ARCH=amd64 ;;
            aarch64|arm64)  ARCH=arm64 ;;
            armv7l)         ARCH=arm   ;;
            *) echo "❌ Kiến trúc CPU không hỗ trợ: $(uname -m)"; exit 1 ;;
        esac
        echo "==> Tải cloudflared ($ARCH)..."
        curl -fL --progress-bar -o "$CLOUDFLARED" \
            "https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-$ARCH"
        chmod +x "$CLOUDFLARED"
    fi
fi

# ---- 3. Mở tunnel — URL hiện trong log ngay dưới đây --------------------------
echo ""
echo "==> Đang mở tunnel... Tìm dòng https://XXXX.trycloudflare.com bên dưới,"
echo "    gửi link đó (thêm /wedding/ngoc-nam-lan-anh) cho khách. Ctrl+C để dừng."
echo ""
"$CLOUDFLARED" tunnel --no-autoupdate --url "http://localhost:$PORT"
