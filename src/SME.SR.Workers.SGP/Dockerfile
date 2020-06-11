FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic

ARG SME_SERVIDOR_RELATORIOS_ENVIRONMENT=dev

ENV SRConnection=$SME_SERVIDOR_RELATORIOS_CONNECTION_STRING
ENV SentryDsn=$SentryDsn
ENV EolConnection=$EolConnection
ENV EolApiConnection=$EolApiConnection
ENV SgpConnection=$SgpConnection
ENV TZ=America/Sao_Paulo
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -yq tzdata && dpkg-reconfigure --frontend noninteractive tzdata

ADD . /src
WORKDIR /src
RUN dotnet restore && \  
    dotnet publish -c Release && \  
    cp -R /src/bin/Release/netcoreapp3.0/publish /app && \ 
    rm -Rf /src

EXPOSE 5010-5011
ENTRYPOINT ["/app/SME.SR.Workers.SGP"]
