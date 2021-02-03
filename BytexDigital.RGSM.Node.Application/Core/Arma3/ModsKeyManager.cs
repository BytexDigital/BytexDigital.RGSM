using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ModsKeyManager : IAsyncDisposable
    {
        private string _keysDirectory;
        private string _serverDirectory;

        private Options _options;
        private readonly ArmaServerState _armaServerState;

        private string OptionsPath => Path.Combine(_serverDirectory, ".rgsm", "keys.json");

        public ModsKeyManager(ArmaServerState armaServerState)
        {
            _armaServerState = armaServerState;
        }

        public async Task ConfigureAsync(CancellationToken cancellationToken = default)
        {
            _keysDirectory = Path.Combine(_armaServerState.BaseDirectory, "keys");
            _serverDirectory = _armaServerState.BaseDirectory;

            _options = await GetOptionsAsync(cancellationToken);
        }

        public async Task SynchronizeAllAsync(CancellationToken cancellationToken = default)
        {
            var mods = await _armaServerState.GetModsAsync(cancellationToken);

            // Deactivate the keys of all mods
            var options = await GetOptionsAsync(cancellationToken);

            foreach (var trackedKey in options.TrackedKeys.ToList())
            {
                var keyPath = Path.Combine(_keysDirectory, trackedKey.FileName);

                if (File.Exists(keyPath)) File.Delete(keyPath);

                options.TrackedKeys.Remove(trackedKey);
            }

            // Add all keys of the mods that are going to be loaded that are NOT only serversided, as these mods do not require their bikeys in the key directory
            foreach (var mod in mods.Where(x => !x.LoadOnlyOnServer))
            {
                string identifier = default;

                if (mod.WorkshopMod != null)
                {
                    // Use the workshop ID to keep track of the loaded keys belonging to this mod
                    identifier = mod.WorkshopMod.Id.ToString();
                }
                else
                {
                    string idFilePath = Path.Combine(mod.Path, ".rgsm-id");

                    if (!File.Exists(idFilePath))
                    {
                        await File.WriteAllTextAsync(idFilePath, Guid.NewGuid().ToString(), cancellationToken);
                    }

                    identifier = await File.ReadAllTextAsync(idFilePath, cancellationToken);
                }

                // Find all bikeys
                var keys = Directory.GetFiles(mod.Path, "*.bikey", SearchOption.AllDirectories);

                // Add all bikeys to the keys directory of the server and add an entry in the Options to track which keys we have loaded so we can remove
                // them next time to ensure an up-to-date keys directory that doesn't accidentally allow mods we are no longer running on the server!
                foreach (var keyPath in keys)
                {
                    var keyFileName = Path.GetFileName(keyPath);
                    var keyGeneralizedFileName = $"{identifier}_{keyFileName}";

                    if (options.IgnoredKeys != null && options.IgnoredKeys.Contains(keyFileName)) continue;
                    if (options.TrackedKeys.Any(x => x.FileName == keyGeneralizedFileName)) continue;

                    options.TrackedKeys.Add(new TrackedKey
                    {
                        FileName = keyGeneralizedFileName,
                        ModIdentifier = identifier
                    });

                    File.Copy(keyPath, Path.Combine(_keysDirectory, keyGeneralizedFileName));
                }
            }
        }

        private async Task<Options> GetOptionsAsync(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(OptionsPath)) return new Options
            {
                IgnoredKeys = new List<string>(),
                TrackedKeys = new List<TrackedKey>()
            };

            return JsonSerializer.Deserialize<Options>(await File.ReadAllTextAsync(OptionsPath, cancellationToken));
        }

        private async Task WriteOptionsAsync(Options options, CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(OptionsPath));

            await File.WriteAllTextAsync(OptionsPath, JsonSerializer.Serialize(options, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        public async ValueTask DisposeAsync()
        {
            await WriteOptionsAsync(_options);
        }

        private class TrackedKey
        {
            public string ModIdentifier { get; set; }
            public string FileName { get; set; }
        }

        private class Options
        {
            public List<string> IgnoredKeys { get; set; }
            public List<TrackedKey> TrackedKeys { get; set; }
        }
    }
}
