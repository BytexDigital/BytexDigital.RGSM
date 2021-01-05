using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;
using BytexDigital.RGSM.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Authorization.Requirements
{

    public class SystemAdministratorRequirement : IAuthorizationRequirement
    {
        public class Handler : AuthorizationHandler<SystemAdministratorRequirement>
        {
            private readonly AccountService _accountsService;

            public Handler(AccountService accountsService)
            {
                _accountsService = accountsService;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SystemAdministratorRequirement requirement)
            {
                var user = await _accountsService.GetUser(context.User).FirstAsync();

                if (user.Groups.Any(x => x.Group.Name == GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
