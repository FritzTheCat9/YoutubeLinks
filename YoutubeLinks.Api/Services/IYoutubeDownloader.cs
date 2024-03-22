using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services
{
    public interface IYoutubeDownloader
    {
        Task<YoutubeFile> Download(string videoId);
    }

    public class MP3YoutubeDownloader : IYoutubeDownloader
    {
        private readonly IYoutubeService _youtubeService;

        public MP3YoutubeDownloader(
            IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        public async Task<YoutubeFile> Download(string videoId)
        {
            return await _youtubeService.GetMP3File(videoId);
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

        public async Task<YoutubeFile> Download(string videoId)
        {
            return await _youtubeService.GetMP4File(videoId);
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
