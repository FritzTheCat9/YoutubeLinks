using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;
using YoutubeLinks.Shared.Abstractions;

namespace YoutubeLinks.Api.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    PagedList<User> GetAllPaginated(QueryParameters query);
    Task<IEnumerable<User>> GetAll();
    Task<User> Get(int id);
    Task<User> GetByEmail(string email);
    Task<User> GetByUserName(string userName);
    Task<bool> EmailExists(string email);
    Task<bool> UserNameExists(string userName);
    Task<bool> IsEmailConfirmationTokenAssignedToUser(string email, string token);
    Task<bool> IsForgotPasswordTokenAssignedToUser(string email, string token);
    Task<int> Create(User user);
    Task Update(User user);
    Task Delete(User user);
}

public class UserRepository(
    AppDbContext dbContext
) : IUserRepository
{
    private IQueryable<User> LoadUsers()
    {
        return dbContext.Users
            .Include(x => x.Playlists)
            .AsQueryable();
    }

    private IQueryable<User> Filter(
        IQueryable<User> users,
        QueryParameters query)
    {
        var searchTerm = query.SearchTerm?.ToLower()?.Trim();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            users = users.Where(x =>
                x.UserName.ToLower().Contains(searchTerm.ToLower())
                || x.Email.ToLower().Contains(searchTerm.ToLower()));
        }

        return users;
    }

    private IQueryable<User> Sort(
        IQueryable<User> users,
        QueryParameters query)
    {
        return query.SortOrder switch
        {
            SortOrder.Ascending => users.OrderBy(GetUserSortProperty(query)),
            SortOrder.Descending => users.OrderByDescending(GetUserSortProperty(query)),
            SortOrder.None => users.OrderBy(x => x.UserName),
            _ => users.OrderBy(x => x.UserName)
        };
    }

    private Expression<Func<User, object>> GetUserSortProperty(QueryParameters query)
    {
        return query.SortColumn.ToLowerInvariant() switch
        {
            "username" => user => user.UserName,
            "email" => user => user.Email,
            _ => user => user.UserName
        };
    }

    private (IQueryable<User> query, int totalCount) Paginate(
        IQueryable<User> users,
        QueryParameters query)
    {
        var totalCount = users.Count();

        users = users
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        return (users, totalCount);
    }

    public PagedList<User> GetAllPaginated(QueryParameters query)
    {
        var usersQuery = LoadUsers();
        
        usersQuery = Filter(usersQuery, query);
        usersQuery = Sort(usersQuery, query);
        (usersQuery, var totalCount) = Paginate(usersQuery, query);

        var users = usersQuery
            .ToList();

        return new PagedList<User>(users, query.Page, query.PageSize, totalCount);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        var users = await LoadUsers()
            .ToListAsync();

        return users;
    }

    public async Task<User> Get(int id)
    {
        var user = await LoadUsers()
            .FirstOrDefaultAsync(x => x.Id == id);

        return user;
    }

    public async Task<User> GetByEmail(string email)
    {
        var user = await LoadUsers()
            .FirstOrDefaultAsync(x => x.Email == email);

        return user;
    }

    public async Task<User> GetByUserName(string userName)
    {
        var user = await LoadUsers()
            .FirstOrDefaultAsync(x => x.UserName == userName);

        return user;
    }

    public async Task<bool> EmailExists(string email)
    {
        return await dbContext.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<bool> UserNameExists(string userName)
    {
        return await dbContext.Users.AnyAsync(x => x.UserName == userName);
    }

    public async Task<bool> IsEmailConfirmationTokenAssignedToUser(string email, string token)
    {
        return await dbContext.Users.AnyAsync(x => x.Email == email
                                                   && x.EmailConfirmationToken == token);
    }

    public async Task<bool> IsForgotPasswordTokenAssignedToUser(string email, string token)
    {
        return await dbContext.Users.AnyAsync(x => x.Email == email
                                                   && x.ForgotPasswordToken == token);
    }

    public async Task<int> Create(User user)
    {
        await dbContext.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return user.Id;
    }

    public Task Update(User user)
    {
        dbContext.Update(user);
        return Task.CompletedTask;
    }

    public Task Delete(User user)
    {
        dbContext.Remove(user);
        return Task.CompletedTask;
    }
}