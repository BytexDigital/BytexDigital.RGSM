using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.Installable;
using BytexDigital.RGSM.Node.Application.Core.Steam.Commands;
using BytexDigital.RGSM.Node.Domain.Models.Status;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public partial class ArmaServerState : IInstallAndUpdatable
    {
        public async Task<ServerInstallationStatus> GetInstallationStatusAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new ServerInstallationStatus
            {
                IsInstalled = Settings.IsInstalled,
                RequiresUpdate = false, // TODO: Make dynamic,
                IsUpdating = IsUpdating,
                UpdateProgress = ServerUpdateState?.Progress ?? 0,
                FailureReason = ServerUpdateState?.FailureException?.Message
            });
        }

        public async Task<CanResult> CanInstallOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            if (await IsRunningAsync(cancellationToken)) return CanResult.CannotBecause("Server is running.");
            if (IsUpdating) return CanResult.CannotBecause("Server is already updating.");
            if (IsUpdatingWorkshopMods) return CanResult.CannotBecause("Server is updating workshop mods.");

            return CanResult.Can();
        }

        public async Task BeginInstallationOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var appId = Settings.AppId.HasValue ? (uint)Settings.AppId.Value : DEDICATED_SERVER_APP_ID;

                ServerUpdateState = (await Mediator.Send(new UpdateAppCmd
                {
                    Id = Id,
                    AppId = appId,
                    UseAnonymousUser = false,
                    Directory = BaseDirectory,
                    UpdateState = ServerUpdateState,
                    Branch = Settings.Branch ?? "public",
                    BranchPassword = null,

                    DepotCondition = depot =>
                    {
                        return Settings.Depots.Contains(depot.Id);
                    }
                })).UpdateState;

                _ = Task.Run(async () =>
                {
                    await ServerUpdateState.ProcessedEvent.WaitAsync(cancellationToken);

                    if (ServerUpdateState.CancellationToken.IsCancellationRequested) return;
                    if (ServerUpdateState.FailureException != default) return;

                    Settings.IsInstalled = true;
                });

                Logger.Information($"Beginning installation/update of server.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Beginning installation/update of server failed.");
                throw;
            }
        }

        public Task CancelInstallationOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            ServerUpdateState?.CancellationToken.Cancel();
            Logger.Information($"Cancelled installation/update of server.");

            return Task.CompletedTask;
        }
    }
}
