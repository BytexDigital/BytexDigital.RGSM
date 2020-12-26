using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared.Exceptions;
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
                var nodeKey = await _nodesService.GetNodeKey(request.NodeId).FirstAsync();

                if (nodeKey == null) throw new ServiceException().WithField(nameof(request.NodeId)).WithMessage("Node has no API key or does not exist.");

                return new Response { NodeKey = nodeKey };
            }
        }

        public class Response
        {
            public ApiKey NodeKey { get; set; }
        }
    }
}
