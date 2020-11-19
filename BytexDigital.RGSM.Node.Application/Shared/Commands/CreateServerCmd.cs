using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Domain.Enumerations;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Shared.Commands
{

    public class CreateServerCmd : IRequest<CreateServerCmd.Response>
    {
        public ServerType ServerType { get; set; }

        public class Handler : IRequestHandler<CreateServerCmd, Response>
        {
            public Handler()
            {

            }

            public async Task<Response> Handle(CreateServerCmd request, CancellationToken cancellationToken)
            {

            }
        }

        public class Response
        {

        }
    }
}
