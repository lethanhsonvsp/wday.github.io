#!/usr/bin/env bash
# ============================================================
# Xuất bản & chạy Thiệp cưới (Blazor WASM Hosted) trên Linux
# Server bind 0.0.0.0 → mọi thiết bị trong mạng truy cập được.
#
# Cách dùng (chạy trên máy Linux, tại thư mục repo):
#   ./deploy-linux.sh                  # publish + chạy, port mặc định 8080
#   PORT=80 ./deploy-linux.sh          # đổi port (port <1024 cần sudo)
#   SELF_CONTAINED=1 ./deploy-linux.sh # đóng gói kèm .NET runtime
#                                      # (máy chạy không cần cài .NET)
#   ./deploy-linux.sh run              # bỏ qua publish, chạy bản có sẵn
#
# Truy cập từ ngoài Internet: cần VPS có IP public, hoặc mở
# port-forward trên router, hoặc dùng tunnel (cloudflared/ngrok).
# Muốn chạy nền lâu dài, cân nhắc tạo service systemd (xem cuối file).
# ============================================================
set -euo pipefail

PORT="${PORT:-8080}"
ROOT="$(cd "$(dirname "$0")" && pwd)"
SERVER_PROJ="$ROOT/wday.github.io/wday.github.io/wday.github.io.csproj"
OUT="$ROOT/publish/linux"
APP_DLL="wday.github.io.dll"
APP_BIN="wday.github.io"

# ---- 1. Kiểm tra dotnet SDK -------------------------------------------------
if ! command -v dotnet >/dev/null 2>&1; then
    echo "❌ Chưa có .NET SDK. Cài bằng:"
    echo "   Ubuntu/Debian : sudo apt-get install -y dotnet-sdk-10.0"
    echo "   Hoặc xem      : https://learn.microsoft.com/dotnet/core/install/linux"
    exit 1
fi

# ---- 2. Publish (trừ khi gọi './deploy-linux.sh run') -----------------------
if [[ "${1:-}" != "run" ]]; then
    echo "==> Publish bản Release vào $OUT"
    if [[ "${SELF_CONTAINED:-0}" == "1" ]]; then
        dotnet publish "$SERVER_PROJ" -c Release -r linux-x64 \
            --self-contained true -o "$OUT"
    else
        dotnet publish "$SERVER_PROJ" -c Release -o "$OUT"
    fi
fi

if [[ ! -f "$OUT/$APP_DLL" && ! -f "$OUT/$APP_BIN" ]]; then
    echo "❌ Không thấy bản publish trong $OUT — chạy lại không kèm 'run'."
    exit 1
fi

# ---- 3. Mở firewall cho port (nếu máy có ufw/firewalld) ---------------------
if command -v ufw >/dev/null 2>&1; then
    sudo ufw allow "$PORT/tcp" >/dev/null 2>&1 || true
elif command -v firewall-cmd >/dev/null 2>&1; then
    sudo firewall-cmd --add-port="$PORT/tcp" --permanent >/dev/null 2>&1 || true
    sudo firewall-cmd --reload >/dev/null 2>&1 || true
fi

# ---- 4. In các địa chỉ truy cập ---------------------------------------------
echo ""
echo "==> Địa chỉ truy cập (mở trên điện thoại/máy khác cùng mạng):"
for ip in $(hostname -I 2>/dev/null || true); do
    echo "    http://$ip:$PORT/wedding/ngoc-nam-lan-anh"
done
PUBLIC_IP="$(curl -s --max-time 3 ifconfig.me 2>/dev/null || true)"
if [[ -n "$PUBLIC_IP" ]]; then
    echo "    IP public (cần mở port trên router/VPS): http://$PUBLIC_IP:$PORT"
fi
echo ""

# ---- 5. Chạy server, bind mọi network interface -----------------------------
# SQLite (wedding.db) tạo tại thư mục publish — giữ nguyên thư mục này
# giữa các lần chạy để không mất RSVP/lời chúc đã nhận.
cd "$OUT"
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:$PORT"

echo "==> Server chạy tại 0.0.0.0:$PORT — Ctrl+C để dừng."
if [[ -x "./$APP_BIN" ]]; then
    exec "./$APP_BIN"
else
    exec dotnet "$APP_DLL"
fi

# ============================================================
# (Tham khảo) Chạy nền như service systemd — tạo file
# /etc/systemd/system/wedding.service với nội dung:
#
#   [Unit]
#   Description=Thiep cuoi Ngoc Nam & Lan Anh
#   After=network.target
#
#   [Service]
#   WorkingDirectory=/duong/dan/repo/publish/linux
#   ExecStart=/usr/bin/dotnet wday.github.io.dll
#   Environment=ASPNETCORE_ENVIRONMENT=Production
#   Environment=ASPNETCORE_URLS=http://0.0.0.0:8080
#   Restart=always
#
#   [Install]
#   WantedBy=multi-user.target
#
# Rồi: sudo systemctl daemon-reload && sudo systemctl enable --now wedding
# ============================================================
