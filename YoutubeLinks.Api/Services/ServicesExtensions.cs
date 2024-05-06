using System.IO.Compression;

namespace YoutubeLinks.Api.Services
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services,
            IWebHostEnvironment webHostEnvironment)
        {
            // unpack ffmpeg.zip file (for yt-dlp on windows)
            string _ffmpegZipPath = Path.Combine(Path.GetFullPath(webHostEnvironment.ContentRootPath), "ffmpeg.zip");
            string _ffmpegPath = Path.GetFullPath(webHostEnvironment.ContentRootPath);
            if (!File.Exists(Path.Combine(_ffmpegPath, "ffmpeg.exe")))
            {
                ZipFile.ExtractToDirectory(_ffmpegZipPath, _ffmpegPath);
            }

            services.AddScoped<IYoutubeService, YoutubeService>();

            return services;
        }
    }
}
