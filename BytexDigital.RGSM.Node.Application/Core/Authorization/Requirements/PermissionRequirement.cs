using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

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
            private readonly HttpClient _httpClient;

            public Handler(PermissionsService permissionsService, ServersService serversService, HttpClient httpClient)
            {
                _permissionsService = permissionsService;
                _serversService = serversService;
                _httpClient = httpClient;
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
                    var usersGroups = await _httpClient.GetFromJsonAsync<List<ApplicationUserGroupDto>>($"/API/Groups/GetUsersGroups?userId={userId}");

                    // Users with this group id are considered sysadmins
                    if (usersGroups.Any(x => x.GroupId == "72056b80-0f35-4b5c-bdac-a143258c0e7c"))
                    {
                        context.Succeed(requirement);
                        return;
                    }

                    if (permission.GroupReferences.Select(x => x.GroupId).Intersect(usersGroups.Select(x => x.GroupId)).Any())
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
