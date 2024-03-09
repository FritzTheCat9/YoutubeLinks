using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Api.Services;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Links.Helpers;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands
{
    public static class UpdateLinkFeature
    {
        public static IEndpointRouteBuilder Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/links/{id}", async (
                int id,
                UpdateLink.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
                .WithTags("Links")
                .RequireAuthorization(Policy.User);

            return app;
        }

        public class Handler : IRequestHandler<UpdateLink.Command, Unit>
        {
            private readonly IPlaylistRepository _playlistRepository;
            private readonly ILinkRepository _linkRepository;
            private readonly IAuthService _authService;
            private readonly IYoutubeService _youtubeService;
            private readonly IClock _clock;
            private readonly IStringLocalizer<ApiValidationMessage> _localizer;

            public Handler(
                IPlaylistRepository playlistRepository,
                ILinkRepository linkRepository,
                IAuthService authService,
                IYoutubeService youtubeService,
                IClock clock,
                IStringLocalizer<ApiValidationMessage> localizer)
            {
                _playlistRepository = playlistRepository;
                _linkRepository = linkRepository;
                _authService = authService;
                _youtubeService = youtubeService;
                _clock = clock;
                _localizer = localizer;
            }

            public async Task<Unit> Handle(
                UpdateLink.Command command,
                CancellationToken cancellationToken)
            {
                var link = await _linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

                if (!_authService.IsLoggedInUser(link.Playlist.UserId))
                    throw new MyForbiddenException();

                var videoId = YoutubeHelpers.GetVideoId(command.Url);
                if (string.IsNullOrWhiteSpace(videoId))
                    throw new MyValidationException(nameof(CreateLink.Command.Url),
                                                    _localizer[nameof(ApiValidationMessageString.UrlIdNotValid)]);

                command.Url = $"{YoutubeHelpers.VideoPathBase}{videoId}";

                var urlExists = await _playlistRepository.LinkUrlExistsInOtherLinksThan(command.Url, link.PlaylistId, command.Id);
                if (urlExists)
                    throw new MyValidationException(nameof(CreateLink.Command.Url),
                                                    _localizer[nameof(ApiValidationMessageString.UrlMustBeUnique)]);

                if (string.IsNullOrWhiteSpace(link.Title) || command.Url != link.Url)
                    link.Title = await _youtubeService.GetVideoTitle(videoId);

                link.Modified = _clock.Current();
                link.Url = command.Url;
                link.VideoId = videoId;
                link.Downloaded = command.Downloaded;

                await _linkRepository.Update(link);
                return Unit.Value;
            }
        }
    }
}
