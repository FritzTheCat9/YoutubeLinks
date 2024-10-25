using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services
{
    public interface IYoutubeDownloader
    {
        Task<YoutubeFile> Download(string videoId, string videoTitle = null);
    }

    public class Mp3YoutubeDownloader : IYoutubeDownloader
    {
        private readonly IYoutubeService _youtubeService;

        public Mp3YoutubeDownloader(
            IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<YoutubeFile> Download(string videoId, string videoTitle = null)
        {
            return await _youtubeService.GetMp3File(videoId, videoTitle);
        }
    }

    public class Mp4YoutubeDownloader : IYoutubeDownloader
    {
        private readonly IYoutubeService _youtubeService;

        public Mp4YoutubeDownloader(
            IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<YoutubeFile> Download(string videoId, string videoTitle = null)
        {
            return await _youtubeService.GetMp4File(videoId, videoTitle);
        }
    }

    public static class YoutubeDownloaderHelpers
    {
        public static IYoutubeDownloader GetYoutubeDownloader(YoutubeFileType fileType, IYoutubeService youtubeService)
        {
            return fileType switch
            {
                YoutubeFileType.Mp4 => new Mp4YoutubeDownloader(youtubeService),
                _ => new Mp3YoutubeDownloader(youtubeService),
            };
        }
    }
}
