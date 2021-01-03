using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Entities;
using BytexDigital.RGSM.Shared.Enumerations;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class CreateServerCmd : IRequest<CreateServerCmd.Response>
    {
        public string DisplayName { get; set; }
        public string Directory { get; set; }
        public ServerType ServerType { get; set; }

        public class Handler : IRequestHandler<CreateServerCmd, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;
            private readonly IntegrityService _serverIntegrityService;
            private readonly ServersService _serversService;

            public Handler(ServerStateRegister serverStateRegister, IntegrityService serverIntegrityService, ServersService serversService)
            {
                _serverStateRegister = serverStateRegister;
                _serverIntegrityService = serverIntegrityService;
                _serversService = serversService;
            }

            public async Task<Response> Handle(CreateServerCmd request, CancellationToken cancellationToken)
            {
                var server = await (await _serversService.CreateServerAsync(request.DisplayName, request.Directory, request.ServerType)).FirstAsync();

                await _serverIntegrityService.EnsureCorrectSetupAsync(server);
                await _serverStateRegister.RegisterAsync(server);

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

                RuleFor(x => x.Directory)
                    .NotEmpty()
                    .Must(directory =>
                     {
                         if (System.IO.Directory.Exists(directory))
                         {
                             return System.IO.Directory.GetFiles(directory).Length == 0;
                         }
                         else
                         {
                             return false;
                         }
                     })
                    .WithMessage("Directory is not empty.");
            }
        }
    }
}
