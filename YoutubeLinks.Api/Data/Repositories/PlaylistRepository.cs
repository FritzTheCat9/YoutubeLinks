using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;

namespace YoutubeLinks.Api.Data.Repositories;

public interface IPlaylistRepository : IRepository<Playlist>
{
    PagedList<Playlist> GetAllPaginated(QueryParameters query);
    PagedList<Playlist> GetAllUserPlaylistsPaginated(QueryParameters query, int userId, bool loadPrivatePlaylists = false);
    PagedList<Playlist> GetAllPublicPlaylistsPaginated(QueryParameters query);
    IQueryable<Link> GetPlaylistLinksAsQueryable(int playlistId, bool loadPrivate = false); // TODO: move pagination to repository
    Task<Playlist> FindPlaylistContainingLink(int linkId);
    Task<IEnumerable<Playlist>> GetAll();
    Task<IEnumerable<Playlist>> GetAllPublic();
    Task<Playlist> Get(int id);
    Task<int> Create(Playlist playlist);
    Task Update(Playlist playlist);
    Task SetLinksDownloadedFlag(Playlist playlist, bool flag);
    Task Delete(Playlist playlist);
}

public class PlaylistRepository(
    AppDbContext dbContext
) : IPlaylistRepository
{
    private IQueryable<Playlist> LoadPlaylists()
    {
        return dbContext.Playlists
            .Include(x => x.Links)
            .Include(x => x.User)
            .AsQueryable();
    }

    private IQueryable<Playlist> Filter(
        IQueryable<Playlist> playlists,
        QueryParameters query)
    {
        var searchTerm = query.SearchTerm?.ToLower()?.Trim();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            playlists = playlists.Where(x =>
                x.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        return playlists;
    }

    private IQueryable<Playlist> Sort(
        IQueryable<Playlist> playlists,
        QueryParameters query)
    {
        return query.SortOrder switch
        {
            SortOrder.Ascending => playlists.OrderBy(GetPlaylistSortProperty(query)),
            SortOrder.Descending => playlists.OrderByDescending(GetPlaylistSortProperty(query)),
            SortOrder.None => playlists.OrderBy(x => x.Name),
            _ => playlists.OrderBy(x => x.Name)
        };
    }

    private Expression<Func<Playlist, object>> GetPlaylistSortProperty(QueryParameters query)
    {
        return query.SortColumn.ToLowerInvariant() switch
        {
            "name" => playlist => playlist.Name,
            _ => playlist => playlist.Name
        };
    }

    private (IQueryable<Playlist> query, int totalCount) Paginate(
        IQueryable<Playlist> playlists,
        QueryParameters query)
    {
        var totalCount = playlists.Count();

        playlists = playlists
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        return (playlists, totalCount);
    }

    public PagedList<Playlist> GetAllPaginated(QueryParameters query)
    {
        var playlistsQuery = LoadPlaylists();

        playlistsQuery = Filter(playlistsQuery, query);
        playlistsQuery = Sort(playlistsQuery, query);
        (playlistsQuery, var totalCount) = Paginate(playlistsQuery, query);

        var playlists = playlistsQuery
            .ToList();

        return new PagedList<Playlist>(playlists, query.Page, query.PageSize, totalCount);
    }

    public PagedList<Playlist> GetAllUserPlaylistsPaginated(QueryParameters query, int userId, bool loadPrivate = false)
    {
        var playlistsQuery = LoadPlaylists()
            .Where(x => x.UserId == userId);

        if (!loadPrivate)
        {
            playlistsQuery = playlistsQuery.Where(x => x.Public);
        }

        playlistsQuery = Filter(playlistsQuery, query);
        playlistsQuery = Sort(playlistsQuery, query);
        (playlistsQuery, var totalCount) = Paginate(playlistsQuery, query);

        var playlists = playlistsQuery
            .ToList();

        return new PagedList<Playlist>(playlists, query.Page, query.PageSize, totalCount);
    }

    public PagedList<Playlist> GetAllPublicPlaylistsPaginated(QueryParameters query)
    {
        var playlistsQuery = LoadPlaylists()
            .Where(x => x.Public);

        playlistsQuery = Filter(playlistsQuery, query);
        playlistsQuery = Sort(playlistsQuery, query);
        (playlistsQuery, var totalCount) = Paginate(playlistsQuery, query);

        var playlists = playlistsQuery
            .ToList();

        return new PagedList<Playlist>(playlists, query.Page, query.PageSize, totalCount);
    }

    public IQueryable<Link> GetPlaylistLinksAsQueryable(int playlistId, bool loadPrivate = false)
    {
        var query = dbContext.Links
            .Include(x => x.Playlist)
            .Where(x => x.PlaylistId == playlistId);

        if (!loadPrivate)
        {
            query = query.Where(x => x.Playlist.Public);
        }

        return query
            .AsSplitQuery()
            .AsQueryable();
    }

    public async Task<Playlist> FindPlaylistContainingLink(int linkId)
    {
        var playlist = await LoadPlaylists()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Links.Any(link => link.Id == linkId));

        return playlist;
    }

    public async Task<IEnumerable<Playlist>> GetAll()
    {
        var playlists = await LoadPlaylists()
            .AsSplitQuery()
            .ToListAsync();

        return playlists;
    }

    public async Task<IEnumerable<Playlist>> GetAllPublic()
    {
        var playlists = await LoadPlaylists()
            .Where(x => x.Public)
            .AsSplitQuery()
            .ToListAsync();

        return playlists;
    }

    public async Task<Playlist> Get(int id)
    {
        var playlist = await LoadPlaylists()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id);

        return playlist;
    }

    

    public async Task<int> Create(Playlist playlist)
    {
        await dbContext.AddAsync(playlist);
        await dbContext.SaveChangesAsync();
        return playlist.Id;
    }

    public Task Update(Playlist playlist)
    {
        dbContext.Update(playlist);
        dbContext.SaveChanges();
        return Task.CompletedTask;
    }

    public Task SetLinksDownloadedFlag(Playlist playlist, bool flag)
    {
        playlist.SetLinksDownloadedFlag(flag);

        dbContext.Update(playlist);
        dbContext.SaveChanges();
        return Task.CompletedTask;
    }

    public Task Delete(Playlist playlist)
    {
        dbContext.Remove(playlist);
        return Task.CompletedTask;
    }
}