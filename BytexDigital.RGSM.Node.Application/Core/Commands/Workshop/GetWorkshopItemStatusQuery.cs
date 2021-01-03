using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Workshop
{
    public class GetWorkshopItemStatusQuery : IRequest<GetWorkshopItemStatusQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetWorkshopItemStatusQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetWorkshopItemStatusQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                return new Response
                {
                    WorkshopItems = await workshopState.GetWorkshopModsAsync(cancellationToken)
                };
            }
        }

        public class Response
        {
            public List<WorkshopItem> WorkshopItems { get; set; }
        }
    }
}
