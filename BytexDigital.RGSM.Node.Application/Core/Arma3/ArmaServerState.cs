using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Arma3.Commands;
using BytexDigital.RGSM.Node.Application.Core.Features.BattlEye;
using BytexDigital.RGSM.Node.Application.Core.Features.Installable;
using BytexDigital.RGSM.Node.Application.Core.Features.Runnable;
using BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs;
using BytexDigital.RGSM.Node.Application.Core.Features.Workshop;
using BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands;
using BytexDigital.RGSM.Node.Application.Core.Generic;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Core.Steam.Commands;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;
using BytexDigital.RGSM.Node.Domain.Models.Logs;
using BytexDigital.RGSM.Node.Domain.Models.Status;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaServerState : ServerStateBase, IRunnable, IServerLogs, IBattlEyeRcon, IWorkshopSupport, IWorkshopStorage, IInstallAndUpdatable
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
            try
            {
                Settings = (await Mediator.Send(new GetArmaServerSettingsQuery { Id = Id })).Server;
                Logger.Information("Refreshed server settings from database.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error refreshing server settings from database.");
                throw;
            }
        }

        public async Task RefreshSettingsAndReconfigureAsync()
        {
            await RefreshSettingsAsync();

            await ProcessMonitor.ConfigureAsync(BaseDirectory, Path.Combine(BaseDirectory, await GetExecutableFileNameAsync()));
            await RconMonitor.ConfigureAsync(Settings.RconIp, Settings.RconPort, Settings.RconPassword);
            await ModKeyManager.ConfigureAsync(Path.Combine(BaseDirectory, "keys"), BaseDirectory);
            await WriteBattlEyeConfigAsync();
        }

        public async Task CreateProcessMonitorAsync()
        {
            try
            {
                ProcessMonitor = new ProcessMonitor(ArgumentStringBuilder);

                await ProcessMonitor.ConfigureAsync(BaseDirectory, Path.Combine(BaseDirectory, await GetExecutableFileNameAsync()));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating process monitor.");
                throw;
            }
        }

        public async Task CreateRconMonitorAsync(CancellationToken cancellationToken = default)
        {
            try
            {

                RconMonitor = new BeRconMonitor();

                await RconMonitor.ConfigureAsync(Settings.RconIp, Settings.RconPort, Settings.RconPassword);

                Logger.Information("Created and configured rcon monitor.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating rcon monitor.");
                throw;
            }
        }

        public async Task CreateModKeyManagerAsync(CancellationToken cancellationToken = default)

        {
            try
            {
                ModKeyManager = new ModsKeyManager();

                await ModKeyManager.ConfigureAsync(Path.Combine(BaseDirectory, "keys"), BaseDirectory, cancellationToken);

                Logger.Information("Created and configured mod keys manager.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error creating mod manager.");
                throw;
            }
        }

        public async Task WriteBattlEyeConfigAsync(CancellationToken cancellationToken = default)
        {
            try
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

                Logger.Information($"Wrote BattlEye configuration to {bePath} with IP {Settings.RconIp}, port {Settings.RconPort}, password **REDACTED**");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Error writing BattlEye configuration.");
                throw;
            }
        }

        public Task<string> GetProfilesPathAsync(CancellationToken cancellationToken = default)
        {
            var path = Settings.ProfilesPath ?? "profiles";

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(BaseDirectory, path);
            }

            return Task.FromResult(path);
        }

        public Task<string> GetBattlEyePathAsync(CancellationToken cancellationToken = default)
        {
            var path = Settings.BattlEyePath ?? "battleye";

            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(BaseDirectory, path);
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
            try
            {
                await RefreshSettingsAndReconfigureAsync();
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
                        return Settings.Server.TrackedDepots.Any(x => x.DepotId == depot.Id);
                    }
                })).UpdateState;

                _ = Task.Run(async () =>
                {
                    await ServerUpdateState.ProcessedEvent.WaitAsync(cancellationToken);

                    if (ServerUpdateState.CancellationToken.IsCancellationRequested) return;
                    if (ServerUpdateState.FailureException != default) return;

                    await Mediator.Send(new MarkArmaServerAsInstalledCmd { Id = Id });
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

            foreach (var trackedMod in (await Mediator.Send(new GetWorkshopModsQuery { ServerId = Id })).TrackedWorkshopMods)
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
            try
            {
                _isUpdatingMods = true;

                List<Task> modTasks = new List<Task>();

                foreach (var trackedMod in (await Mediator.Send(new GetWorkshopModsQuery { ServerId = Id })).TrackedWorkshopMods)
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

                Logger.Information($"Beginning update of {modTasks.Count} workshop mods.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Beginning update of workshop mods failed.");
                throw;
            }
        }

        public Task CancelUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            if (WorkshopUpdateStates == null) return Task.CompletedTask;

            foreach (var updateState in WorkshopUpdateStates.ToList())
            {
                updateState.Value.CancellationToken.Cancel();
            }

            Logger.Information($"Cancelled update of workshop mods.");

            return Task.CompletedTask;
        }

        public Task<string> GetWorkshopModPathAsync(Domain.Entities.TrackedWorkshopMod trackedWorkshopMod, CancellationToken cancellationToken)
        {
            string modDirectory = trackedWorkshopMod.Directory ?? Path.Combine(".workshop", trackedWorkshopMod.PublishedFileId.ToString());

            if (!Path.IsPathRooted(modDirectory)) modDirectory = Path.Combine(BaseDirectory, modDirectory);

            return Task.FromResult(modDirectory);
        }

        public async Task<List<LogSource>> GetLogSourcesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var profilesFolder = await GetProfilesPathAsync(cancellationToken);
                if (!Path.IsPathRooted(profilesFolder)) profilesFolder = System.IO.Path.Combine(BaseDirectory, profilesFolder);
                if (!Directory.Exists(profilesFolder)) return new List<LogSource>();

                List<LogSource> sources = new List<LogSource>();

                // RGSM logs
                var rgsmLogsDirectory = Path.Combine(BaseDirectory, ".rgsm", "logs");

                if (Directory.Exists(rgsmLogsDirectory))
                {
                    var rgsmLogs = Directory.GetFiles(rgsmLogsDirectory, "*.log");

                    foreach (var rgsmLogFile in rgsmLogs)
                    {
                        sources.Add(new LogSource
                        {
                            Type = "rgsm",
                            Name = $"rgsm:{Path.GetFileNameWithoutExtension(rgsmLogFile)}",
                            SizeInBytes = new FileInfo(rgsmLogFile).Length,
                            TimeLastUpdated = new DateTimeOffset(File.GetLastWriteTimeUtc(rgsmLogFile), TimeSpan.Zero),
                            MetaValues = new Dictionary<string, string>
                            {
                                { "path", rgsmLogFile }
                            }
                        });
                    }
                }

                // console_xxxxx.log files
                var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.log");

                foreach (var consoleFilePath in consoleFiles)
                {
                    sources.Add(new LogSource
                    {
                        Type = "console_file",
                        Name = Path.GetFileNameWithoutExtension(consoleFilePath),
                        SizeInBytes = new FileInfo(consoleFilePath).Length,
                        TimeLastUpdated = new DateTimeOffset(File.GetLastWriteTimeUtc(consoleFilePath), TimeSpan.Zero),
                        MetaValues = new Dictionary<string, string>
                        {
                            { "path", consoleFilePath }
                        }
                    });
                }

                // RPT files
                var rptFiles = System.IO.Directory.GetFiles(profilesFolder, "*.rpt");

                foreach (var rptFilePath in rptFiles)
                {
                    sources.Add(new LogSource
                    {
                        Type = "rpt_file",
                        Name = Path.GetFileNameWithoutExtension(rptFilePath),
                        SizeInBytes = new FileInfo(rptFilePath).Length,
                        TimeLastUpdated = new DateTimeOffset(File.GetLastWriteTimeUtc(rptFilePath), TimeSpan.Zero),
                        MetaValues = new Dictionary<string, string>
                        {
                            { "path", rptFilePath }
                        }
                    });
                }

                return sources;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fetching log sources failed.");
                throw;
            }
        }

        public async Task<LogSource> GetPrimaryLogSourceOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var primarySource = (await GetLogSourcesAsync(cancellationToken))
                    .Where(x => x.Type == "rpt_file")
                    .OrderByDescending(x => x.TimeLastUpdated)
                    .FirstOrDefault();

                return primarySource;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fetching primary log source failed.");
                throw;
            }
        }

        public async Task<LogContent> GetLogContentOrDefaultAsync(string logSourceName, int limitLines = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                var profilesFolder = await GetProfilesPathAsync(cancellationToken);
                if (!System.IO.Path.IsPathRooted(profilesFolder)) profilesFolder = System.IO.Path.Combine(BaseDirectory, profilesFolder);

                var sources = await GetLogSourcesAsync(cancellationToken);
                var requestSource = sources.FirstOrDefault(x => x.Name == logSourceName);

                if (requestSource == default) return default;

                string fileToReadPath = default;

                if (requestSource.Type == "rgsm")
                {
                    var rgsmFiles = System.IO.Directory.GetFiles(Path.Combine(BaseDirectory, ".rgsm", "logs"), "*.log");
                    var requestedRgsmFile = rgsmFiles.FirstOrDefault(x =>
                        Path.GetFileNameWithoutExtension(x) == Path.GetFileNameWithoutExtension(requestSource.MetaValues["path"]));

                    fileToReadPath = requestedRgsmFile;
                }
                else if (requestSource.Type == "console_file")
                {
                    var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.log");
                    var requestedConsoleFile = consoleFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == requestSource.Name);

                    fileToReadPath = requestedConsoleFile;
                }
                else if (requestSource.Type == "rpt_file")
                {
                    var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.rpt");
                    var requestedConsoleFile = consoleFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == requestSource.Name);

                    fileToReadPath = requestedConsoleFile;
                }

                if (!File.Exists(fileToReadPath)) return null;

                using var fileStream = new FileStream(fileToReadPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fileStream);

                var content = await reader.ReadToEndAsync();
                var lines = content.Split(Environment.NewLine).ToList();

                if (limitLines > 0)
                {
                    lines = lines.TakeLast(limitLines).ToList();
                }

                return new LogContent
                {
                    Lines = lines
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fetching log content failed.");
                throw;
            }
        }
    }
}
