using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Nodes.Commands
{

    public class RegisterNodeCmd : IRequest<RegisterNodeCmd.Response>
    {
        public string BaseUri { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public class Handler : IRequestHandler<RegisterNodeCmd, Response>
        {
            private readonly NodeService _nodesService;

            public Handler(NodeService nodesService)
            {
                _nodesService = nodesService;
            }

            public async Task<Response> Handle(RegisterNodeCmd request, CancellationToken cancellationToken)
            {
                if (await _nodesService.GetNodeByName(request.Name).AnyAsync())
                    throw new ServiceException().AddServiceError().WithField(nameof(request.Name)).WithDescription("Name is not unique.");

                var node = await (await _nodesService.RegisterNodeAsync(request.BaseUri, request.Name, request.DisplayName)).FirstAsync();

                return new Response { Node = node };
            }
        }

        public class Response
        {
            public Domain.Entities.Node Node { get; set; }
        }

        public class Validator : AbstractValidator<RegisterNodeCmd>
        {
            public Validator()
            {
                RuleFor(x => x.BaseUri)
                    .Must((instance, uri) =>
                    {
                        Uri.TryCreate(uri, UriKind.Absolute, out var parsedUri);

                        return parsedUri != null;
                    })
                    .WithMessage("The base uri must be a valid absolute uri.");

                RuleFor(x => x.Name)
                    .NotEmpty();

                RuleFor(x => x.DisplayName)
                    .NotEmpty();
            }
        }
    }
}
