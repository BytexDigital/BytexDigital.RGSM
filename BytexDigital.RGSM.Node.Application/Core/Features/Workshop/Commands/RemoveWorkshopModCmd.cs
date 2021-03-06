﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Servers;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.Steam.Core.Structs;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Features.Workshop.Commands
{
    public class RemoveWorkshopModCmd : IRequest
    {
        public string Id { get; set; }
        public PublishedFileId PublishedFileId { get; set; }

        public class Handler : IRequestHandler<RemoveWorkshopModCmd>
        {
            private readonly ServerService _serversService;
            private readonly ServerStateRegister _serverStateRegister;
            private readonly WorkshopManagerService _workshopManagerService;

            public Handler(ServerService serversService, ServerStateRegister serverStateRegister, WorkshopManagerService workshopManagerService)
            {
                _serversService = serversService;
                _serverStateRegister = serverStateRegister;
                _workshopManagerService = workshopManagerService;
            }

            public async Task<Unit> Handle(RemoveWorkshopModCmd request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (state == null) throw new ServerNotFoundException();
                if (state is not IWorkshopSupport workshopState) throw new ServerDoesNotSupportFeatureException<IWorkshopSupport>();

                if (!server.TrackedWorkshopMods.Any(x => x.PublishedFileId == request.PublishedFileId))
                    throw new ServiceException().AddServiceError().WithField(nameof(request.PublishedFileId)).WithDescription("Workshop item is already removed.");

                await _workshopManagerService.RemoveTrackedWorkshopItemAsync(server, request.PublishedFileId);

                return Unit.Value;
            }
        }
    }
}
