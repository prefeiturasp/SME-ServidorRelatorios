FROM bitnami/jasperreports:7.2.0

COPY ./jasper_server/WEB-INF/web.xml /
COPY ./jasper_server/app-entrypoint.sh /

RUN chmod +x /app-entrypoint.sh