#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IpInfoViewer.CountryPingInfoService/IpInfoViewer.CountryPingInfoService.csproj", "IpInfoViewer.CountryPingInfoService/"]
RUN dotnet restore "IpInfoViewer.CountryPingInfoService/IpInfoViewer.CountryPingInfoService.csproj"
COPY . .
WORKDIR "/src/IpInfoViewer.CountryPingInfoService"
RUN dotnet build "IpInfoViewer.CountryPingInfoService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IpInfoViewer.CountryPingInfoService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IpInfoViewer.CountryPingInfoService.dll"]