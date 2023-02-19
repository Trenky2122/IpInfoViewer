#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IpInfoViewer/IpInfoViewer.Api.csproj", "IpInfoViewer/"]
COPY ["IpInfoViewer.Libs/IpInfoViewer.Libs.csproj", "IpInfoViewer.Libs/"]
RUN dotnet restore "IpInfoViewer/IpInfoViewer.Api.csproj"
COPY . .
WORKDIR "/src/IpInfoViewer"
RUN dotnet build "IpInfoViewer.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IpInfoViewer.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IpInfoViewer.Api.dll"]