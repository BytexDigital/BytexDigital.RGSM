using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Arma3.Commands;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Core.Generic;
using BytexDigital.RGSM.Node.Application.Core.SteamCmd;
using BytexDigital.RGSM.Node.Application.Core.SteamCmd.Commands;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Models.Status;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaServerState : ServerStateBase, IRunnable
    {
        public Arma3Server Settings { get; private set; }
        public ProcessMonitor ProcessMonitor { get; private set; }
        public UpdateState UpdateState { get; private set; }
        public bool IsUpdating => UpdateState != null && (UpdateState.State == UpdateState.Status.Queued || UpdateState.State == UpdateState.Status.Processing);


        public ArmaServerState(IMediator mediator, string id, string directory) : base(mediator, id, directory)
        {

        }

        public async Task InitializeAsync()
        {
            await RefreshSettingsAsync();

            var executableFileName = Settings.ExecutableFileName ?? "arma3server_x64.exe";

            ProcessMonitor = new ProcessMonitor(executableFileName, new ArmaArgumentStringBuilder(this));
        }

        public async Task RefreshSettingsAsync()
        {
            Settings = (await Mediator.Send(new GetArmaServerSettingsQuery { Id = Id })).Server;
        }

        public async Task<bool> CanStartAsync(CancellationToken cancellationToken = default)
        {
            return !await ProcessMonitor.IsRunningAsync();
        }

        public async Task<bool> CanStopAsync(CancellationToken cancellationToken = default)
        {
            return await ProcessMonitor.IsRunningAsync();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await ProcessMonitor.RunAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await ProcessMonitor.ShutdownAsync(cancellationToken);
        }

        public async Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
        {
            return await ProcessMonitor.IsRunningAsync();
        }

        public async Task<ServerInstallationStatus> GetInstallationStatusAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new ServerInstallationStatus
            {
                IsInstalled = Settings.IsInstalled,
                RequiresUpdate = false, // TODO: Make dynamic
                InstalledVersion = Settings.InstalledVersion,
                AvailableVersion = null // TODO: Make dynamic
            });
        }

        public async Task<bool> CanInstallOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            return !await IsRunningAsync(cancellationToken) && !IsUpdating;
        }

        public async Task BeginInstallationOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            UpdateState = (await Mediator.Send(new UpdateAppIdCmd
            {
                Id = Id,
                Directory = Directory,
                UpdateState = UpdateState
            })).UpdateState;
        }

        public Task CancelInstallationOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            UpdateState?.CancellationToken.Cancel();

            return Task.CompletedTask;
        }
    }
}
