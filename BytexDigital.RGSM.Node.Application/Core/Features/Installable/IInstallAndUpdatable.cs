using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.Status;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Installable
{
    public interface IInstallAndUpdatable
    {
        Task<ServerInstallationStatus> GetInstallationStatusAsync(CancellationToken cancellationToken = default);

        Task<CanResult> CanInstallOrUpdateAsync(CancellationToken cancellationToken = default);
        Task BeginInstallationOrUpdateAsync(CancellationToken cancellationToken = default);
        Task CancelInstallationOrUpdateAsync(CancellationToken cancellationToken = default);
    }
}
