using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Nodes
{

    public class UpdateNodeCmd : IRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string BaseUri { get; set; }

        public class Handler : IRequestHandler<UpdateNodeCmd>
        {
            private readonly NodesService _nodesService;

            public Handler(NodesService nodesService)
            {
                _nodesService = nodesService;
            }

            public async Task<Unit> Handle(UpdateNodeCmd request, CancellationToken cancellationToken)
            {
                var node = await _nodesService.GetNode(request.Id).FirstAsync();

                await _nodesService.UpdateNodeAsync(
                    node,
                    request.Name ?? node.Name,
                    request.DisplayName ?? node.DisplayName,
                    request.BaseUri ?? node.BaseUri);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<UpdateNodeCmd>
        {
            public Validator(NodesService nodesService)
            {
                RuleFor(x => x.Id)
                    .MustAsync(async (id, cancelToken) => await nodesService.GetNode(id).AnyAsync())
                    .WithMessage("Node does not exist.");

                When(x => x.BaseUri != null, () =>
                {
                    RuleFor(x => x.BaseUri)
                        .Must((instance, uri) =>
                        {
                            Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri);

                            return parsedUri != null;
                        })
                        .WithMessage("The base uri must be a valid absolute uri.");
                });

                When(x => x.Name != null, () =>
                {
                    RuleFor(x => x.Name)
                        .NotEmpty()
                        .MustAsync(async (request, name, cancelToken) =>
                        {
                            return !await nodesService.GetNodeByName(name).Where(x => x.Id != request.Id).AnyAsync();
                        })
                        .WithMessage("Name is not unique.");
                });

                When(x => x.DisplayName != null, () =>
                {
                    RuleFor(x => x.DisplayName)
                        .NotEmpty();
                });
            }
        }
    }
}
