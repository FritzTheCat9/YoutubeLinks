using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Links.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using static YoutubeLinks.Shared.Features.Links.Queries.GetAllLinks;

namespace YoutubeLinks.Api.Features.Links.Queries
{
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

        public class Handler : IRequestHandler<Query, IEnumerable<LinkInfoDto>>
        {
            private readonly ILinkRepository _linkRepository;
            private readonly IPlaylistRepository _playlistRepository;
            private readonly IAuthService _authService;

            public Handler(
                ILinkRepository linkRepository,
                IPlaylistRepository playlistRepository,
                IAuthService authService)
            {
                _linkRepository = linkRepository;
                _playlistRepository = playlistRepository;
                _authService = authService;
            }

            public async Task<IEnumerable<LinkInfoDto>> Handle(
                Query query,
                CancellationToken cancellationToken)
            {
                var playlist = await _playlistRepository.Get(query.PlaylistId) ?? throw new MyNotFoundException();

                var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
                var linkQuery = _linkRepository.AsQueryable(query.PlaylistId, isUserPlaylist);

                if (isUserPlaylist)
                    linkQuery = linkQuery.FilterDownloaded(query);

                linkQuery = linkQuery.SortLinks();

                var linkInfoDtos = linkQuery.ToLinkInfoDtos();

                return linkInfoDtos;
            }
        }
    }
}
