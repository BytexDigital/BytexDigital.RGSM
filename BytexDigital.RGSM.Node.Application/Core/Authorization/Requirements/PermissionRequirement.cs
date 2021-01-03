using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Core.Infrastructure;
using BytexDigital.RGSM.Node.Application.Core.Servers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements
{

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string ServerId { get; set; }
        public string Name { get; set; }

        public class Handler : AuthorizationHandler<PermissionRequirement>
        {
            private readonly PermissionsService _permissionsService;
            private readonly ServersService _serversService;
            private readonly MasterApiService _masterApiService;

            public Handler(PermissionsService permissionsService, ServersService serversService, MasterApiService masterApiService)
            {
                _permissionsService = permissionsService;
                _serversService = serversService;
                _masterApiService = masterApiService;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
            {
                var server = await _serversService.GetServer(requirement.ServerId).FirstOrDefaultAsync();

                if (server == null) return;

                var permission = await _permissionsService.GetPermission(server, requirement.Name).FirstOrDefaultAsync();

                if (permission == null) return;

                if (context.User.HasClaim(x => x.Type == "scope" && (x.Value == "rgsm.app" || x.Value == "rgsm.node")))
                {
                    context.Succeed(requirement);
                    return;
                }

                try
                {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var usersGroupsResult = await ServiceResult.FromAsync(async () => await _masterApiService.GetGroupsOfUserAsync(userId));


                    // Users with this group id are considered sysadmins
                    if (usersGroupsResult.Result.Any(x => x.GroupId == MasterApiService.SYSTEM_ADMINISTRATOR_GROUP_ID))
                    {
                        context.Succeed(requirement);
                        return;
                    }

                    if (permission.GroupReferences.Select(x => x.GroupId).Intersect(usersGroupsResult.Result.Select(x => x.GroupId)).Any())
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
                catch
                {

                }
            }
        }
    }
}
