using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.BattlEye
{
    public class GetRconMessagesQuery : IRequest<GetRconMessagesQuery.Response>
    {
        public string Id { get; set; }
        public int Limit { get; set; }

        public class Handler : IRequestHandler<GetRconMessagesQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetRconMessagesQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IBattlEyeRcon beRconState) throw new ServerDoesNotSupportFeatureException<IBattlEyeRcon>();

                return new Response
                {
                    Messages = await beRconState.GetBeRconMessagesAsync(request.Limit, cancellationToken)
                };
            }
        }

        public class Response
        {
            public List<BeRconMessage> Messages { get; set; }
        }
    }
}
