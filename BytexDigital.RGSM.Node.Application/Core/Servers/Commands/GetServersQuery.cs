using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Servers.Commands
{
    public class GetServersQuery : IRequest<GetServersQuery.Response>
    {
        public Func<IQueryable<Server>, IQueryable<Server>> Query { get; set; }

        public class Handler : IRequestHandler<GetServersQuery, Response>
        {
            private readonly ServerService _serversService;

            public Handler(ServerService serversService)
            {
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetServersQuery request, CancellationToken cancellationToken)
            {
                var servers = _serversService.GetServers();

                if (request.Query != null)
                {
                    servers = request.Query.Invoke(servers);
                }

                return new Response { Servers = await servers.ToListAsync() };
            }
        }

        public class Response
        {
            public List<Server> Servers { get; set; }
        }
    }
}
