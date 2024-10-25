using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Helpers;

namespace YoutubeLinks.Api.Services;

public interface IYoutubeService
{
    Task<string> GetVideoTitle(string videoId);
    Task<YoutubeFile> GetMp3File(string videoId, string videoTitle = null);
    Task<YoutubeFile> GetMp4File(string videoId, string videoTitle = null);
}

public class YoutubeService : IYoutubeService
{
    private readonly string _ffmpegPath;
    private readonly string _tmpFolderPath;
    private readonly string _ytDlpPath;

    public YoutubeService(IWebHostEnvironment webHostEnvironment)
    {
        _ytDlpPath = Path.Combine(Path.GetFullPath(webHostEnvironment.ContentRootPath),
            OperatingSystem.IsLinux() ? "yt-dlp" : "yt-dlp.exe");
        _ffmpegPath = Path.Combine(Path.GetFullPath(webHostEnvironment.ContentRootPath),
            OperatingSystem.IsLinux() ? "ffmpeg" : "ffmpeg.exe");
        _tmpFolderPath = Path.Combine(Path.GetFullPath(webHostEnvironment.ContentRootPath), "Tmp");
    }

    public async Task<string> GetVideoTitle(string videoId)
    {
        var youtubeDl = new YoutubeDL
        {
            YoutubeDLPath = _ytDlpPath,
            FFmpegPath = _ffmpegPath
        };

        var videoDataRequest = await youtubeDl.RunVideoDataFetch($"{YoutubeHelpers.VideoPathBase}{videoId}");
        if (!videoDataRequest.Success)
            throw new MyServerException();

        var title = videoDataRequest.Data.Title;
        var normalizedTitle = YoutubeHelpers.NormalizeVideoTitle(title);

        return normalizedTitle;
    }

    public async Task<YoutubeFile> GetMp3File(string videoId, string videoTitle = null)
    {
        var title = videoTitle ?? await GetVideoTitle(videoId);
        var fileName = $"{Guid.NewGuid()}.mp3";

        var youtubeDl = new YoutubeDL
        {
            YoutubeDLPath = _ytDlpPath,
            FFmpegPath = _ffmpegPath,
            OutputFolder = _tmpFolderPath
        };

        var options = new OptionSet
        {
            ExtractAudio = true,
            AudioQuality = 0,
            AudioFormat = AudioConversionFormat.Mp3,
            Output = Path.Combine(_tmpFolderPath, fileName),
            EmbedThumbnail = true
        };

        var runResult =
            await youtubeDl.RunAudioDownload($"{YoutubeHelpers.VideoPathBase}{videoId}", overrideOptions: options);
        if (!runResult.Success)
            throw new MyServerException();

        var filePath = Path.Combine(_tmpFolderPath, fileName);

        if (!File.Exists(filePath))
            throw new MyServerException();

        var normalizedFileName = $"{title}.mp3";
        var normalizedFilePath = Path.Combine(_tmpFolderPath, normalizedFileName);

        if (File.Exists(normalizedFilePath))
            File.Delete(normalizedFilePath);

        File.Move(filePath, normalizedFilePath);

        var youtubeFile = new YoutubeFile
        {
            FilePath = normalizedFilePath,
            ContentType = "audio/mpeg",
            FileName = normalizedFileName,
            YoutubeFileType = YoutubeFileType.Mp3
        };

        return youtubeFile;
    }

    public async Task<YoutubeFile> GetMp4File(string videoId, string videoTitle = null)
    {
        var title = videoTitle ?? await GetVideoTitle(videoId);
        var fileName = $"{Guid.NewGuid()}.mp4";

        var youtubeDl = new YoutubeDL
        {
            YoutubeDLPath = _ytDlpPath,
            FFmpegPath = _ffmpegPath,
            OutputFolder = _tmpFolderPath
        };

        var options = new OptionSet
        {
            Output = Path.Combine(_tmpFolderPath, fileName),
            Format = "bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best",
            EmbedThumbnail = true
        };

        var runResult =
            await youtubeDl.RunVideoDownload($"{YoutubeHelpers.VideoPathBase}{videoId}", overrideOptions: options);
        if (!runResult.Success)
            throw new MyServerException();

        var filePath = Path.Combine(_tmpFolderPath, fileName);

        if (!File.Exists(filePath))
            throw new MyServerException();

        var normalizedFileName = $"{title}.mp4";
        var normalizedFilePath = Path.Combine(_tmpFolderPath, normalizedFileName);

        if (File.Exists(normalizedFilePath))
            File.Delete(normalizedFilePath);

        File.Move(filePath, normalizedFilePath);

        var youtubeFile = new YoutubeFile
        {
            FilePath = normalizedFilePath,
            ContentType = "video/mp4",
            FileName = normalizedFileName,
            YoutubeFileType = YoutubeFileType.Mp4
        };

        return youtubeFile;
    }
}