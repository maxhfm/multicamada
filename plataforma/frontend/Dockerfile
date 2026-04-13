FROM nginx:1.25-alpine
#FROM bitnamilegacy/nginx

# REMOVE configs padrão
RUN rm -rf /etc/nginx/conf.d/*

# USA seu nginx completo
COPY nginx.conf /etc/nginx/nginx.conf

COPY index.html /usr/share/nginx/html/index.html

#RUN chmod -R 777 /tmp \
#   && chmod -R 777 /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]