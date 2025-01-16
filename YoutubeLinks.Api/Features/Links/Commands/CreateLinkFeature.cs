using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands;

public static class CreateLinkFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/links", async (
                CreateLink.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var linkId = await mediator.Send(command, cancellationToken);
                return Results.CreatedAtRoute("GetLink", new { id = linkId }, linkId);
            })
            .WithTags(Tags.Links)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService,
        IYoutubeService youtubeService,
        IStringLocalizer<ApiValidationMessage> localizer)
        : IRequestHandler<CreateLink.Command, int>
    {
        public async Task<int> Handle(
            CreateLink.Command command,
            CancellationToken cancellationToken)
        {
            var videoId = YoutubeHelpers.GetVideoId(command.Url);
            if (string.IsNullOrWhiteSpace(videoId))
            {
                throw new MyValidationException(nameof(CreateLink.Command.Url),
                    localizer[nameof(ApiValidationMessageString.UrlIdNotValid)]);
            }

            command.Url = $"{YoutubeHelpers.VideoPathBase}{videoId}";
            var videoTitle = await youtubeService.GetVideoTitle(videoId);

            var playlist = await playlistRepository.Get(command.PlaylistId) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!isUserPlaylist)
            {
                throw new MyForbiddenException();
            }

            var urlExists = playlist.LinkUrlExists(command.Url);
            if (urlExists)
            {
                throw new MyValidationException(nameof(CreateLink.Command.Url),
                    localizer[nameof(ApiValidationMessageString.UrlMustBeUnique)]);
            }

            var link = playlist.AddLink(command.Url, videoId, videoTitle);

            await playlistRepository.Update(playlist);
            return link.Id;
        }
    }
}