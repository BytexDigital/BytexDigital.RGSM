using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Nodes.Commands
{
    public class GetAllNodesQuery : IRequest<GetAllNodesQuery.Response>
    {
        public class Handler : IRequestHandler<GetAllNodesQuery, Response>
        {
            private readonly NodeService _nodeService;

            public Handler(NodeService nodeService)
            {
                _nodeService = nodeService;
            }

            public async Task<Response> Handle(GetAllNodesQuery request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    Nodes = await _nodeService.GetNodes().ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<Node> Nodes { get; set; }
        }
    }
}
