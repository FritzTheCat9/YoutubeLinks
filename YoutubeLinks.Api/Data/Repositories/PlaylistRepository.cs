using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Repositories;

public interface IPlaylistRepository
{
    IQueryable<Playlist> AsQueryable(int userId, bool loadPrivatePlaylists = false);
    IQueryable<Playlist> AsQueryablePublic();
    Task<IEnumerable<Playlist>> GetAll();
    Task<IEnumerable<Playlist>> GetAllPublic();
    Task<Playlist> Get(int id);
    Task<bool> LinkUrlExists(string url, int playlistId);
    Task<bool> LinkUrlExistsInOtherLinksThan(string url, int playlistId, int id);
    Task<int> Create(Playlist playlist);
    Task Update(Playlist playlist);
    Task SetLinksDownloadedFlag(Playlist playlist, bool flag);
    Task Delete(Playlist playlist);
}

public class PlaylistRepository(AppDbContext dbContext) : IPlaylistRepository
{
    public IQueryable<Playlist> AsQueryable(int userId, bool loadPrivate = false)
    {
        var query = dbContext.Playlists.Include(x => x.Links)
            .Include(x => x.User)
            .Where(x => x.UserId == userId);

        if (!loadPrivate)
            query = query.Where(x => x.Public);

        return query.AsSplitQuery()
            .AsQueryable();
    }

    public IQueryable<Playlist> AsQueryablePublic()
        => dbContext.Playlists
            .Include(x => x.Links)
            .Include(x => x.User)
            .Where(x => x.Public)
            .AsSplitQuery()
            .AsQueryable();

    public async Task<IEnumerable<Playlist>> GetAll()
        => await dbContext.Playlists
            .Include(x => x.Links)
            .Include(x => x.User)
            .AsSplitQuery()
            .ToListAsync();

    public async Task<IEnumerable<Playlist>> GetAllPublic()
        => await dbContext.Playlists
            .Include(x => x.Links)
            .Include(x => x.User)
            .Where(x => x.Public)
            .AsSplitQuery()
            .ToListAsync();

    public async Task<Playlist> Get(int id)
        => await dbContext.Playlists
            .Include(x => x.Links)
            .Include(x => x.User)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool> LinkUrlExists(string url, int playlistId)
        => await dbContext.Links
            .AnyAsync(x => x.Url == url
                           && x.PlaylistId == playlistId);

    public async Task<bool> LinkUrlExistsInOtherLinksThan(string url, int playlistId, int id)
        => await dbContext.Links
            .Where(x => x.Id != id)
            .AnyAsync(x => x.Url == url
                           && x.PlaylistId == playlistId);

    public async Task<int> Create(Playlist playlist)
    {
        await dbContext.AddAsync(playlist);
        await dbContext.SaveChangesAsync();
        return playlist.Id;
    }

    public Task Update(Playlist playlist)
    {
        dbContext.Update(playlist);
        return Task.CompletedTask;
    }

    public Task SetLinksDownloadedFlag(Playlist playlist, bool flag)
    {
        playlist.Links.ForEach(x => x.Downloaded = flag);
        dbContext.Update(playlist);
        return Task.CompletedTask;
    }

    public Task Delete(Playlist playlist)
    {
        dbContext.Remove(playlist);
        return Task.CompletedTask;
    }
}