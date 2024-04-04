using System.Text.RegularExpressions;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services
{
    public interface IYoutubeService
    {
        Task<string> GetVideoTitle(string videoId);
        Task<YoutubeFile> GetMP3File(string videoId, string videoTitle = null);
        Task<YoutubeFile> GetMP4File(string videoId, string videoTitle = null);
    }

    public class YoutubeService : IYoutubeService
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
            _ytDlpPath = Path.Combine(Path.GetFullPath(_webHostEnvironment.ContentRootPath), "yt-dlp.exe");
            _tmpFolderPath = Path.Combine(Path.GetFullPath(_webHostEnvironment.ContentRootPath), "Tmp");
        }

        public async Task<string> GetVideoTitle(string videoId)
        {
            var youtubeDL = new YoutubeDL();
            youtubeDL.YoutubeDLPath = _ytDlpPath;

            var videoDataRequest = await youtubeDL.RunVideoDataFetch($"{YoutubeHelpers.VideoPathBase}{videoId}");
            if (!videoDataRequest.Success)
                throw new MyServerException();

            var title = videoDataRequest.Data.Title;
            var normalizedTitle = YoutubeHelpers.NormalizeVideoTitle(title);

            return normalizedTitle;
        }

        public async Task<YoutubeFile> GetMP3File(string videoId, string videoTitle = null)
        {
            var title = YoutubeHelpers.NormalizeVideoTitle(videoTitle) ?? await GetVideoTitle(videoId);
            var fileName = $"{Guid.NewGuid()}.mp3";

            var youtubeDL = new YoutubeDL();
            youtubeDL.YoutubeDLPath = _ytDlpPath;
            youtubeDL.OutputFolder = _tmpFolderPath;

            var options = new OptionSet();
            options.ExtractAudio = true;
            options.AudioQuality = 0;
            options.AudioFormat = AudioConversionFormat.Mp3;
            options.Output = Path.Combine(_tmpFolderPath, fileName);
            options.EmbedThumbnail = true;

            var runResult = await youtubeDL.RunAudioDownload($"{YoutubeHelpers.VideoPathBase}{videoId}", overrideOptions: options);
            if (!runResult.Success)
                throw new MyServerException();

            var filePath = Path.Combine(_tmpFolderPath, fileName);

            if (!File.Exists(filePath))
                throw new MyServerException();

            var normalizedFileName = $"{title}.mp3";
            var normalizedFilePath = Path.Combine(_tmpFolderPath, normalizedFileName);

            File.Move(filePath, normalizedFilePath);

            var youtubeFile = new YoutubeFile
            {
                FileBytes = File.ReadAllBytes(normalizedFilePath),
                ContentType = "audio/mpeg",
                FileName = normalizedFileName,
                YoutubeFileType = YoutubeFileType.MP3,
            };

            File.Delete(normalizedFilePath);

            return youtubeFile;
        }

        public async Task<YoutubeFile> GetMP4File(string videoId, string videoTitle = null)
        {
            var title = YoutubeHelpers.NormalizeVideoTitle(videoTitle) ?? await GetVideoTitle(videoId);
            var fileName = $"{Guid.NewGuid()}.mp4";

            var youtubeDL = new YoutubeDL();
            youtubeDL.YoutubeDLPath = _ytDlpPath;
            youtubeDL.OutputFolder = _tmpFolderPath;

            var options = new OptionSet();
            options.Output = Path.Combine(_tmpFolderPath, fileName);
            options.Format = "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best";
            options.EmbedThumbnail = true;

            var runResult = await youtubeDL.RunVideoDownload($"{YoutubeHelpers.VideoPathBase}{videoId}", overrideOptions: options);
            if (!runResult.Success)
                throw new MyServerException();

            var filePath = Path.Combine(_tmpFolderPath, fileName);

            if (!File.Exists(filePath))
                throw new MyServerException();

            var normalizedFileName = $"{title}.mp4";
            var normalizedFilePath = Path.Combine(_tmpFolderPath, normalizedFileName);

            File.Move(filePath, normalizedFilePath);

            var youtubeFile = new YoutubeFile
            {
                FileBytes = File.ReadAllBytes(normalizedFilePath),
                ContentType = "video/mp4",
                FileName = normalizedFileName,
                YoutubeFileType = YoutubeFileType.MP4,
            };

            File.Delete(normalizedFilePath);

            return youtubeFile;
        }
    }
}
