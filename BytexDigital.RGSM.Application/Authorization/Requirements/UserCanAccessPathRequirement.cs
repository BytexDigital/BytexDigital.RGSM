using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace BytexDigital.RGSM.Application.Authorization.Requirements
{

    public class UserCanAccessPathRequirement : IAuthorizationRequirement
    {
        public class Handler : AuthorizationHandler<UserCanAccessPathRequirement>
        {
            public Handler()
            {

            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserCanAccessPathRequirement requirement)
            {

            }
        }
    }
}
