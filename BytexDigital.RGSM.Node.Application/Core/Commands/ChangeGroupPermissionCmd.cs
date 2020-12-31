using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Exceptions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class ChangeGroupPermissionCmd : IRequest
    {
        public string Id { get; set; }
        public bool AddOrRemove { get; set; }
        public string Name { get; set; }
        public string GroupId { get; set; }

        public class Handler : IRequestHandler<ChangeGroupPermissionCmd>
        {
            private readonly ServersService _serversService;
            private readonly PermissionsService _permissionsService;

            public Handler(ServersService serversService, PermissionsService permissionsService)
            {
                _serversService = serversService;
                _permissionsService = permissionsService;
            }

            public async Task<Unit> Handle(ChangeGroupPermissionCmd request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.Id).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                var permission = await _permissionsService.GetPermission(server, request.Name).FirstOrDefaultAsync();

                if (permission == null) throw new ServiceException().AddServiceError().WithField(nameof(request.Name)).WithDescription("Permission not found.");

                await _permissionsService.AddOrRemoveGroupFromPermissionAsync(permission, request.AddOrRemove, request.GroupId);

                return Unit.Value;
            }
        }
    }
}
