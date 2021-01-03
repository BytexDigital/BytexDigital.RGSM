using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.ServerLogs;

namespace BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs
{
    public interface IServerLogs
    {
        Task<List<LogSource>> GetLogSourcesAsync(CancellationToken cancellationToken = default);
        Task<LogSource> GetPrimaryLogSourceOrDefaultAsync(CancellationToken cancellationToken = default);
        Task<LogContent> GetLogContentOrDefaultAsync(string logSourceName, int limitLines = default, CancellationToken cancellationToken = default);
    }
}
