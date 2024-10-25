using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services;

public interface IYoutubeDownloader
{
    Task<YoutubeFile> Download(string videoId, string videoTitle = null);
}

public class Mp3YoutubeDownloader(IYoutubeService youtubeService) : IYoutubeDownloader
{
    public async Task<YoutubeFile> Download(string videoId, string videoTitle = null)
    {
        return await youtubeService.GetMp3File(videoId, videoTitle);
    }
}

public class Mp4YoutubeDownloader(IYoutubeService youtubeService) : IYoutubeDownloader
{
    public async Task<YoutubeFile> Download(string videoId, string videoTitle = null)
    {
        return await youtubeService.GetMp4File(videoId, videoTitle);
    }
}

public static class YoutubeDownloaderHelpers
{
    public static IYoutubeDownloader GetYoutubeDownloader(YoutubeFileType fileType, IYoutubeService youtubeService)
    {
        return fileType switch
        {
            YoutubeFileType.Mp4 => new Mp4YoutubeDownloader(youtubeService),
            _ => new Mp3YoutubeDownloader(youtubeService)
        };
    }
}