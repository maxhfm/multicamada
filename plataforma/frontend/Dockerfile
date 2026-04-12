FROM nginx:1.25-alpine

# Remove config padrão
RUN rm /etc/nginx/conf.d/default.conf

# Copia sua config
COPY nginx.conf /etc/nginx/nginx.conf

COPY index.html /usr/share/nginx/html/index.html

# Garante permissões amplas
RUN chmod -R 777 /tmp \
    && chmod -R 777 /usr/share/nginx/html

EXPOSE 8080