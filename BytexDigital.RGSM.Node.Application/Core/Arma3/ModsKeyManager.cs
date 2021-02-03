using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Steam.Core.Structs;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ModsKeyManager
    {
        private string _keysDirectory;
        private string _serverDirectory;

        private string OptionsPath => Path.Combine(_serverDirectory, ".rgsm", "keys.json");

        public Task ConfigureAsync(string keysDirectory, string serverDirectory, CancellationToken cancellationToken = default)
        {
            _keysDirectory = keysDirectory;
            _serverDirectory = serverDirectory;

            return Task.CompletedTask;
        }

        public async Task ActivateKeysAsync(string identifier, string directory, CancellationToken cancellationToken = default)
        {
            var keys = Directory.GetFiles(directory, "*.bikey", SearchOption.AllDirectories);
            var options = await GetOptionsAsync(cancellationToken);

            foreach (var keyPath in keys)
            {
                var keyFileName = Path.GetFileName(keyPath);
                var keyGeneralizedFileName = $"{identifier}_{keyFileName}";

                if (options.IgnoredKeys != null && options.IgnoredKeys.Contains(keyFileName)) continue;
                if (options.TrackedKeys.Any(x => x.FileName == keyGeneralizedFileName)) continue;

                options.TrackedKeys.Add(new TrackedKey
                {
                    FileName = keyGeneralizedFileName,
                    Identifier = identifier
                });

                File.Copy(keyPath, Path.Combine(_keysDirectory, keyGeneralizedFileName));
            }

            await WriteOptionsAsync(options, cancellationToken);
        }

        public async Task DeactivateKeysAsync(string identifier, CancellationToken cancellationToken = default)
        {
            var options = await GetOptionsAsync(cancellationToken);

            foreach (var trackedKey in options.TrackedKeys.ToList())
            {
                if (trackedKey.Identifier != identifier) continue;

                var keyPath = Path.Combine(_keysDirectory, trackedKey.FileName);

                if (File.Exists(keyPath)) File.Delete(keyPath);

                options.TrackedKeys.Remove(trackedKey);
            }

            await WriteOptionsAsync(options, cancellationToken);
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

        private class TrackedKey
        {
            public string Identifier { get; set; }
            public string FileName { get; set; }
        }

        private class Options
        {
            public List<string> IgnoredKeys { get; set; }
            public List<TrackedKey> TrackedKeys { get; set; }
        }
    }
}
