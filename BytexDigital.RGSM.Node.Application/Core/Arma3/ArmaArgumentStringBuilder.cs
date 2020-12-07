
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Generic;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public class ArmaArgumentStringBuilder : ArgumentStringBuilder
    {
        private readonly ArmaServerState _armaServerState;

        public ArmaArgumentStringBuilder(ArmaServerState armaServerState)
        {
            _armaServerState = armaServerState;
        }

        public override async Task<string> BuildAsync()
        {
            List<string> arguments = new List<string>();

            arguments.Add("-server");
            arguments.Add($"-port={_armaServerState.Settings.Port}");
            arguments.Add($"-bepath={Path.Combine("rgsm", "battleye")}");
            arguments.Add($"-config={Path.Combine("rgsm", "server.cfg")}");

            return await Task.FromResult(string.Join(" ", arguments.Select(x => $"\"{x}\"")));
        }
    }
}
