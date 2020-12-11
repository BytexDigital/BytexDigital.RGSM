using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Nodes
{

    public class GetNodeKeyQuery : IRequest<GetNodeKeyQuery.Response>
    {
        public string NodeId { get; set; }

        public class Handler : IRequestHandler<GetNodeKeyQuery, Response>
        {
            private readonly NodesService _nodesService;

            public Handler(NodesService nodesService)
            {
                _nodesService = nodesService;
            }

            public async Task<Response> Handle(GetNodeKeyQuery request, CancellationToken cancellationToken)
            {
                return new Response { NodeKey = await _nodesService.GetNodeKey(request.NodeId).FirstAsync() };
            }
        }

        public class Response
        {
            public ApiKey NodeKey { get; set; }
        }
    }
}
