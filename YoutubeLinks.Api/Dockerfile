FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

# Install python3 and ffmpeg as root to use yt-dlp on linux container
USER root
RUN apt-get update && apt-get install -y python3 ffmpeg

USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

# Install curl, create app directory, download latest yt-dlp from github to /app/yt-dlp,
# Make yt-dlp file executable and readable to all users
FROM debian:buster-slim AS yt-dlp-download
RUN apt-get update && apt-get install -y curl
RUN mkdir -p /app
RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /app/yt-dlp
RUN chmod a+rx /app/yt-dlp

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["YoutubeLinks.Api/YoutubeLinks.Api.csproj", "YoutubeLinks.Api/"]
# Copy email templates
COPY ["YoutubeLinks.Api/Emails/Templates/", "/src/Emails/Templates/"]
# Copy ffmpeg.zip
COPY ["YoutubeLinks.Api/ffmpeg.zip", "/src/ffmpeg.zip"]
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
# Copy email templates
COPY --from=build /src/Emails/Templates /app/Emails/Templates
# Copy ffmpeg.zip
COPY --from=build /src/ffmpeg.zip /app/ffmpeg.zip
# Create Tmp folder (helper folder to download mp3 and mp4 files)
RUN mkdir /app/Tmp/
# Copy downloaded yt-dlp file to /app/yt-dlp
COPY --from=yt-dlp-download /app/yt-dlp /app/yt-dlp
ENTRYPOINT ["dotnet", "YoutubeLinks.Api.dll"]