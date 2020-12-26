using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.RGSM.Shared;

using Microsoft.AspNetCore.Authorization;

namespace BytexDigital.RGSM.Node.Application.Core.Authorization.Requirements
{
    public class SystemAdministratorRequirement : IAuthorizationRequirement
    {
        public class Handler : AuthorizationHandler<SystemAdministratorRequirement>
        {
            private readonly HttpClient _httpClient;

            public Handler(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemAdministratorRequirement requirement)
            {
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
                    if (usersGroups.Any(x => x.GroupId == GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_ID))
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
