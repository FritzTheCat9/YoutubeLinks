using MediatR;
using YoutubeLinks.Api.Abstractions;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands;

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

    public class Handler(
        ILinkRepository linkRepository,
        IAuthService authService,
        IClock clock)
        : IRequestHandler<SetLinkDownloadedFlag.Command, Unit>
    {
        public async Task<Unit> Handle(
            SetLinkDownloadedFlag.Command command,
            CancellationToken cancellationToken)
        {
            var link = await linkRepository.Get(command.Id) ?? throw new MyNotFoundException();

            if (!authService.IsLoggedInUser(link.Playlist.UserId))
            {
                throw new MyForbiddenException();
            }

            link.Modified = clock.Current();
            link.Downloaded = command.Downloaded;

            await linkRepository.Update(link);
            return Unit.Value;
        }
    }
}