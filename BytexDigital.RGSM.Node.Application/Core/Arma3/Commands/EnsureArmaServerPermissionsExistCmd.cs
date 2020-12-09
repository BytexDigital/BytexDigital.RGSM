using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3.Commands
{

    public class EnsureArmaServerPermissionsExistCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<EnsureArmaServerPermissionsExistCmd>
        {
            private readonly ArmaServerService _armaServerService;

            public Handler(ArmaServerService armaServerService)
            {
                _armaServerService = armaServerService;
            }

            public async Task<Unit> Handle(EnsureArmaServerPermissionsExistCmd request, CancellationToken cancellationToken)
            {
                var server = await _armaServerService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                await _armaServerService.EnsurePermissionsExistAsync(server, cancellationToken);

                return Unit.Value;
            }
        }
    }
}
