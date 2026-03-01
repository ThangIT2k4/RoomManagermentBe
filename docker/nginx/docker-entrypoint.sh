#!/bin/sh
set -e

# Template và output paths
TEMPLATE="/etc/nginx/nginx.conf.template"
OUTPUT="/etc/nginx/nginx.conf"

# Kiểm tra template có tồn tại
if [ ! -f "$TEMPLATE" ]; then
    echo "ERROR: Template file not found: $TEMPLATE"
    exit 1
fi

# Danh sách biến môi trường cần thay thế (chỉ những biến này mới được thay)
# Format: $VAR1 $VAR2 $VAR3
VARS_TO_SUBST='$WORKER_PROCESSES $WORKER_CONNECTIONS $ACCESS_LOG_PATH $ERROR_LOG_PATH $LOG_LEVEL $GZIP_ENABLED $GZIP_MIN_LENGTH $RATE_LIMIT $CLIENT_BODY_TIMEOUT $CLIENT_HEADER_TIMEOUT $KEEPALIVE_TIMEOUT $SEND_TIMEOUT $CLIENT_MAX_BODY_SIZE'


# Đặt giá trị mặc định nếu biến chưa set (envsubst không hiểu ${VAR:-default})
export WORKER_PROCESSES="${WORKER_PROCESSES:-auto}"
export WORKER_CONNECTIONS="${WORKER_CONNECTIONS:-4096}"
export ACCESS_LOG_PATH="${ACCESS_LOG_PATH:-/var/log/nginx/access.log}"
export ERROR_LOG_PATH="${ERROR_LOG_PATH:-/var/log/nginx/error.log}"
export LOG_LEVEL="${LOG_LEVEL:-warn}"
export GZIP_ENABLED="${GZIP_ENABLED:-on}"
export GZIP_MIN_LENGTH="${GZIP_MIN_LENGTH:-1024}"
export RATE_LIMIT="${RATE_LIMIT:-10r/s}"
export CLIENT_BODY_TIMEOUT="${CLIENT_BODY_TIMEOUT:-10s}"
export CLIENT_HEADER_TIMEOUT="${CLIENT_HEADER_TIMEOUT:-10s}"
export KEEPALIVE_TIMEOUT="${KEEPALIVE_TIMEOUT:-65s}"
export SEND_TIMEOUT="${SEND_TIMEOUT:-10s}"
export CLIENT_MAX_BODY_SIZE="${CLIENT_MAX_BODY_SIZE:-50M}"

# Thay thế biến môi trường vào template (CHỈ thay những biến được chỉ định)
envsubst "$VARS_TO_SUBST" < "$TEMPLATE" > "$OUTPUT"

echo "✓ Generated nginx.conf from template"
echo "✓ Validating nginx configuration..."

# Kiểm tra syntax nginx
nginx -t

echo "✓ Nginx configuration is valid"
echo "✓ Starting nginx..."

# Start nginx ở foreground
exec nginx -g "daemon off;"