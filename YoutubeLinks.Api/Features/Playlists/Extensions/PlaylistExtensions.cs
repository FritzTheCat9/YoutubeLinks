using System.Linq.Expressions;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Api.Features.Playlists.Commands;
using YoutubeLinks.Api.Features.Playlists.Queries;
using YoutubeLinks.Shared.Abstractions;
using YoutubeLinks.Shared.Features.Playlists.Queries;
using YoutubeLinks.Shared.Features.Playlists.Responses;

namespace YoutubeLinks.Api.Features.Playlists.Extensions
{
    public static class PlaylistExtensions
    {
        public static IEndpointRouteBuilder AddPlaylistsEndpoints(this IEndpointRouteBuilder app)
        {
            CreatePlaylistFeature.Endpoint(app);
            DeletePlaylistFeature.Endpoint(app);
            ExportPlaylistToJsonFeature.Endpoint(app);
            ImportPlaylistFromJsonFeature.Endpoint(app);
            UpdatePlaylistFeature.Endpoint(app);
            GetAllPublicPlaylistsFeature.Endpoint(app);
            GetAllUserPlaylistsFeature.Endpoint(app);
            GetPlaylistFeature.Endpoint(app);

            return app;
        }

        public static PlaylistDto ToDto(this Playlist playlist)
        {
            return new()
            {
                Id = playlist.Id,
                Created = playlist.Created,
                Modified = playlist.Modified,
                Name = playlist.Name,
                Public = playlist.Public,
                UserId = playlist.UserId,
            };
        }

        /* GetAllPublicPlaylists.Query */

        public static IQueryable<Playlist> FilterPlaylists(
            this IQueryable<Playlist> playlists,
            GetAllPublicPlaylists.Query query)
        {
            var searchTerm = query.SearchTerm.ToLower().Trim();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                playlists = playlists.Where(x =>
                    x.Name.ToLower().Contains(searchTerm));

            return playlists;
        }

        public static IQueryable<Playlist> SortPlaylists(
            this IQueryable<Playlist> playlists,
            GetAllPublicPlaylists.Query query)
        {
            return query.SortOrder switch
            {
                SortOrder.Ascending => playlists.OrderBy(GetPlaylistSortProperty(query)),
                SortOrder.Descending => playlists.OrderByDescending(GetPlaylistSortProperty(query)),
                SortOrder.None => playlists.OrderBy(x => x.Name),
                _ => playlists.OrderBy(x => x.Name),
            };
        }

        private static Expression<Func<Playlist, object>> GetPlaylistSortProperty(GetAllPublicPlaylists.Query query)
        {
            return query.SortColumn.ToLowerInvariant() switch
            {
                "name" => playlist => playlist.Name,
                _ => playlist => playlist.Name,
            };
        }

        /* GetAllUserPlaylists.Query */

        public static IQueryable<Playlist> FilterPlaylists(
            this IQueryable<Playlist> playlists,
            GetAllUserPlaylists.Query query)
        {
            var searchTerm = query.SearchTerm.ToLower().Trim();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                playlists = playlists.Where(x =>
                    x.Name.ToLower().Contains(searchTerm));

            return playlists;
        }

        public static IQueryable<Playlist> SortPlaylists(
            this IQueryable<Playlist> playlists,
            GetAllUserPlaylists.Query query)
        {
            return query.SortOrder switch
            {
                SortOrder.Ascending => playlists.OrderBy(GetPlaylistSortProperty(query)),
                SortOrder.Descending => playlists.OrderByDescending(GetPlaylistSortProperty(query)),
                SortOrder.None => playlists.OrderBy(x => x.Name),
                _ => playlists.OrderBy(x => x.Name),
            };
        }

        private static Expression<Func<Playlist, object>> GetPlaylistSortProperty(GetAllUserPlaylists.Query query)
        {
            return query.SortColumn.ToLowerInvariant() switch
            {
                "name" => playlist => playlist.Name,
                _ => playlist => playlist.Name,
            };
        }
    }
}
