using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Node.Domain.Enumerations;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{

    public class CreateServerCmd : IRequest<CreateServerCmd.Response>
    {
        public string DisplayName { get; set; }
        public ServerType ServerType { get; set; }

        public class Handler : IRequestHandler<CreateServerCmd, Response>
        {
            private readonly ServersService _serversService;

            public Handler(ServersService serversService)
            {
                _serversService = serversService;
            }

            public async Task<Response> Handle(CreateServerCmd request, CancellationToken cancellationToken)
            {
                var server = await (await _serversService.CreateServerAsync(request.DisplayName, request.ServerType)).FirstAsync();

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
                RuleFor(x => x.DisplayName)
                    .NotEmpty();
            }
        }
    }
}
