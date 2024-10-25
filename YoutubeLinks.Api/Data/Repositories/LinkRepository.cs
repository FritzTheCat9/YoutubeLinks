using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Repositories;

public interface ILinkRepository
{
    IQueryable<Link> AsQueryable(int playlistId, bool loadPrivate = false);
    Task<IEnumerable<Link>> GetAll();
    Task<Link> Get(int id);
    Task<int> Create(Link link);
    Task Update(Link link);
    Task Delete(Link link);
    Task SaveChanges();
}

public class LinkRepository(AppDbContext dbContext) : ILinkRepository
{
    public IQueryable<Link> AsQueryable(int playlistId, bool loadPrivate = false)
    {
        var query = dbContext.Links.Include(x => x.Playlist)
            .Where(x => x.PlaylistId == playlistId);

        if (!loadPrivate)
            query = query.Where(x => x.Playlist.Public);

        return query.AsSplitQuery()
            .AsQueryable();
    }

    public async Task<IEnumerable<Link>> GetAll()
    {
        return await dbContext.Links
            .Include(x => x.Playlist)
            .ToListAsync();
    }

    public async Task<Link> Get(int id)
    {
        return await dbContext.Links
            .Include(x => x.Playlist)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<int> Create(Link link)
    {
        await dbContext.AddAsync(link);
        await dbContext.SaveChangesAsync();
        return link.Id;
    }

    public Task Update(Link link)
    {
        dbContext.Update(link);
        return Task.CompletedTask;
    }

    public Task Delete(Link link)
    {
        dbContext.Remove(link);
        return Task.CompletedTask;
    }

    public async Task SaveChanges()
    {
        await dbContext.SaveChangesAsync();
    }
}