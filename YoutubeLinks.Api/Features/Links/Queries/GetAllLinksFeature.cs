using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using static YoutubeLinks.Shared.Features.Links.Queries.GetAllLinks;

namespace YoutubeLinks.Api.Features.Links.Queries;

public static class GetAllLinksFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/links/all", async (
                Query query,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
            .WithTags(Tags.Links)
            .AllowAnonymous();
    }

    public class Handler(
        IPlaylistRepository playlistRepository,
        IAuthService authService)
        : IRequestHandler<Query, IEnumerable<LinkInfoDto>>
    {
        public async Task<IEnumerable<LinkInfoDto>> Handle(
            Query query,
            CancellationToken cancellationToken)
        {
            var playlist = await playlistRepository.Get(query.PlaylistId) ?? throw new MyNotFoundException();

            var isUserPlaylist = authService.IsLoggedInUser(playlist.UserId);
            var linkQuery = playlistRepository.GetPlaylistLinksAsQueryable(query.PlaylistId, isUserPlaylist);

            if (isUserPlaylist)
            {
                linkQuery = linkQuery.FilterDownloaded(query);
            }

            linkQuery = linkQuery.SortLinks();

            var linkInfoDtos = linkQuery.ToLinkInfoDtos();

            return linkInfoDtos;
        }
    }
}