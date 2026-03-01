FROM nginx:alpine

# Cài đặt gettext (chứa envsubst)
RUN apk add --no-cache gettext

# Copy template và entrypoint script
COPY docker/nginx/nginx.conf.template /etc/nginx/nginx.conf.template
COPY docker/nginx/docker-entrypoint.sh /docker-entrypoint.sh

# Set quyền thực thi cho entrypoint
RUN chmod +x /docker-entrypoint.sh

# Copy các file config khác nếu có (conf.d, ssl, etc.)
# COPY docker/nginx/conf.d /etc/nginx/conf.d
# COPY docker/nginx/ssl /etc/nginx/ssl

# Sử dụng entrypoint script
ENTRYPOINT ["/docker-entrypoint.sh"]
