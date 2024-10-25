using System.IO.Compression;

namespace YoutubeLinks.Api.Services;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IWebHostEnvironment webHostEnvironment)
    {
        // unpack ffmpeg.zip file (for yt-dlp on windows)
        var ffmpegZipPath = Path.Combine(Path.GetFullPath(webHostEnvironment.ContentRootPath), "ffmpeg.zip");
        var ffmpegPath = Path.GetFullPath(webHostEnvironment.ContentRootPath);
        if (!File.Exists(Path.Combine(ffmpegPath, "ffmpeg.exe")))
            ZipFile.ExtractToDirectory(ffmpegZipPath, ffmpegPath);

        services.AddScoped<IYoutubeService, YoutubeService>();

        return services;
    }
}