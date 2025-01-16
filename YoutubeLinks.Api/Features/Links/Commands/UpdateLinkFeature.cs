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

public static class UpdateLinkFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/links/{id:int}", async (
                int id,
                UpdateLink.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
            .WithTags(Tags.Links)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService,
        IYoutubeService youtubeService,
        IStringLocalizer<ApiValidationMessage> localizer)
        : IRequestHandler<UpdateLink.Command, Unit>
    {
        public async Task<Unit> Handle(
            UpdateLink.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.FindPlaylistContainingLink(command.Id) ??
                           throw new MyNotFoundException();

            if (!authService.IsLoggedInUser(playlist.UserId))
            {
                throw new MyForbiddenException();
            }

            var videoId = YoutubeHelpers.GetVideoId(command.Url);
            if (string.IsNullOrWhiteSpace(videoId))
            {
                throw new MyValidationException(nameof(CreateLink.Command.Url),
                    localizer[nameof(ApiValidationMessageString.UrlIdNotValid)]);
            }

            command.Url = $"{YoutubeHelpers.VideoPathBase}{videoId}";

            var urlExists = playlist.LinkUrlExistsInOtherLinksThan(command.Url, command.Id);
            if (urlExists)
            {
                throw new MyValidationException(nameof(CreateLink.Command.Url),
                    localizer[nameof(ApiValidationMessageString.UrlMustBeUnique)]);
            }

            var link = playlist.GetLink(command.Id);
            
            if (string.IsNullOrWhiteSpace(link.Title) || command.Url != link.Url)
            {
                link.SetTitle(await youtubeService.GetVideoTitle(videoId));
            }
            else
            {
                link.SetTitle(YoutubeHelpers.NormalizeVideoTitle(command.Title));
            }

            link.SetUrl(command.Url);
            link.SetVideoId(videoId);
            link.SetDownloaded(command.Downloaded);

            await playlistRepository.Update(playlist);
            return Unit.Value;
        }
    }
}