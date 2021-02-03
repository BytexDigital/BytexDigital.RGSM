using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.BattlEye;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public partial class ArmaServerState : IBattlEyeRcon
    {
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
    }
}
