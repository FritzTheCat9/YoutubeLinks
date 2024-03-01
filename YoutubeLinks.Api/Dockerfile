#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YoutubeLinks.Api/YoutubeLinks.Api.csproj", "YoutubeLinks.Api/"]
COPY ["YoutubeLinks.Shared/YoutubeLinks.Shared.csproj", "YoutubeLinks.Shared/"]
RUN dotnet restore "./YoutubeLinks.Api/YoutubeLinks.Api.csproj"
COPY . .
WORKDIR "/src/YoutubeLinks.Api"
RUN dotnet build "./YoutubeLinks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./YoutubeLinks.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YoutubeLinks.Api.dll"]