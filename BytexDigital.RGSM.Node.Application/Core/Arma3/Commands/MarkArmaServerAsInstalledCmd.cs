using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3.Commands
{

    public class MarkArmaServerAsInstalledCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<MarkArmaServerAsInstalledCmd>
        {
            private readonly ServersService _serversService;
            private readonly ArmaServerService _armaServerService;

            public Handler(ServersService serversService, ArmaServerService armaServerService)
            {
                _serversService = serversService;
                _armaServerService = armaServerService;
            }

            public async Task<Unit> Handle(MarkArmaServerAsInstalledCmd request, CancellationToken cancellationToken)
            {
                var server = await _armaServerService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null)
                    throw new ServerNotFoundException();

                await _armaServerService.MarkAsInstalledAsync(server);

                return Unit.Value;
            }
        }
    }
}
