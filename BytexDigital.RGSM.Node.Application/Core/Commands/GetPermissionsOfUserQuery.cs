using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{
    public class GetPermissionsOfUserQuery : IRequest<GetPermissionsOfUserQuery.Response>
    {
        public string ServerId { get; set; }
        public string UserId { get; set; }

        public class Handler : IRequestHandler<GetPermissionsOfUserQuery, Response>
        {
            private readonly ServersService _serversService;
            private readonly PermissionsService _permissionsService;
            private readonly MasterApiService _masterApiService;

            public Handler(ServersService serversService, PermissionsService permissionsService, MasterApiService masterApiService)
            {
                _serversService = serversService;
                _permissionsService = permissionsService;
                _masterApiService = masterApiService;
            }

            public async Task<Response> Handle(GetPermissionsOfUserQuery request, CancellationToken cancellationToken)
            {
                var groupsResult = await _masterApiService.GetGroupsOfUserAsync(request.UserId);
                var server = await _serversService.GetServer(request.ServerId).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();
                if (!groupsResult.Succeeded) throw new ServiceException().WithField(nameof(request.UserId)).WithMessage("User's groups could not be fetched.");

                List<Permission> permissions = new List<Permission>();

                if (groupsResult.Result.Any(x => x.GroupId == MasterApiService.SYSTEM_ADMINISTRATOR_GROUP_ID))
                {
                    permissions = await _permissionsService.GetPermissionsOfGroup(MasterApiService.SYSTEM_ADMINISTRATOR_GROUP_ID).ToListAsync();
                }
                else
                {
                    foreach (var group in groupsResult.Result)
                    {
                        permissions.AddRange(await _permissionsService.GetPermissionsOfGroup(group.GroupId).ToListAsync());
                    }
                }

                return new Response
                {
                    Permissions = permissions
                };
            }
        }

        public class Response
        {
            public List<Permission> Permissions { get; set; }
        }
    }
}
