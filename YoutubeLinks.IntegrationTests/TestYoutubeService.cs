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
            "dQw4w9WgXcQ" => "Rick Astley - Never Gonna Give You Up (Official Music Video)",
            "GtUVQei3nX4" => "Snoop Dogg - Drop It Like It's Hot (Official Music Video) ft. Pharrell Williams",
            "u15tEo0wsQI" => "Dawid Podsiadło, P.T. Adamczyk — Phantom Liberty (Official Cyberpunk 2077 Music Video)",
            _ => "Test Video"
        };

        return Task.FromResult(YoutubeHelpers.NormalizeVideoTitle(title));
    }

    public Task<YoutubeFile> GetMp3File(string videoId, string videoTitle = null)
    {
        var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp3");
        File.WriteAllText(tmp, "dummy mp3 content");

        var youtubeFile = new YoutubeFile
        {
            FilePath = tmp,
            ContentType = "audio/mpeg",
            FileName = Path.GetFileName(tmp),
            YoutubeFileType = YoutubeFileType.Mp3
        };

        return Task.FromResult(youtubeFile);
    }

    public Task<YoutubeFile> GetMp4File(string videoId, string videoTitle = null)
    {
        var tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.mp4");
        File.WriteAllText(tmp, "dummy mp4 content");

        var youtubeFile = new YoutubeFile
        {
            FilePath = tmp,
            ContentType = "video/mp4",
            FileName = Path.GetFileName(tmp),
            YoutubeFileType = YoutubeFileType.Mp4
        };

        return Task.FromResult(youtubeFile);
    }
}
