#!/usr/bin/env bash
# ============================================================
# Xuất bản & chạy Thiệp cưới (Blazor WASM Hosted) trên Linux
# Server bind 0.0.0.0 → mọi thiết bị trong mạng truy cập được.
#
# Cách dùng (chạy trên máy Linux, tại thư mục repo):
#   ./deploy-linux.sh                  # publish + chạy foreground (Ctrl+C dừng)
#   ./deploy-linux.sh install          # publish + cài thành SERVICE systemd:
#                                      #   chạy nền 24/7, tự bật lại khi reboot
#   ./deploy-linux.sh uninstall        # gỡ service
#   ./deploy-linux.sh run              # chạy foreground bản publish sẵn có
#
#   PORT=80 ./deploy-linux.sh install  # đổi port (port <1024 cần sudo)
#   SELF_CONTAINED=1 ./deploy-linux.sh # đóng gói kèm .NET runtime
#                                      # (máy chạy không cần cài .NET)
#
# Sau khi install: quản lý bằng systemctl
#   sudo systemctl status wedding      # xem trạng thái
#   sudo systemctl restart wedding     # khởi động lại
#   journalctl -u wedding -f           # xem log trực tiếp
#
# Truy cập từ ngoài Internet không cần mua domain: ./tunnel-linux.sh
# ============================================================
set -euo pipefail

MODE="${1:-serve}"   # serve | run | install | uninstall
PORT="${PORT:-8080}"
ROOT="$(cd "$(dirname "$0")" && pwd)"
SERVER_PROJ="$ROOT/wday.github.io/wday.github.io/wday.github.io.csproj"
OUT="$ROOT/publish/linux"
APP_DLL="wday.github.io.dll"
APP_BIN="wday.github.io"
SERVICE_NAME="wedding"
SERVICE_FILE="/etc/systemd/system/$SERVICE_NAME.service"

# ---- Gỡ service -------------------------------------------------------------
if [[ "$MODE" == "uninstall" ]]; then
    echo "==> Gỡ service $SERVICE_NAME..."
    sudo systemctl disable --now "$SERVICE_NAME" 2>/dev/null || true
    sudo rm -f "$SERVICE_FILE"
    sudo systemctl daemon-reload
    echo "✅ Đã gỡ. Dữ liệu (wedding.db) vẫn còn trong $OUT."
    exit 0
fi

# ---- 1. Kiểm tra dotnet SDK -------------------------------------------------
if ! command -v dotnet >/dev/null 2>&1; then
    echo "❌ Chưa có .NET SDK. Cài bằng:"
    echo "   Ubuntu/Debian : sudo apt-get install -y dotnet-sdk-10.0"
    echo "   Hoặc xem      : https://learn.microsoft.com/dotnet/core/install/linux"
    exit 1
fi

# ---- 2. Publish (trừ chế độ 'run') ------------------------------------------
if [[ "$MODE" != "run" ]]; then
    echo "==> Publish bản Release vào $OUT"
    if [[ "${SELF_CONTAINED:-0}" == "1" ]]; then
        dotnet publish "$SERVER_PROJ" -c Release -r linux-x64 \
            --self-contained true -o "$OUT"
    else
        dotnet publish "$SERVER_PROJ" -c Release -o "$OUT"
    fi
fi

if [[ ! -f "$OUT/$APP_DLL" && ! -x "$OUT/$APP_BIN" ]]; then
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

# ---- 4. In các địa chỉ truy cập ----------------------------------------------
print_urls() {
    echo ""
    echo "==> Địa chỉ truy cập (mở trên điện thoại/máy khác cùng mạng):"
    for ip in $(hostname -I 2>/dev/null || true); do
        echo "    http://$ip:$PORT/wedding/ngoc-nam-lan-anh"
    done
    local public_ip
    public_ip="$(curl -s --max-time 3 ifconfig.me 2>/dev/null || true)"
    if [[ -n "$public_ip" ]]; then
        echo "    IP public (cần mở port trên router/VPS): http://$public_ip:$PORT"
    fi
    echo ""
}

# ---- 5a. Cài thành service systemd -------------------------------------------
if [[ "$MODE" == "install" ]]; then
    # Chạy service dưới user hiện tại (không phải root) cho an toàn
    RUN_USER="${SUDO_USER:-$USER}"
    if [[ -x "$OUT/$APP_BIN" ]]; then
        EXEC_START="$OUT/$APP_BIN"
    else
        EXEC_START="$(command -v dotnet) $OUT/$APP_DLL"
    fi

    echo "==> Tạo service $SERVICE_FILE (user: $RUN_USER, port: $PORT)"
    sudo tee "$SERVICE_FILE" >/dev/null <<UNIT
[Unit]
Description=Thiep cuoi Ngoc Nam & Lan Anh
After=network.target

[Service]
WorkingDirectory=$OUT
ExecStart=$EXEC_START
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:$PORT
Restart=always
RestartSec=5
User=$RUN_USER

[Install]
WantedBy=multi-user.target
UNIT

    sudo systemctl daemon-reload
    sudo systemctl enable --now "$SERVICE_NAME"
    sleep 2

    if systemctl is-active --quiet "$SERVICE_NAME"; then
        echo "✅ Service đang chạy nền — tự bật lại khi crash/reboot."
    else
        echo "❌ Service không chạy được — xem log: journalctl -u $SERVICE_NAME -e"
        exit 1
    fi
    print_urls
    echo "Quản lý:  sudo systemctl status|restart|stop $SERVICE_NAME"
    echo "Xem log:  journalctl -u $SERVICE_NAME -f"
    exit 0
fi

# ---- 5b. Chạy foreground -------------------------------------------------------
# SQLite (wedding.db) tạo tại thư mục publish — giữ nguyên thư mục này
# giữa các lần chạy để không mất RSVP/lời chúc đã nhận.
print_urls
cd "$OUT"
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:$PORT"

echo "==> Server chạy tại 0.0.0.0:$PORT — Ctrl+C để dừng."
echo "    (Muốn chạy nền 24/7: ./deploy-linux.sh install)"
if [[ -x "./$APP_BIN" ]]; then
    exec "./$APP_BIN"
else
    exec dotnet "$APP_DLL"
fi
