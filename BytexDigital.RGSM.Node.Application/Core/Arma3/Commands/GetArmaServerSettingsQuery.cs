using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Domain.Entities.Arma3;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3.Commands
{

    public class GetArmaServerSettingsQuery : IRequest<GetArmaServerSettingsQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetArmaServerSettingsQuery, Response>
        {
            private readonly ServersService _serversService;

            public Handler(ServersService serversService)
            {
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetArmaServerSettingsQuery request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    Server = await _serversService.GetArmaServer(request.Id)
                        .Include(x => x.Server).ThenInclude(x => x.TrackedDepots)
                        .FirstAsync()
                };
            }
        }

        public class Response
        {
            public Arma3Server Server { get; set; }
        }
    }
}
