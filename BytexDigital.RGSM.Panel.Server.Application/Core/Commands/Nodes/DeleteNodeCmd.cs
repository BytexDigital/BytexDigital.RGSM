using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Nodes
{
    public class DeleteNodeCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteNodeCmd>
        {
            private readonly NodesService _nodesService;

            public Handler(NodesService nodesService)
            {
                _nodesService = nodesService;
            }

            public async Task<Unit> Handle(DeleteNodeCmd request, CancellationToken cancellationToken)
            {
                var node = await _nodesService.GetNode(request.Id).FirstAsync();

                await _nodesService.UnregisterNodeAsync(node);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<DeleteNodeCmd>
        {
            public Validator(NodesService nodesService)
            {
                RuleFor(x => x.Id)
                    .MustAsync(async (id, cancelToken) => await nodesService.GetNode(id).AnyAsync())
                    .WithMessage("Node does not exist.");
            }
        }
    }
}
