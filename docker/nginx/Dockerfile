FROM nginxinc/nginx-unprivileged:alpine

USER root
RUN apk add --no-cache gettext

COPY docker/nginx/nginx.conf.template /etc/nginx/nginx.conf.template
COPY docker/nginx/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

USER nginx
ENTRYPOINT ["/docker-entrypoint.sh"]
