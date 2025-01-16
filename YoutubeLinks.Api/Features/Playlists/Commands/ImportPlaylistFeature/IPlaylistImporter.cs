using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands.ImportPlaylistFeature;

public interface IPlaylistImporter
{
    Task Import(
        IYoutubeService youtubeService,
        IStringLocalizer<ApiValidationMessage> localizer,
        ImportPlaylist.Command command,
        Playlist playlist);
}

public class JsonPlaylistImporter : IPlaylistImporter
{
    public Task Import(
        IYoutubeService youtubeService,
        IStringLocalizer<ApiValidationMessage> localizer,
        ImportPlaylist.Command command,
        Playlist playlist)
    {
        foreach (var link in command.ExportedLinks)
        {
            playlist.AddLink(link.Url, link.VideoId, link.Title);
        }

        return Task.CompletedTask;
    }
}

public class TxtPlaylistImporter : IPlaylistImporter
{
    public async Task Import(
        IYoutubeService youtubeService,
        IStringLocalizer<ApiValidationMessage> localizer,
        ImportPlaylist.Command command,
        Playlist playlist)
    {
        foreach (var videoId in command.ExportedLinkUrls.Select(YoutubeHelpers.GetVideoId))
        {
            if (string.IsNullOrWhiteSpace(videoId))
            {
                throw new MyValidationException(nameof(CreateLink.Command.Url),
                    localizer[nameof(ApiValidationMessageString.UrlIdNotValid)]);
            }

            var url = $"{YoutubeHelpers.VideoPathBase}{videoId}";
            var videoTitle = await youtubeService.GetVideoTitle(videoId);

            playlist.AddLink(url, videoId, videoTitle);
        }
    }
}

public static class PlaylistImporterHelpers
{
    public static IPlaylistImporter GetImporter(PlaylistFileType fileType)
    {
        return fileType switch
        {
            PlaylistFileType.Txt => new TxtPlaylistImporter(),
            _ => new JsonPlaylistImporter()
        };
    }
}