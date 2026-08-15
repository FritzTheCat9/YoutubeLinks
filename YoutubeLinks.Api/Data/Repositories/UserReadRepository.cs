using Microsoft.EntityFrameworkCore;
using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Repositories
{
    public interface IUserReadRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUserNameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UserNameExistsAsync(string username);
    }

    public class UserReadRepository(AppDbContext context) : IUserReadRepository
    {
        public Task<User> GetByIdAsync(int id) =>
            context.Users.Include(u => u.Playlists).FirstOrDefaultAsync(x => x.Id == id);

        public Task<User> GetByEmailAsync(string email) =>
            context.Users.FirstOrDefaultAsync(x => x.Email == email);

        public Task<User> GetByUserNameAsync(string username) =>
            context.Users.FirstOrDefaultAsync(x => x.UserName == username);

        public Task<bool> EmailExistsAsync(string email) =>
            context.Users.AnyAsync(x => x.Email == email);

        public Task<bool> UserNameExistsAsync(string username) =>
            context.Users.AnyAsync(x => x.UserName == username);
    }
}
