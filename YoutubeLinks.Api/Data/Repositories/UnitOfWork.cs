using YoutubeLinks.Api.Data.Database;

namespace YoutubeLinks.Api.Data.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => context.SaveChangesAsync(cancellationToken);
    }
}
