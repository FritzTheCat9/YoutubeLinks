using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands
{
    public static class SetLinkDownloadedFlagFeature
    {
        public static void Endpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/links/{id:int}/downloaded", async (
                int id,
                SetLinkDownloadedFlag.Command command,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                command.Id = id;
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
                .WithTags(Tags.Links)
                .RequireAuthorization(Policy.User);
        }

        public class Handler : IRequestHandler<SetLinkDownloadedFlag.Command, Unit>
        {
            private readonly ILinkRepository _linkRepository;
            private readonly IAuthService _authService;
            private readonly IClock _clock;

            public Handler(
                ILinkRepository linkRepository,
                IAuthService authService,
                IClock clock)
            {
                _linkRepository = linkRepository;
                _authService = authService;
                _clock = clock;
            }

            public async Task<Unit> Handle(
                SetLinkDownloadedFlag.Command command,
                CancellationToken cancellationToken)
            {
                var link = await _linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

                if (!_authService.IsLoggedInUser(link.Playlist.UserId))
                    throw new MyForbiddenException();

                link.Modified = _clock.Current();
                link.Downloaded = command.Downloaded;

                await _linkRepository.Update(link);
                return Unit.Value;
            }
        }
    }
}
