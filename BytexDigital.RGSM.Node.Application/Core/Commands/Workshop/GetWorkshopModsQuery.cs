using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Workshop
{
    public class GetWorkshopModsQuery : IRequest<GetWorkshopModsQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetWorkshopModsQuery, Response>
        {
            private readonly ServersService _serversService;
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServersService serversService, ServerStateRegister serverStateRegister)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetWorkshopModsQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                var mods = await _serversService.GetTrackedWorkshopMods(server).ToListAsync();

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
