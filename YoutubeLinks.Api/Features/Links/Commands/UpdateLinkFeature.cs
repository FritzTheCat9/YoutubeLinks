using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Abstractions;
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
        ILinkRepository linkRepository,
        IAuthService authService,
        IYoutubeService youtubeService,
        IClock clock,
        IStringLocalizer<ApiValidationMessage> localizer)
        : IRequestHandler<UpdateLink.Command, Unit>
    {
        public async Task<Unit> Handle(
            UpdateLink.Command command,
            CancellationToken cancellationToken)
        {
            var link = await linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

            if (!authService.IsLoggedInUser(link.Playlist.UserId))
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

            var urlExists =
                await playlistRepository.LinkUrlExistsInOtherLinksThan(command.Url, link.PlaylistId, command.Id);
            if (urlExists)
            {
                throw new MyValidationException(nameof(CreateLink.Command.Url),
                    localizer[nameof(ApiValidationMessageString.UrlMustBeUnique)]);
            }

            if (string.IsNullOrWhiteSpace(link.Title) || command.Url != link.Url)
            {
                link.Title = await youtubeService.GetVideoTitle(videoId);
            }
            else
            {
                link.Title = YoutubeHelpers.NormalizeVideoTitle(command.Title);
            }

            link.Modified = clock.Current();
            link.Url = command.Url;
            link.VideoId = videoId;
            link.Downloaded = command.Downloaded;

            await linkRepository.Update(link);
            return Unit.Value;
        }
    }
}