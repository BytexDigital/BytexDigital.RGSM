using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace BytexDigital.RGSM.Panel.Client.Common.Authorization
{

    public class ServerPermissionRequirement : IAuthorizationRequirement
    {
        public class Handler : AuthorizationHandler<ServerPermissionRequirement>
        {
            public Handler()
            {

            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ServerPermissionRequirement requirement)
            {

            }
        }
    }
}
