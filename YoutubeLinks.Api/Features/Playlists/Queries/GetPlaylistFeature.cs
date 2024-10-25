using MediatR;
using YoutubeLinks.Api.Auth;
using YoutubeLinks.Api.Data.Repositories;
using YoutubeLinks.Api.Features.Playlists.Extensions;
using YoutubeLinks.Api.Helpers;
using YoutubeLinks.Shared.Exceptions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Api.Features.Playlists.Queries;

public static class GetPlaylistFeature
{
    public static void Endpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/playlists/{id:int}", async (
                int id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var query = new GetPlaylist.Query { Id = id };
                return Results.Ok(await mediator.Send(query, cancellationToken));
            })
            .WithName("GetPlaylist")
            .WithTags(Tags.Playlists)
            .AllowAnonymous();
    }

    public class Handler : IRequestHandler<GetPlaylist.Query, PlaylistDto>
    {
        private readonly IAuthService _authService;
        private readonly IPlaylistRepository _playlistRepository;

        public Handler(IPlaylistRepository playlistRepository,
            IAuthService authService)
        {
            _playlistRepository = playlistRepository;
            _authService = authService;
        }

        public async Task<PlaylistDto> Handle(
            GetPlaylist.Query query,
            CancellationToken cancellationToken)
        {
            var playlist = await _playlistRepository.Get(query.Id) ?? throw new MyNotFoundException();

            var isUserPlaylist = _authService.IsLoggedInUser(playlist.UserId);
            if (!playlist.Public
                && !isUserPlaylist)
                throw new MyForbiddenException();

            return playlist.ToDto();
        }
    }
}