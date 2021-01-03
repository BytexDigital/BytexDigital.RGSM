using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.BattlEye.Commands
{
    public class GetRconPlayersQuery : IRequest<GetRconPlayersQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetRconPlayersQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetRconPlayersQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IBattlEyeRcon beRconState) throw new ServerDoesNotSupportFeatureException<IBattlEyeRcon>();

                return new Response
                {
                    Players = await beRconState.GetBeRconPlayersAsync(cancellationToken)
                };
            }
        }

        public class Response
        {
            public List<BeRconPlayer> Players { get; set; }
        }
    }
}
