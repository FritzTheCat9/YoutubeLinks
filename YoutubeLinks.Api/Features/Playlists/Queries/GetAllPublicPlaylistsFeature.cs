using MediatR;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Extensions;
using YoutubeLinks.Api.Features.Playlists.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Api.Features.Playlists.Queries;

public static class GetAllPublicPlaylistsFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/playlists/allPublic", async (
                GetAllPublicPlaylists.Query query,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
            .WithTags(Tags.Playlists)
            .AllowAnonymous();
    }

    public class Handler(IPlaylistRepository playlistRepository)
        : IRequestHandler<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>
    {
        public Task<PagedList<PlaylistDto>> Handle(
            GetAllPublicPlaylists.Query query,
            CancellationToken cancellationToken)
        {

            var playlistPageList = playlistRepository.GetAllPublicPlaylistsPaginated(query);

            var playlistsDtoPageList = PageListExtensions<PlaylistDto>.Convert(playlistPageList, PlaylistExtensions.ToDto);

            return Task.FromResult(playlistsDtoPageList);
        }
    }
}