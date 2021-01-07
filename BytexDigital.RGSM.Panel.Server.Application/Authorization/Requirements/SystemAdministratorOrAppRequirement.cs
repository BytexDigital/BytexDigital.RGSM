using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Authorization.Requirements
{

    public class SystemAdministratorOrAppRequirement : IAuthorizationRequirement
    {
        public class Handler : AuthorizationHandler<SystemAdministratorOrAppRequirement>
        {
            private readonly AccountService _accountsService;

            public Handler(AccountService accountsService)
            {
                _accountsService = accountsService;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemAdministratorOrAppRequirement requirement)
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    return;
                }

                var scopes = context.User.Claims.Where(x => x.Type == "scope").Select(x => x.Value);

                if (scopes.Contains("rgsm.app"))
                {
                    context.Succeed(requirement);
                    return;
                }

                var user = await _accountsService.GetUser(context.User).FirstOrDefaultAsync();

                if (user == null) return;

                if (user.Groups.Any(x => x.Group.Name == GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
