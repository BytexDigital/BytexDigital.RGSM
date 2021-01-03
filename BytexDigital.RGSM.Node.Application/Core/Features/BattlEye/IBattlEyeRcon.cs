using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.BattlEye;

namespace BytexDigital.RGSM.Node.Application.Core.Features.BattlEye
{
    public interface IBattlEyeRcon
    {
        Task<BeRconStatus> IsBeRconConnectedAsync(CancellationToken cancellationToken1 = default);
        Task<List<BeRconMessage>> GetBeRconMessagesAsync(int limit = 0, CancellationToken cancellationToken = default);
        Task<List<BeRconPlayer>> GetBeRconPlayersAsync(CancellationToken cancellationToken = default);
        Task SendBeRconMessageAsync(string message, CancellationToken cancellationToken = default);
    }
}
