#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CollectNews/source/AgregaNews.CollectNews.Api/AgregaNews.CollectNews.Api.csproj", "source/AgregaNews.CollectNews.Api/"]
COPY ["CollectNews/source/AgregaNews.CollectNews.Application/AgregaNews.CollectNews.Application.csproj", "source/AgregaNews.CollectNews.Application/"]
COPY ["CollectNews/source/AgregaNews.CollectNews.Domain/AgregaNews.CollectNews.Domain.csproj", "source/AgregaNews.CollectNews.Domain/"]
COPY ["CollectNews/source/AgregaNews.CollectNews.Infrastructure/AgregaNews.CollectNews.Infrastructure.csproj", "source/AgregaNews.CollectNews.Infrastructure/"]
COPY ["Common/Packages", "source/common/packages"]
COPY ["Common/NuGet.config", "."]
RUN dotnet restore "./source/AgregaNews.CollectNews.Api/./AgregaNews.CollectNews.Api.csproj"
COPY CollectNews .
WORKDIR "/src/source/AgregaNews.CollectNews.Api"
RUN dotnet build "./AgregaNews.CollectNews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AgregaNews.CollectNews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgregaNews.CollectNews.Api.dll"]