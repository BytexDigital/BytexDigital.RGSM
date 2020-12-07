using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.Status;

namespace BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces
{
    public interface IRunnable
    {
        Task<bool> CanStartAsync(CancellationToken cancellationToken = default);
        Task StartAsync(CancellationToken cancellationToken = default);

        Task<bool> CanStopAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);

        Task<bool> IsRunningAsync(CancellationToken cancellationToken = default);

        Task<ServerInstallationStatus> GetInstallationStatusAsync(CancellationToken cancellationToken = default);

        Task<bool> CanInstallOrUpdateAsync(CancellationToken cancellationToken = default);
        Task BeginInstallationOrUpdateAsync(CancellationToken cancellationToken = default);
        Task CancelInstallationOrUpdateAsync(CancellationToken cancellationToken = default);
    }
}
