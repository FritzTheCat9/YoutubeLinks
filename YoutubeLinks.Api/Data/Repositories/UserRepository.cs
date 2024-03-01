using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Repositories
{
    public interface IUserRepository
    {
        IQueryable<User> AsQueryable();
        Task<IEnumerable<User>> GetAll();
        Task<User> Get(int id);
        Task<User> GetByEmail(string email);
        Task<User> GetByUserName(string userName);
        Task<bool> EmailExists(string email);
        Task<bool> UserNameExists(string userName);
        Task<bool> IsTokenAssignedToUser(string email, string token);
        Task<int> Create(User user);
        Task Update(User user);
        Task Delete(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<User> AsQueryable()
            => _dbContext.Users.AsQueryable();

        public async Task<IEnumerable<User>> GetAll()
            => await _dbContext.Users.ToListAsync();

        public async Task<User> Get(int id)
            => await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<User> GetByEmail(string email)
            => await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        public async Task<User> GetByUserName(string userName)
            => await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == userName);

        public async Task<bool> EmailExists(string email)
            => await _dbContext.Users.AnyAsync(x => x.Email == email);

        public async Task<bool> UserNameExists(string userName)
            => await _dbContext.Users.AnyAsync(x => x.UserName == userName);

        public async Task<bool> IsTokenAssignedToUser(string email, string token)
            => await _dbContext.Users.AnyAsync(x => x.Email == email
                                                    && x.EmailConfirmationToken == token);

        public async Task<int> Create(User user)
        {
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public Task Update(User user)
        {
            _dbContext.Update(user);
            return Task.CompletedTask;
        }

        public Task Delete(User user)
        {
            _dbContext.Remove(user);
            return Task.CompletedTask;
        }
    }
}
