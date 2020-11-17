using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Persistence;

using MediatR;

namespace BytexDigital.RGSM.Application.Behaviors
{
    public class DbTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ApplicationDbContext _storage;

        public DbTransactionBehavior(ApplicationDbContext storage)
        {
            _storage = storage;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (_storage.Database.CurrentTransaction != null)
            {
                return await next();
            }
            else
            {
                using (var transaction = await _storage.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var result = await next();
                        await transaction.CommitAsync();

                        return result;

                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }
    }
}
