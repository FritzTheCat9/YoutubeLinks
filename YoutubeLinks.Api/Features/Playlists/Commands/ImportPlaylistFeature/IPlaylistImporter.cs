using Microsoft.Extensions.Localization;
using System.Text;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Playlists.Commands;
using YoutubeLinks.Shared.Features.Playlists.Helpers;

namespace YoutubeLinks.Api.Features.Playlists.Commands.ImportPlaylistFeature
{
    public interface IPlaylistImporter
    {
        Task<IEnumerable<Link>> Import(
            IYoutubeService youtubeService,
            IClock clock,
            IStringLocalizer<ApiValidationMessage> localizer,
            ImportPlaylist.Command command);
    }

    public class JSONPlaylistImporter : IPlaylistImporter
    {
        public async Task<IEnumerable<Link>> Import(
            IYoutubeService youtubeService,
            IClock clock,
            IStringLocalizer<ApiValidationMessage> localizer,
            ImportPlaylist.Command command)
        {
            var links = new List<Link>();

            foreach (var exportedLink in command.ExportedLinks)
            {
                var link = new Link
                {
                    Id = 0,
                    Created = clock.Current(),
                    Modified = clock.Current(),
                    Url = exportedLink.Url,
                    VideoId = exportedLink.VideoId,
                    Title = exportedLink.Title,
                    Downloaded = false,
                };

                links.Add(link);
            }

            return await Task.FromResult(links);
        }
    }

    public class TXTPlaylistImporter : IPlaylistImporter
    {
        public async Task<IEnumerable<Link>> Import(
            IYoutubeService youtubeService,
            IClock clock,
            IStringLocalizer<ApiValidationMessage> localizer,
            ImportPlaylist.Command command)
        {
            var links = new List<Link>();

            foreach (var exportedUrl in command.ExportedLinkUrls)
            {
                var videoId = YoutubeHelpers.GetVideoId(exportedUrl);
                if (string.IsNullOrWhiteSpace(videoId))
                    throw new MyValidationException(nameof(CreateLink.Command.Url),
                                                    localizer[nameof(ApiValidationMessageString.UrlIdNotValid)]);

                var url = $"{YoutubeHelpers.VideoPathBase}{videoId}";
                var videoTitle = await youtubeService.GetVideoTitle(videoId);

                var link = new Link
                {
                    Id = 0,
                    Created = clock.Current(),
                    Modified = clock.Current(),
                    Url = url,
                    VideoId = videoId,
                    Title = videoTitle,
                    Downloaded = false,
                };

                links.Add(link);
            }

            return links;
        }
    }

    public static class PlaylistImporterHelpers
    {
        public static IPlaylistImporter GetImporter(PlaylistFileType fileType)
        {
            return fileType switch
            {
                PlaylistFileType.TXT => new TXTPlaylistImporter(),
                _ => new JSONPlaylistImporter(),
            };
        }
    }
}
