using YoutubeLinks.Api.Data.Database;
using YoutubeLinks.Api.Data.Entities;

namespace YoutubeLinks.Api.Data.Repositories
{
    public interface IUserWriteRepository
    {
        Task AddAsync(User user);
        void Update(User user);
        void Remove(User user);
    }

    public class UserWriteRepository(AppDbContext context) : IUserWriteRepository
    {
        public async Task AddAsync(User user) => await context.Users.AddAsync(user);
        public void Update(User user) => context.Users.Update(user);
        public void Remove(User user) => context.Users.Remove(user);
    }
}
