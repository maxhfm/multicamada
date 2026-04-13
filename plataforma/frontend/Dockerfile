FROM nginx:1.25-alpine

# Copia sua config (sem deletar nada)
COPY nginx.conf /etc/nginx/conf.d/default.conf

COPY index.html /usr/share/nginx/html/index.html

# Permissões OpenShift
RUN chmod -R 777 /usr/share/nginx/html \
    && chmod -R 777 /var/cache/nginx \
    && chmod -R 777 /var/run \
    && chmod -R 777 /tmp

EXPOSE 8080