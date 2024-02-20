#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LogService/source/AgregaNews.Log.LogApi/AgregaNews.Log.LogApi.csproj", "source/AgregaNews.Log.LogApi/"]
COPY ["LogService/source/AgregaNews.Log.Application/AgregaNews.Log.Application.csproj", "source/AgregaNews.Log.Application/"]
COPY ["LogService/source/AgregaNews.Log.Domain/AgregaNews.Log.Domain.csproj", "source/AgregaNews.Log.Domain/"]
COPY ["LogService/source/AgregaNews.Log.Infrastructure/AgregaNews.Log.Infrastructure.csproj", "source/AgregaNews.Log.Infrastructure/"]
COPY ["Common/Packages", "source/common/packages"]
COPY ["Common/NuGet.config", "."]
RUN dotnet restore "./source/AgregaNews.Log.LogApi/./AgregaNews.Log.LogApi.csproj"
COPY LogService .
WORKDIR "/src/source/AgregaNews.Log.LogApi"
RUN dotnet build "./AgregaNews.Log.LogApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AgregaNews.Log.LogApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgregaNews.Log.LogApi.dll"]