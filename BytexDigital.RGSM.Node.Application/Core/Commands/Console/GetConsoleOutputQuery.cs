using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.Console;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Console
{
    public class GetConsoleOutputQuery : IRequest<GetConsoleOutputQuery.Response>
    {
        public string Id { get; set; }
        public int LastNLines { get; set; }
        public List<string> Identifiers { get; set; }

        public class Handler : IRequestHandler<GetConsoleOutputQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetConsoleOutputQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IProvidesConsoleOutput consoleState) throw new ServerProvidesNoConsoleOutputException();

                return new Response
                {
                    Outputs = await consoleState.GetConsoleOutputAsync(request.Identifiers, request.LastNLines, cancellationToken)
                };
            }
        }

        public class Response
        {
            public List<ConsoleOutputContent> Outputs { get; set; }
        }
    }
}
