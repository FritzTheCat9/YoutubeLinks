using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Repositories
{
    public interface IPlaylistWriteRepository
    {
        Task<Playlist> GetByIdAsync(int id);
        Task AddAsync(Playlist entity);
        void Update(Playlist entity);
        void Remove(Playlist entity);
    }

    public class PlaylistWriteRepository(AppDbContext context) : IPlaylistWriteRepository
    {
        public async Task AddAsync(Playlist entity) => await context.Playlists.AddAsync(entity);

        public async Task<Playlist> GetByIdAsync(int id) =>
            await context.Playlists.Include(p => p.Links).FirstOrDefaultAsync(x => x.Id == id);

        public void Update(Playlist entity) => context.Playlists.Update(entity);

        public void Remove(Playlist entity) => context.Playlists.Remove(entity);
    }
}
