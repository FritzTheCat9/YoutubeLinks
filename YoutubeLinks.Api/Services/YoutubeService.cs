using System.Diagnostics;
using System.Text;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services
{
    public interface IYoutubeService
    {
        Task<(byte[], string)> GetMp3File(string videoId);
        Task<string> GetVideoTitle(string videoId);
    }

    public partial class YoutubeService : IYoutubeService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<YoutubeService> _logger;
        private readonly string _ytDlpPath;
        private readonly string _tmpFolderPath;

        public YoutubeService(
            IWebHostEnvironment webHostEnvironment,
            ILogger<YoutubeService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _ytDlpPath = $"{Path.GetFullPath(_webHostEnvironment.ContentRootPath)}/yt-dlp.exe";
            _tmpFolderPath = $"{Path.GetFullPath(_webHostEnvironment.ContentRootPath)}/Tmp";
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
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
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

            var sanitizedTitle = string.Join("_", videoTitle.Split(Path.GetInvalidFileNameChars()));

            return sanitizedTitle;
        }

        public async Task<(byte[], string)> GetMp3File(string videoId)
        {
            var title = await GetVideoTitle(videoId);
            var sanitizedTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            var fileName = $"{sanitizedTitle}.mp3";
            var arguments = $"-x --audio-format mp3 --audio-quality 0 --output \"{_tmpFolderPath}\\{fileName}\" \"{YoutubeHelpers.VideoPathBase}{videoId}\"";

            var startInfo = new ProcessStartInfo
            {
                FileName = _ytDlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;

                process.Start();
                await process.WaitForExitAsync();
            }

            var filePath = Path.Combine(_tmpFolderPath, fileName);

            if (!File.Exists(filePath))
                throw new MyServerException();

            var result = (File.ReadAllBytes(filePath), fileName);

            File.Delete(filePath);

            return result;
        }
    }
}
