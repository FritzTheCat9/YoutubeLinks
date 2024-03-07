using MediatR;
using Microsoft.Extensions.Localization;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Localization;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
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
            private readonly IClock _clock;
            private readonly IStringLocalizer<ApiValidationMessage> _localizer;

            public Handler(
                IPlaylistRepository playlistRepository,
                ILinkRepository linkRepository,
                IAuthService authService,
                IClock clock,
                IStringLocalizer<ApiValidationMessage> localizer)
            {
                _playlistRepository = playlistRepository;
                _linkRepository = linkRepository;
                _authService = authService;
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

                var urlExists = await _playlistRepository.LinkUrlExistsInOtherLinksThan(command.Url, link.PlaylistId, command.Id);
                if (urlExists)
                    throw new MyValidationException(nameof(CreateLink.Command.Url),
                                                    _localizer[nameof(ApiValidationMessageString.UrlMustBeUnique)]);

                link.Modified = _clock.Current();
                link.Url = command.Url;              // only video id is important, we can truncate some url (channel etc.)
                link.Title = "";                     // read yt title
                link.Downloaded = command.Downloaded;

                await _linkRepository.Update(link);
                return Unit.Value;
            }
        }
    }
}
