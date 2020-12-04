using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Authorization.Requirements
{

    public class SystemAdministratorRequirement : IAuthorizationRequirement
    {
        public class Handler : AuthorizationHandler<SystemAdministratorRequirement>
        {
            private readonly AccountsService _accountsService;

            public Handler(AccountsService accountsService)
            {
                _accountsService = accountsService;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemAdministratorRequirement requirement)
            {
                var user = await _accountsService.GetUser(context.User).FirstAsync();

                if (user.Groups.Any(x => x.Group.Name == Group.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
