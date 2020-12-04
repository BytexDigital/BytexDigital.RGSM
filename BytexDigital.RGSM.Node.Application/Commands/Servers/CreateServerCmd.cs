using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Domain.Enumerations;
using BytexDigital.RGSM.Node.Application.Shared.Services;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace BytexDigital.RGSM.Node.Application.Commands.Servers
{

    public class CreateServerCmd : IRequest<CreateServerCmd.Response>
    {
        public ServerType ServerType { get; set; }
        public string DisplayName { get; set; }
        public string Directory { get; set; }

        public class Handler : IRequestHandler<CreateServerCmd, Response>
        {
            private readonly NodeServerService _nodeServerService;
            private readonly Shared.Services.NodeFileSystemService _nodeFileSystemService;
            private readonly IServiceProvider _serviceProvider;

            public Handler(NodeServerService nodeServerService, Shared.Services.NodeFileSystemService nodeFileSystemService, IServiceProvider serviceProvider)
            {
                _nodeServerService = nodeServerService;
                _nodeFileSystemService = nodeFileSystemService;
                _serviceProvider = serviceProvider;
            }

            public async Task<Response> Handle(CreateServerCmd request, CancellationToken cancellationToken)
            {
                if (!_nodeFileSystemService.IsDirectoryUsableForServer(request.Directory))
                {
                    throw new ServiceException().WithField(request.Directory).WithMessage($"The directory '{request.Directory}' is not usable for a server installation.");
                }

                var server = request.ServerType switch
                {
                    ServerType.Arma3 => await _serviceProvider.GetRequiredService<Games.Arma3.Services.CreationService>().CreateServerAsync(request.DisplayName, request.Directory),
                    ServerType.DayZ => throw new NotImplementedException(),
                    _ => throw new NotImplementedException()
                };

                return new Response { Server = server };
            }
        }

        public class Response
        {
            public Server Server { get; set; }
        }

        public class Validator : AbstractValidator<CreateServerCmd>
        {
            public Validator()
            {
                RuleFor(x => x.Directory)
                    .NotNull()
                    .NotEmpty();

                RuleFor(x => x.DisplayName)
                    .NotNull()
                    .NotEmpty();
            }
        }
    }
}
