FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-bionic AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-bionic AS build
WORKDIR /src

COPY ["src/SME.SR.Application/SME.SR.Application.csproj", "src/SME.SR.Application/"]
COPY ["src/SME.SR.Data/SME.SR.Data.csproj", "src/SME.SR.Data/"]
COPY ["src/SME.SR.Infra/SME.SR.Infra.csproj", "src/SME.SR.Infra/"]
COPY ["src/SME.SR.JRSClient/SME.SR.JRSClient.csproj", "src/SME.SR.JRSClient/"]
COPY ["src/SME.SR.Workers.SGP/SME.SR.Workers.SGP.csproj", "src/SME.SR.Workers.SGP/"]
RUN dotnet restore "src/SME.SR.Workers.SGP/SME.SR.Workers.SGP.csproj"
COPY . .
WORKDIR "/src/src/SME.SR.Workers.SGP"
RUN dotnet build "SME.SR.Workers.SGP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SME.SR.Workers.SGP.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get install -y wget libfontconfig1 libgdiplus libxrender1 unzip fontconfig

RUN wget https://github.com/googlefonts/RobotoMono/archive/refs/heads/main.zip -O robotomono.zip \
    && wget https://github.com/googlefonts/roboto/archive/refs/heads/main.zip -O roboto.zip \
    && wget https://github.com/googlefonts/opensans/archive/refs/heads/main.zip -O opensans.zip \
    && unzip robotomono.zip && mkdir /usr/share/fonts/truetype/robotomono \
    && unzip roboto.zip -d roboto && mkdir /usr/share/fonts/truetype/roboto \
    && unzip opensans.zip -d opensans && mkdir /usr/share/fonts/truetype/opensans

RUN find roboto/ -name "*.ttf" -exec install -m644 {} /usr/share/fonts/truetype/roboto/ \; || return 1 \
     && rm -rf roboto.zip && rm -rf roboto
     
RUN find opensans/ -name "*.ttf" -exec install -m644 {} /usr/share/fonts/truetype/opensans/ \; || return 1 \
     && rm -rf opensans.zip && rm -rf opensans

RUN find RobotoMono-main/fonts/ttf/ -name "*.ttf" -exec install -m644 {} /usr/share/fonts/truetype/robotomono/ \; || return 1 \
     && rm -rf robotomono.zip && rm -rf RobotoMono-main && rm -rf /usr/share/fonts/truetype/dejavu \ && fc-cache -f && rm -rf /var/cache/*
    
ENTRYPOINT ["dotnet", "SME.SR.Workers.SGP.dll"]
