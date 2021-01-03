using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class GetServerPermissionsQuery : IRequest<GetServerPermissionsQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetServerPermissionsQuery, Response>
        {
            private readonly ServersService _serversService;

            public Handler(ServersService serversService)
            {
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetServerPermissionsQuery request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                return new Response
                {
                    Permissions = await _serversService.GetServerPermissions(server).ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<Permission> Permissions { get; set; }
        }
    }
}