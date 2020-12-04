using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Domain.Entities;
using BytexDigital.RGSM.Domain.Enumerations;
using BytexDigital.RGSM.Node.Application.Shared.Services;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
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
            private readonly Shared.Services.NodeFileSystemService _nodeFileSystemService;
            private readonly NodeService _nodeService;
            private readonly IServiceProvider _serviceProvider;

            public Handler(Shared.Services.NodeFileSystemService nodeFileSystemService, NodeService nodeService, IServiceProvider serviceProvider)
            {
                _nodeFileSystemService = nodeFileSystemService;
                _nodeService = nodeService;
                _serviceProvider = serviceProvider;
            }

            public async Task<Response> Handle(CreateServerCmd request, CancellationToken cancellationToken)
            {
                if (!_nodeFileSystemService.IsDirectoryUsableForServer(request.Directory))
                {
                    throw new ServiceException()
                        .WithField(nameof(request.Directory))
                        .WithMessage($"The directory '{request.Directory}' is not usable for a server installation.");
                }

                var localNode = await (await _nodeService.GetLocalNodeAsync()).FirstAsync();

                var server = request.ServerType switch
                {
                    ServerType.Arma3 => await _serviceProvider
                                            .GetRequiredService<Games.Arma3.Services.CreationService>()
                                            .CreateServerAsync(localNode, request.DisplayName, request.Directory),

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
