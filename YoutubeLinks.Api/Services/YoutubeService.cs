using System.Diagnostics;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services
{
    public interface IYoutubeService
    {
        Task<string> GetVideoTitle(string videoId);
    }

    public partial class YoutubeService : IYoutubeService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<YoutubeService> _logger;
        private readonly string _ytDlpPath;

        public YoutubeService(
            IWebHostEnvironment webHostEnvironment,
            ILogger<YoutubeService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _ytDlpPath = $"{Path.GetFullPath(_webHostEnvironment.ContentRootPath)}/yt-dlp.exe";
        }

        public async Task<string> GetVideoTitle(string videoId)
        {
            var videoTitle = "";
            var arguments = $"--get-title \"{YoutubeHelpers.VideoPathBase}{videoId}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = _ytDlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;

                var taskCompletionSource = new TaskCompletionSource<string>();

                process.OutputDataReceived += (sender, e) =>
                {
                    _logger.LogInformation(e.Data);

                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        taskCompletionSource.TrySetResult(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    _logger.LogError(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();

                videoTitle = await taskCompletionSource.Task;
            }

            return videoTitle;
        }
    }
}
