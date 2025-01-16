using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Commands;
using YoutubeLinks.Shared.Features.Users.Helpers;

namespace YoutubeLinks.Api.Features.Links.Commands;

public static class DeleteLinkFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/links/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new DeleteLink.Command { Id = id };
                return Results.Ok(await mediator.Send(command, cancellationToken));
            })
            .WithTags(Tags.Links)
            .RequireAuthorization(Policy.User);
    }

    public class Handler(
        IAuthService authService,
        IPlaylistRepository playlistRepository)
        : IRequestHandler<DeleteLink.Command, Unit>
    {
        public async Task<Unit> Handle(
            DeleteLink.Command command,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.FindPlaylistContainingLink(command.Id) ??
                           throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!isUserPlaylist)
            {
                throw new MyForbiddenException();
            }

            playlist.RemoveLink(command.Id);
            await playlistRepository.Update(playlist);
            return Unit.Value;
        }
    }
}