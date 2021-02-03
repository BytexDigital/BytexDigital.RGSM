using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.BattlEye;
using BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs;
using BytexDigital.RGSM.Node.Application.Core.Generic;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Domain.Models.Arma;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.RGSM.Shared;
using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public partial class ArmaServerState : ServerStateBase, IAsyncDisposable
    {
        public const uint DEDICATED_SERVER_APP_ID = 233780;

        public ArmaServer Settings { get; private set; }
        public ProcessMonitor ProcessMonitor { get; private set; }
        public BeRconMonitor RconMonitor { get; private set; }
        public ArmaArgumentStringBuilder ArgumentStringBuilder { get; private set; }
        public ModsKeyManager ModKeyManager { get; private set; }
        public UpdateState ServerUpdateState { get; private set; }
        public ConcurrentDictionary<PublishedFileId, UpdateState> WorkshopUpdateStates { get; private set; }

        public string SettingsPath => Path.Combine(BaseDirectory, ".rgsm", "settings.json");

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
            ArgumentStringBuilder = new ArmaArgumentStringBuilder(this);

            await LoadOrCreateSettingsAsync();
            await CreateProcessMonitorAsync();
            await CreateRconMonitorAsync();
            await CreateModKeyManagerAsync();
        }

        public async Task LoadOrCreateSettingsAsync()
        {
            if (!File.Exists(SettingsPath))
            {
                CreateDefaultSettings();
            }
            else
            {
                Settings = JsonSerializer.Deserialize<ArmaServer>(await File.ReadAllTextAsync(SettingsPath));
            }
        }

        public void CreateDefaultSettings()
        {
            Settings = new ArmaServer
            {
                IsInstalled = false,
                Port = 2302,

                RconIp = "0.0.0.0",
                RconPort = 2302 + 10,
                RconPassword = Guid.NewGuid().ToString(),
                Depots = new List<uint> { 233781, 233782 }
            };
        }

        public async Task SaveSettingsAsync()
        {
            await File.WriteAllTextAsync(SettingsPath, JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true }));
        }

        public async Task ReconfigureAsync()
        {
            await ProcessMonitor.ConfigureAsync(BaseDirectory, Path.Combine(BaseDirectory, await GetExecutableFileNameAsync()));
            await RconMonitor.ConfigureAsync(Settings.RconIp, Settings.RconPort, Settings.RconPassword);
            await ModKeyManager.ConfigureAsync();
            await ModKeyManager.SynchronizeAllAsync(this);
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

        public async Task<List<(WorkshopMod WorkshopMod, string Path, bool LoadOnlyOnServer)>> GetModsAsync(CancellationToken cancellationToken = default)
        {
            List<(WorkshopMod, string, bool)> mods = new List<(WorkshopMod, string, bool)>();

            var workshopMods = Settings.WorkshopMods;

            foreach (var workshopMod in workshopMods)
            {
                if (!workshopMod.Enabled) continue;

                mods.Add((workshopMod, workshopMod.Directory, workshopMod.Metadata != null & workshopMod.Metadata.ContainsKey("server")));
            }

            // Merge with unmanaged mods
            var customArguments = await ArgumentsHelper.GetArgumentsListAsync(Settings.AdditionalArguments, cancellationToken);
            var modArguments = customArguments.FirstOrDefault(x => x.StartsWith("-mod="));
            var serverModArguments = customArguments.FirstOrDefault(x => x.ToLower().StartsWith("-servermod="));

            if (!string.IsNullOrEmpty(modArguments))
            {
                var paths = modArguments.Substring(5).Split(";", System.StringSplitOptions.RemoveEmptyEntries);

                foreach (var path in paths)
                {
                    mods.Add((null, path, false));
                }
            }

            if (!string.IsNullOrEmpty(serverModArguments))
            {
                var paths = serverModArguments.Substring(5).Split(";", System.StringSplitOptions.RemoveEmptyEntries);

                foreach (var path in paths)
                {
                    mods.Add((null, path, true));
                }
            }

            return mods;
        }

        public async ValueTask DisposeAsync()
        {
            await SaveSettingsAsync();
        }
    }
}
