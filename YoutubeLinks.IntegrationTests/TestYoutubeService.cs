using System.IO;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.IntegrationTests;

public class TestYoutubeService : IYoutubeService
{
    public Task<string> GetVideoTitle(string videoId)
    {
        var title = videoId switch
        {
            "dQw4w9WgXcQ" => "Rick Astley - Never Gonna Give You Up (Official Music Video) (4K Remaster)",
            "GtUVQei3nX4" => "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams",
            "u15tEo0wsQI" => "Dawid Podsiadło, P.T. Adamczyk — Phantom Liberty (Official Cyberpunk 2077 Music Video)",
            _ => "Test Video"
        };

        return Task.FromResult(YoutubeHelpers.NormalizeVideoTitle(title));
    }

    public async Task<YoutubeFile> GetMp3File(string videoId, string videoTitle = null)
    {
        var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
        File.WriteAllText(tmp, "dummy mp3 content");
        var title = await GetVideoTitle(videoId);

        var linkTitle = YoutubeHelpers.NormalizeVideoTitle(title);
        var youtubeFile = new YoutubeFile
        {
            FilePath = tmp,
            ContentType = "audio/mpeg",
            FileName = $"{linkTitle}.{YoutubeHelpers.YoutubeFileTypeToString(YoutubeFileType.Mp3)}",
            YoutubeFileType = YoutubeFileType.Mp3
        };

        return youtubeFile;
    }

    public async Task<YoutubeFile> GetMp4File(string videoId, string videoTitle = null)
    {
        var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");
        File.WriteAllText(tmp, "dummy mp4 content");
        var title = await GetVideoTitle(videoId);

        var linkTitle = YoutubeHelpers.NormalizeVideoTitle(title);
        var youtubeFile = new YoutubeFile
        {
            FilePath = tmp,
            ContentType = "video/mp4",
            FileName = $"{linkTitle}.{YoutubeHelpers.YoutubeFileTypeToString(YoutubeFileType.Mp4)}",
            YoutubeFileType = YoutubeFileType.Mp4
        };

        return youtubeFile;
    }
}
