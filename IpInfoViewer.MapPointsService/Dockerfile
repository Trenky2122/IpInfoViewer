#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IpInfoViewer.MapPointsService/IpInfoViewer.MapPointsService.csproj", "IpInfoViewer.MapPointsService/"]
RUN dotnet restore "IpInfoViewer.MapPointsService/IpInfoViewer.MapPointsService.csproj"
COPY . .
WORKDIR "/src/IpInfoViewer.MapPointsService"
RUN dotnet build "IpInfoViewer.MapPointsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IpInfoViewer.MapPointsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IpInfoViewer.MapPointsService.dll"]