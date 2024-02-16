#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AnalyzeNews/source/AgregaNews.AnalyzeNews.Api/AgregaNews.AnalyzeNews.Api.csproj", "source/AgregaNews.AnalyzeNews.Api/"]
COPY ["AnalyzeNews/source/AgregaNews.AnalyzeNews.Application/AgregaNews.AnalyzeNews.Application.csproj", "source/AgregaNews.AnalyzeNews.Application/"]
COPY ["AnalyzeNews/source/AgregaNews.AnalyzeNews.Domain/AgregaNews.AnalyzeNews.Domain.csproj", "source/AgregaNews.AnalyzeNews.Domain/"]
COPY ["AnalyzeNews/source/AgregaNews.AnalyzeNews.Infrastructure/AgregaNews.AnalyzeNews.Infrastructure.csproj", "source/AgregaNews.AnalyzeNews.Infrastructure/"]
COPY ["Common/Packages", "source/common/packages"]
COPY ["Common/NuGet.config", "."]
RUN dotnet restore "./source/AgregaNews.AnalyzeNews.Api/AgregaNews.AnalyzeNews.Api.csproj"
COPY AnalyzeNews .
WORKDIR "/src/source/AgregaNews.AnalyzeNews.Api"
RUN dotnet build "./AgregaNews.AnalyzeNews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AgregaNews.AnalyzeNews.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AgregaNews.AnalyzeNews.Api.dll"]