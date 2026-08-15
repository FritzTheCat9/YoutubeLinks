using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;

namespace YoutubeLinks.Api.Data.Repositories
{
    public interface IPlaylistReadRepository
    {
        Task<Playlist> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<PagedList<Playlist>> GetPublicPaginated(QueryParameters query);
        Task<PagedList<Playlist>> GetUserPlaylistsPaginated(QueryParameters query, int userId, bool includePrivate = false);
    }

    public class PlaylistReadRepository(AppDbContext context) : IPlaylistReadRepository
    {
        public async Task<Playlist> GetByIdAsync(int id) =>
            await context.Playlists
                .Include(p => p.Links)
                .Include(p => p.User)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> ExistsAsync(int id) =>
            context.Playlists.AnyAsync(x => x.Id == id);

        public async Task<PagedList<Playlist>> GetPublicPaginated(QueryParameters query)
        {
            var data = context.Playlists.Where(x => x.IsPublic);
            return await BuildPagedList(data, query);
        }

        public async Task<PagedList<Playlist>> GetUserPlaylistsPaginated(QueryParameters query, int userId, bool includePrivate = false)
        {
            var data = context.Playlists.Where(x => x.UserId == userId);

            if (!includePrivate)
                data = data.Where(x => x.IsPublic);

            return await BuildPagedList(data, query);
        }

        private static async Task<PagedList<Playlist>> BuildPagedList(IQueryable<Playlist> query, QueryParameters q)
        {
            var total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.PageSize).Take(q.PageSize).ToListAsync();

            return new PagedList<Playlist>(items, q.Page, q.PageSize, total);
        }
    }
}
