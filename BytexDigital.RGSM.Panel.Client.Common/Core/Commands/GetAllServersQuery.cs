using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

namespace BytexDigital.RGSM.Panel.Client.Common.Core.Commands
{
    public class GetAllServersQuery : IRequest<GetAllServersQuery.Response>
    {
        public class Handler : IRequestHandler<GetAllServersQuery, Response>
        {
            private readonly NodesService _nodesService;
            private readonly ServerService _serversService;

            public Handler(NodesService nodesService, ServerService serversService)
            {
                _nodesService = nodesService;
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetAllServersQuery request, CancellationToken cancellationToken)
            {
                var nodes = await _nodesService.GetNodesAsync();
                var servers = new Dictionary<NodeDto, List<ServerDto>>();

                foreach (var node in nodes)
                {
                    servers.Add(node, new List<ServerDto>());

                    var availability = await _nodesService.IsNodeReachableAsync(node.BaseUri);

                    if (!availability.IsReachable) continue;

                    var nodeServers = await _serversService.GetServersAsync(node);

                    foreach (var server in nodeServers)
                    {
                        servers[node].Add(server);
                    }
                }

                return new Response
                {
                    Servers = servers
                };
            }
        }

        public class Response
        {
            public Dictionary<NodeDto, List<ServerDto>> Servers { get; set; }
        }
    }
}
