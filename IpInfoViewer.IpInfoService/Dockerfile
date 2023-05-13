#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IpInfoViewer.IpInfoService/IpInfoViewer.IpInfoService.csproj", "IpInfoViewer.IpInfoService/"]
COPY ["IpInfoViewer.Libs/IpInfoViewer.Libs.csproj", "IpInfoViewer.Libs/"]
RUN dotnet restore "IpInfoViewer.IpInfoService/IpInfoViewer.IpInfoService.csproj"
COPY . .
WORKDIR "/src/IpInfoViewer.IpInfoService"
RUN dotnet build "IpInfoViewer.IpInfoService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IpInfoViewer.IpInfoService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IpInfoViewer.IpInfoService.dll"]