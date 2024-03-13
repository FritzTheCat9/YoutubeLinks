using MediatR;
using YoutubeLinks.Api.Data.Database;

namespace YoutubeLinks.Api.Behaviors
{
    public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

        private static bool IsNotCommand
            => !typeof(TRequest).Name.EndsWith("Command");
        private static bool IsDownloadLinkCommand
            => typeof(TRequest).Name.EndsWith("DownloadLinkCommand");

        public UnitOfWorkBehavior(
            AppDbContext dbContext,
            ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (IsNotCommand || IsDownloadLinkCommand)
            {
                return await next();
            }

            _logger.LogInformation("[UnitOfWork] Begin Transaction");

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("[UnitOfWork] Commited Transaction");

                return response;
            }
            catch (Exception exception)
            {
                _logger.LogError("[UnitOfWork] Rollback Transaction {Exception}", exception);

                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
