using MediatR;
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
                    CancellationToken cancellationToken)
                => Results.Ok(await mediator.Send(query, cancellationToken)))
            .WithTags(Tags.Playlists)
            .AllowAnonymous();
    }

    public class Handler : IRequestHandler<GetAllPublicPlaylists.Query, PagedList<PlaylistDto>>
    {
        private readonly IPlaylistRepository _playlistRepository;

        public Handler(
            IPlaylistRepository playlistRepository)
        {
            _playlistRepository = playlistRepository;
        }

        public Task<PagedList<PlaylistDto>> Handle(
            GetAllPublicPlaylists.Query query,
            CancellationToken cancellationToken)
        {
            var playlistQuery = _playlistRepository.AsQueryablePublic();

            playlistQuery = playlistQuery.FilterPlaylists(query);
            playlistQuery = playlistQuery.SortPlaylists(query);

            var playlistsPagedList = PageListExtensions<PlaylistDto>.Create(playlistQuery.Select(x => x.ToDto()),
                query.Page,
                query.PageSize);

            return Task.FromResult(playlistsPagedList);
        }
    }
}