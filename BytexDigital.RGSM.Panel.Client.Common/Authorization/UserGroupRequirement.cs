using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Client.Common.Core.Master;

using Microsoft.AspNetCore.Authorization;

namespace BytexDigital.RGSM.Panel.Client.Common.Authorization
{
    public class UserGroupRequirement : IAuthorizationRequirement
    {
        public string GroupName { get; set; }

        public class Handler : AuthorizationHandler<UserGroupRequirement>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserGroupRequirement requirement)
            {
                var groups = await _accountService.GetAssignedGroupsAsync();

                if (groups.Any(x => x.Name.ToLowerInvariant() == requirement.GroupName.ToLowerInvariant()))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
