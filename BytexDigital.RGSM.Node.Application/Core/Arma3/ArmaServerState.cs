using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Core.Arma3.Commands;
using BytexDigital.RGSM.Node.Application.Core.BattlEye;
using BytexDigital.RGSM.Node.Application.Core.Commands.Workshop;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Core.Generic;
using BytexDigital.RGSM.Node.Application.Core.SteamCmd;
using BytexDigital.RGSM.Node.Application.Core.SteamCmd.Commands;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;
using BytexDigital.RGSM.Node.Domain.Models.Console;
using BytexDigital.RGSM.Node.Domain.Models.Status;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaServerState : ServerStateBase, IRunnable, IProvidesConsoleOutput, IBattlEyeRcon, IWorkshopSupport, IWorkshopStorage
    {
        public const uint DEDICATED_SERVER_APP_ID = 233780;

        public Arma3Server Settings { get; private set; }
        public ProcessMonitor ProcessMonitor { get; private set; }
        public BeRconMonitor RconMonitor { get; private set; }
        public ArmaArgumentStringBuilder ArgumentStringBuilder { get; private set; }
        public ModsKeyManager ModKeyManager { get; private set; }
        public UpdateState ServerUpdateState { get; private set; }
        public ConcurrentDictionary<PublishedFileId, UpdateState> WorkshopUpdateStates { get; private set; }

        public bool IsUpdating => ServerUpdateState != null &&
            (ServerUpdateState.State == UpdateState.Status.Queued || ServerUpdateState.State == UpdateState.Status.Processing);

        public bool IsUpdatingWorkshopMods => WorkshopUpdateStates != null &&
            (WorkshopUpdateStates.Any(x => x.Value.State == UpdateState.Status.Queued || x.Value.State == UpdateState.Status.Processing) || _isUpdatingMods);

        private bool _isUpdatingMods = false;


        public ArmaServerState(IMediator mediator, string id, string directory) : base(mediator, id, directory)
        {
            WorkshopUpdateStates = new ConcurrentDictionary<PublishedFileId, UpdateState>();
        }

        public override async Task InitializeAsync()
        {
            await RefreshSettingsAsync();

            ArgumentStringBuilder = new ArmaArgumentStringBuilder(this);

            await CreateProcessMonitorAsync();
            await CreateRconMonitorAsync();
            await CreateModKeyManagerAsync();
        }

        public async Task RefreshSettingsAsync()
        {
            Settings = (await Mediator.Send(new GetArmaServerSettingsQuery { Id = Id })).Server;
        }

        public async Task RefreshSettingsAndReconfigureAsync()
        {
            await RefreshSettingsAsync();

            await ProcessMonitor.ConfigureAsync(Directory, Path.Combine(Directory, await GetExecutableFileNameAsync()));
            await RconMonitor.ConfigureAsync(Settings.RconIp, Settings.RconPort, Settings.RconPassword);
            await ModKeyManager.ConfigureAsync(Path.Combine(Directory, "keys"), Directory);
            await WriteBattlEyeConfigAsync();
        }

        public async Task CreateProcessMonitorAsync()
        {
            ProcessMonitor = new ProcessMonitor(ArgumentStringBuilder);

            await ProcessMonitor.ConfigureAsync(Directory, Path.Combine(Directory, await GetExecutableFileNameAsync()));
        }

        public async Task CreateRconMonitorAsync(CancellationToken cancellationToken = default)
        {
            RconMonitor = new BeRconMonitor(this);

            await RconMonitor.ConfigureAsync(Settings.RconIp, Settings.RconPort, Settings.RconPassword);
        }

        public async Task CreateModKeyManagerAsync(CancellationToken cancellationToken = default)
        {
            ModKeyManager = new ModsKeyManager();

            await ModKeyManager.ConfigureAsync(Path.Combine(Directory, "keys"), Directory, cancellationToken);
        }

        public async Task WriteBattlEyeConfigAsync(CancellationToken cancellationToken = default)
        {
            var bePath = await GetBattlEyePathAsync(cancellationToken);

            var config32Path = Path.Combine(bePath, "beserver.cfg");
            var config64Path = Path.Combine(bePath, "beserver_x64.cfg");

            File.Delete(config32Path);
            File.Delete(config64Path);

            var contentLines = new List<string>
            {
                $"// AUTO GENERATED - CHANGES WILL BE OVERWRITTEN",
                $"RConPassword {Settings.RconPassword}",
                $"RConPort {Settings.RconPort}",
                $"RConIP {Settings.RconIp}"
            };

            await File.WriteAllLinesAsync(config32Path, contentLines);
            await File.WriteAllLinesAsync(config64Path, contentLines);
        }

        public Task<string> GetProfilesPathAsync(CancellationToken cancellationToken = default)
        {
            var path = Settings.ProfilesPath ?? "profiles";

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Directory, path);
            }

            return Task.FromResult(path);
        }

        public Task<string> GetBattlEyePathAsync(CancellationToken cancellationToken = default)
        {
            var path = Settings.BattlEyePath ?? "battleye";

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Directory, path);
            }

            return Task.FromResult(path);
        }

        public Task<string> GetExecutableFileNameAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Settings.ExecutableFileName ?? "arma3server_x64.exe");
        }

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
            await RefreshSettingsAndReconfigureAsync();
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
                RequiresUpdate = false, // TODO: Make dynamic,
                IsUpdating = IsUpdating,
                UpdateProgress = ServerUpdateState?.Progress ?? 0
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
            var appId = Settings.AppId.HasValue ? (uint)Settings.AppId.Value : DEDICATED_SERVER_APP_ID;

            ServerUpdateState = (await Mediator.Send(new UpdateAppCmd
            {
                Id = Id,
                AppId = appId,
                UseAnonymousUser = false,
                Directory = Directory,
                UpdateState = ServerUpdateState,
                Branch = Settings.Branch ?? "public",
                BranchPassword = null,

                DepotCondition = depot =>
                {
                    return Settings.Server.TrackedDepots.Any(x => x.DepotId == depot.Id);
                }
            })).UpdateState;

            _ = Task.Run(async () =>
            {
                await ServerUpdateState.ProcessedEvent.WaitAsync(cancellationToken);

                if (ServerUpdateState.CancellationToken.IsCancellationRequested) return;

                await Mediator.Send(new MarkArmaServerAsInstalledCmd { Id = Id });
            });
        }

        public Task CancelInstallationOrUpdateAsync(CancellationToken cancellationToken = default)
        {
            ServerUpdateState?.CancellationToken.Cancel();

            return Task.CompletedTask;
        }

        public async Task<List<ConsoleOutputContent>> GetConsoleOutputAsync(List<string> identifiers = default, int lastNLines = 0, CancellationToken cancellationToken = default)
        {
            var profilesFolder = await GetProfilesPathAsync(cancellationToken);

            if (!System.IO.Path.IsPathRooted(profilesFolder)) profilesFolder = System.IO.Path.Combine(Directory, profilesFolder);

            if (!System.IO.Directory.Exists(profilesFolder)) return new List<ConsoleOutputContent>();

            List<ConsoleOutputContent> outputs = new List<ConsoleOutputContent>();

            if (identifiers == default || identifiers.Contains("console"))
            {
                var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.log");
                var currentConsoleFilePath = consoleFiles.OrderByDescending(x => new System.IO.FileInfo(x).LastWriteTimeUtc).FirstOrDefault();

                if (currentConsoleFilePath != default)
                {
                    using (var fileStream = new FileStream(currentConsoleFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        var content = await reader.ReadToEndAsync();
                        var lines = content.Split(Environment.NewLine).ToList();

                        if (lastNLines > 0)
                        {
                            lines = lines.TakeLast(lastNLines).ToList();
                        }

                        outputs.Add(new ConsoleOutputContent
                        {
                            Type = "console",
                            FromFile = currentConsoleFilePath,
                            Lines = lines
                        });
                    }
                }
            }

            if (identifiers == default || identifiers.Contains("rpt"))
            {
                var rptFiles = System.IO.Directory.GetFiles(profilesFolder, "*.rpt");
                var currentRptFilePath = rptFiles.OrderByDescending(x => x).FirstOrDefault();

                if (currentRptFilePath != default)
                {

                    using (var fileStream = new FileStream(currentRptFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        var content = await reader.ReadToEndAsync();
                        var lines = content.Split(Environment.NewLine).ToList();

                        if (lastNLines > 0)
                        {
                            lines = lines.TakeLast(lastNLines).ToList();
                        }

                        outputs.Add(new ConsoleOutputContent
                        {
                            Type = "rpt",
                            FromFile = currentRptFilePath,
                            Lines = lines
                        });
                    }
                }
            }

            return outputs;
        }

        public Task<BeRconStatus> IsBeRconConnectedAsync(CancellationToken cancellationToken1 = default)
        {
            return Task.FromResult(new BeRconStatus
            {
                IsConnected = RconMonitor.IsConnected()
            });
        }

        public Task<List<BeRconMessage>> GetBeRconMessagesAsync(int limit = 0, CancellationToken cancellationToken = default)
        {
            return RconMonitor.GetMessagesAsync(limit, cancellationToken);
        }

        public Task<List<BeRconPlayer>> GetBeRconPlayersAsync(CancellationToken cancellationToken = default)
        {
            return RconMonitor.GetPlayersAsync(cancellationToken);
        }

        public async Task SendBeRconMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            await RconMonitor.SendMessageAsync(message, cancellationToken);
        }

        public async Task<List<WorkshopItem>> GetWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            var mods = new List<WorkshopItem>();

            foreach (var trackedMod in (await Mediator.Send(new GetWorkshopModsQuery { Id = Id })).TrackedWorkshopMods)
            {
                string modDirectory = await GetWorkshopModPathAsync(trackedMod, cancellationToken);

                mods.Add(new WorkshopItem
                {
                    Id = trackedMod.PublishedFileId,
                    IsInstalled = System.IO.Directory.Exists(modDirectory),
                    RequiresUpdate = false, // TODO: Make dynamic

                    IsUpdating = WorkshopUpdateStates.Any(x =>
                        x.Key == trackedMod.PublishedFileId && (x.Value.State == UpdateState.Status.Queued || x.Value.State == UpdateState.Status.Processing)),
                    UpdateProgress = WorkshopUpdateStates.GetValueOrDefault(trackedMod.PublishedFileId)?.Progress ?? 0,
                    UpdateFailureReason = WorkshopUpdateStates.GetValueOrDefault(trackedMod.PublishedFileId)?.FailureException?.Message
                });
            }

            return mods;
        }

        public async Task<CanResult> CanUpdateWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            if (await IsRunningAsync(cancellationToken)) return CanResult.CannotBecause("Server is running.");
            if (IsUpdating) return CanResult.CannotBecause("Server is updating.");
            if (IsUpdatingWorkshopMods) return CanResult.CannotBecause("Server is updating workshop mods.");

            return CanResult.Can();
        }

        public async Task BeginUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            _isUpdatingMods = true;

            List<Task> modTasks = new List<Task>();

            foreach (var trackedMod in (await Mediator.Send(new GetWorkshopModsQuery { Id = Id })).TrackedWorkshopMods)
            {
                string modDirectory = await GetWorkshopModPathAsync(trackedMod, cancellationToken);

                var response = await Mediator.Send(new UpdatePublishedFileCmd
                {
                    AppId = /*440,*/ trackedMod.OfAppId ?? 107410,
                    PublishedFileId = trackedMod.PublishedFileId,
                    Directory = modDirectory,
                    //UseAnonymousUser = true
                });

                WorkshopUpdateStates.AddOrUpdate(trackedMod.PublishedFileId, response.UpdateState, (key, value) => response.UpdateState);

                var modTask = Task.Run(async () =>
                {
                    // Wait for the mod to finish downloading
                    await response.UpdateState.ProcessedEvent.WaitAsync(cancellationToken);

                    if (response.UpdateState.CancellationToken.IsCancellationRequested) return;

                    // Do not continue if an exception occurred
                    if (response.UpdateState.FailureException != default) return;

                    // Activate the keys from this downloaded mod and deactivate old keys
                    await ModKeyManager.DeactivateKeysAsync(trackedMod.PublishedFileId, response.UpdateState.CancellationToken.Token);
                    await ModKeyManager.ActivateKeysAsync(trackedMod.PublishedFileId, modDirectory, response.UpdateState.CancellationToken.Token);
                });

                modTasks.Add(modTask);
            }

            _ = Task.Run(async () =>
            {
                await Task.WhenAll(modTasks);

                _isUpdatingMods = false;
            });
        }

        public Task CancelUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetWorkshopModPathAsync(Domain.Entities.TrackedWorkshopMod trackedWorkshopMod, CancellationToken cancellationToken)
        {
            string modDirectory = trackedWorkshopMod.Directory ?? Path.Combine(".workshop", trackedWorkshopMod.PublishedFileId.ToString());

            if (!Path.IsPathRooted(modDirectory)) modDirectory = Path.Combine(Directory, modDirectory);

            return Task.FromResult(modDirectory);
        }
    }
}
