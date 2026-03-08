#!/bin/sh
set -eu

TEMPLATE="/etc/nginx/nginx.conf.template"
OUTPUT="/tmp/nginx.conf"

if [ ! -f "$TEMPLATE" ]; then
    echo "ERROR: Template file not found: $TEMPLATE"
    exit 1
fi

VARS_TO_SUBST='$WORKER_PROCESSES $WORKER_CONNECTIONS $ACCESS_LOG_PATH $ERROR_LOG_PATH $LOG_LEVEL $GZIP_ENABLED $GZIP_MIN_LENGTH $RATE_LIMIT $CLIENT_BODY_TIMEOUT $CLIENT_HEADER_TIMEOUT $KEEPALIVE_TIMEOUT $SEND_TIMEOUT $CLIENT_MAX_BODY_SIZE'

export WORKER_PROCESSES="${WORKER_PROCESSES:-auto}"
export WORKER_CONNECTIONS="${WORKER_CONNECTIONS:-4096}"
export ACCESS_LOG_PATH="${ACCESS_LOG_PATH:-/dev/stdout}"
export ERROR_LOG_PATH="${ERROR_LOG_PATH:-/dev/stderr}"
export LOG_LEVEL="${LOG_LEVEL:-warn}"
export GZIP_ENABLED="${GZIP_ENABLED:-on}"
export GZIP_MIN_LENGTH="${GZIP_MIN_LENGTH:-1024}"
export RATE_LIMIT="${RATE_LIMIT:-10r/s}"
export CLIENT_BODY_TIMEOUT="${CLIENT_BODY_TIMEOUT:-10s}"
export CLIENT_HEADER_TIMEOUT="${CLIENT_HEADER_TIMEOUT:-10s}"
export KEEPALIVE_TIMEOUT="${KEEPALIVE_TIMEOUT:-65s}"
export SEND_TIMEOUT="${SEND_TIMEOUT:-10s}"
export CLIENT_MAX_BODY_SIZE="${CLIENT_MAX_BODY_SIZE:-50M}"

envsubst "$VARS_TO_SUBST" < "$TEMPLATE" > "$OUTPUT"

echo "Generated nginx.conf from template"
echo "Validating nginx configuration..."

nginx -t -c "$OUTPUT"

echo "Nginx configuration is valid"
echo "Starting nginx..."

exec nginx -c "$OUTPUT" -g "daemon off;"
