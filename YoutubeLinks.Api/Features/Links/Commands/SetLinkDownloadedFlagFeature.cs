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
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<SetLinkDownloadedFlag.Command, Unit>
    {
        public async Task<Unit> Handle(
            SetLinkDownloadedFlag.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.FindPlaylistContainingLink(command.Id) ??
                           throw new MyNotFoundException();

            if (!authService.IsLoggedInUser(playlist.UserId))
            {
                throw new MyForbiddenException();
            }

            playlist.SetLinkDownloadedFlag(command.Id, command.Downloaded);

            await playlistRepository.Update(playlist);
            return Unit.Value;
        }
    }
}