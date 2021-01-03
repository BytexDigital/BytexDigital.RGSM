using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class GetWorkshopModsQuery : IRequest<GetWorkshopModsQuery.Response>
    {
        public string ServerId { get; set; }

        public class Handler : IRequestHandler<GetWorkshopModsQuery, Response>
        {
            private readonly ServersService _serversService;
            private readonly ServerStateRegister _serverStateRegister;
            private readonly WorkshopManagerService _workshopService;

            public Handler(ServersService serversService, ServerStateRegister serverStateRegister, WorkshopManagerService workshopService)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
                _workshopService = workshopService;
            }

            public async Task<Response> Handle(GetWorkshopModsQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.ServerId);
                var server = await _serversService.GetServer(request.ServerId).FirstOrDefaultAsync();

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                var mods = await _workshopService.GetTrackedWorkshopMods(server).ToListAsync();

                return new Response
                {
                    TrackedWorkshopMods = mods
                };
            }
        }

        public class Response
        {
            public List<TrackedWorkshopMod> TrackedWorkshopMods { get; set; }
        }
    }
}
