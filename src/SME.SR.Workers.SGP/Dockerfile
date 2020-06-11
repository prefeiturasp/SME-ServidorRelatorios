FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
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
ENTRYPOINT ["dotnet", "SME.SR.Workers.SGP.dll"]