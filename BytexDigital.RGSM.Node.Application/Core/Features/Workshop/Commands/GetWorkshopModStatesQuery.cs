using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class GetWorkshopModStatesQuery : IRequest<GetWorkshopModStatesQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetWorkshopModStatesQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetWorkshopModStatesQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                return new Response
                {
                    WorkshopModStates = await workshopState.GetWorkshopModStatesAsync(cancellationToken)
                };
            }
        }

        public class Response
        {
            public List<WorkshopModState> WorkshopModStates { get; set; }
        }
    }
}
