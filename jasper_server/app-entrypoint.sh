#!/bin/bash -e

. /opt/bitnami/base/functions
. /opt/bitnami/base/helpers

print_welcome_page


if [[ "$1" == "nami" && "$2" == "start" ]] || [[ "$1" == "/init.sh" ]]; then
    nami_initialize tomcat mysql-client jasperreports
    info "Starting Jasper Server Edited... "
fi

cp web.xml bitnami/tomcat/data/jasperserver/WEB-INF/

exec tini -- "$@"
