using System.Collections.Generic;
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
            private readonly ServersService _serversService;

            public Handler(NodesService nodesService, ServersService serversService)
            {
                _nodesService = nodesService;
                _serversService = serversService;
            }

            public async Task<Response> Handle(GetAllServersQuery request, CancellationToken cancellationToken)
            {
                var nodes = await _nodesService.GetNodesAsync();
                var servers = new List<(NodeDto Node, ServerDto Server)>();

                foreach (var node in nodes)
                {
                    var availability = await _nodesService.IsNodeReachableAsync(node.BaseUri);

                    if (!availability.IsReachable) continue;

                    var nodeServers = await _serversService.GetServersAsync(node);

                    foreach (var server in nodeServers)
                    {
                        servers.Add((node, server));
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
            public List<(NodeDto Node, ServerDto Server)> Servers { get; set; }
        }
    }
}
