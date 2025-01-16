using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Links.Queries;
using YoutubeLinks.Shared.Features.Links.Responses;

namespace YoutubeLinks.Api.Features.Links.Queries;

public static class GetLinkFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/links/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetLink.Query { Id = id };
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
            .WithName("GetLink")
            .WithTags(Tags.Links)
            .AllowAnonymous();
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<GetLink.Query, LinkDto>
    {
        public async Task<LinkDto> Handle(
            GetLink.Query query,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.FindPlaylistContainingLink(query.Id) ??
                           throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            if (!playlist.Public
                && !isUserPlaylist)
            {
                throw new MyForbiddenException();
            }

            var link = playlist.GetLink(query.Id);

            return link.ToDto();
        }
    }
}