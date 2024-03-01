FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY ["YoutubeLinks.Blazor/YoutubeLinks.Blazor.csproj", "YoutubeLinks.Blazor/"]
COPY ["YoutubeLinks.Shared/YoutubeLinks.Shared.csproj", "YoutubeLinks.Shared/"]
RUN dotnet restore "./YoutubeLinks.Blazor/YoutubeLinks.Blazor.csproj"
COPY . .
WORKDIR "/app/YoutubeLinks.Blazor"
RUN dotnet build "./YoutubeLinks.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./YoutubeLinks.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine
EXPOSE 80
EXPOSE 443
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot/ .
COPY ./YoutubeLinks.Blazor/nginx.conf /etc/nginx/nginx.conf