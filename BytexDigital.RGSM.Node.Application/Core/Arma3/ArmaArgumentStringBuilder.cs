
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Generic;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.RGSM.Shared;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaArgumentStringBuilder : ArgumentStringBuilder
    {
        private readonly ArmaServerState _armaServerState;
        private static readonly List<string> _managedArguments = new List<string> {
            "-server",
            "-port",
            "-bepath",
            "-config",
            "-profiles",
            "-mod",
            "-servermod"
        };

        public ArmaArgumentStringBuilder(ArmaServerState armaServerState)
        {
            _armaServerState = armaServerState;
        }

        public override async Task<string> BuildAsync(CancellationToken cancellationToken = default)
        {
            List<string> arguments = new List<string>();

            arguments.Add("-server");
            arguments.Add($"-port={_armaServerState.Settings.Port}");
            arguments.Add($"-bepath={await _armaServerState.GetBattlEyePathAsync(cancellationToken)}");
            arguments.Add($"-config={Path.Combine(".rgsm", "server.cfg")}");
            arguments.Add($"-profiles={await _armaServerState.GetProfilesPathAsync(cancellationToken)}");

            // Get mods that should be loaded as -mod
            var mods = (await GetModPathsAsync(cancellationToken));
            var globalMods = mods.Where(x => !x.LoadOnlyOnServer).ToList();

            arguments.Add($"-mod={string.Join(";", globalMods.Select(x => x.Path))}");

            // Get mods that should be loaded as -servermod
            var serverMods = mods.Where(x => x.LoadOnlyOnServer).ToList();

            arguments.Add($"-serverMod={string.Join(";", serverMods.Select(x => x.Path))}");

            // Merge all additional arguments that are not managed by RGSM
            foreach (var additionalArgument in await ArgumentsHelper.GetArgumentsListAsync(_armaServerState.Settings.AdditionalArguments, cancellationToken))
            {
                // Ignore managed arguments
                if (_managedArguments.Any(x => additionalArgument.StartsWith(x))) continue;

                arguments.Add(additionalArgument);
            }

            return await Task.FromResult(string.Join(" ", arguments.Select(x => $"\"{x}\"")));
        }

        public async Task<List<(WorkshopMod WorkshopMod, string Path, bool LoadOnlyOnServer)>> GetModPathsAsync(CancellationToken cancellationToken = default)
        {
            List<(WorkshopMod, string, bool)> mods = new List<(WorkshopMod, string, bool)>();

            var workshopMods = _armaServerState.Settings.WorkshopMods;

            foreach (var workshopMod in workshopMods)
            {
                if (!workshopMod.Enabled) continue;

                mods.Add((workshopMod, workshopMod.Directory, workshopMod.Metadata != null & workshopMod.Metadata.ContainsKey("server")));
            }

            // Merge with unmanaged mods
            var customArguments = await ArgumentsHelper.GetArgumentsListAsync(_armaServerState.Settings.AdditionalArguments, cancellationToken);
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
    }
}
