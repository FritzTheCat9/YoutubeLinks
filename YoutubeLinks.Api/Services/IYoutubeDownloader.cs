using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services
{
    public interface IYoutubeDownloader
    {
        Task<YoutubeFile> Download(string videoId, string videoTitle = null);
    }

    public class MP3YoutubeDownloader : IYoutubeDownloader
    {
        private readonly IYoutubeService _youtubeService;

        public MP3YoutubeDownloader(
            IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<YoutubeFile> Download(string videoId, string videoTitle = null)
        {
            return await _youtubeService.GetMP3File(videoId, videoTitle);
        }
    }

    public class MP4YoutubeDownloader : IYoutubeDownloader
    {
        private readonly IYoutubeService _youtubeService;

        public MP4YoutubeDownloader(
            IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<YoutubeFile> Download(string videoId, string videoTitle = null)
        {
            return await _youtubeService.GetMP4File(videoId, videoTitle);
        }
    }

    public static class YoutubeDownloaderHelpers
    {
        public static IYoutubeDownloader GetYoutubeDownloader(YoutubeFileType fileType, IYoutubeService youtubeService)
        {
            return fileType switch
            {
                YoutubeFileType.MP4 => new MP4YoutubeDownloader(youtubeService),
                _ => new MP3YoutubeDownloader(youtubeService),
            };
        }
    }
}
