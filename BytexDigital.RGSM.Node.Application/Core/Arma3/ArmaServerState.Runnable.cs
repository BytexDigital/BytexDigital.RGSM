using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.Runnable;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public partial class ArmaServerState : IRunnable
    {
        public async Task<CanResult> CanStartAsync(CancellationToken cancellationToken = default)
        {
            if (await ProcessMonitor.IsRunningAsync()) return CanResult.CannotBecause("Server is running.");
            if (!Settings.IsInstalled) return CanResult.CannotBecause("Server is not installed.");
            if (IsUpdating) return CanResult.CannotBecause("Server is updating.");
            if (IsUpdatingWorkshopMods) return CanResult.CannotBecause("Server is updating workshop mods.");

            return CanResult.Can();
        }

        public async Task<CanResult> CanStopAsync(CancellationToken cancellationToken = default)
        {
            if (!await ProcessMonitor.IsRunningAsync()) return CanResult.CannotBecause("Server is not running.");
            if (!Settings.IsInstalled) return CanResult.CannotBecause("Server is not installed.");

            return CanResult.Can();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await ReconfigureAsync();
                await ProcessMonitor.RunAsync();

                Logger.Information($"Started server.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Starting server failed.");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await ProcessMonitor.ShutdownAsync(cancellationToken);

                Logger.Information($"Stopped server.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Stopping server failed.");
                throw;
            }
        }

        public async Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
        {
            return await ProcessMonitor.IsRunningAsync();
        }
    }
}
