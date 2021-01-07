using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class GetUserGroupLinksQuery : IRequest<GetUserGroupLinksQuery.Response>
    {
        public string ApplicationUserId { get; set; }

        public class Handler : IRequestHandler<GetUserGroupLinksQuery, Response>
        {
            private readonly AccountService _accountsService;

            public Handler(AccountService accountsService)
            {
                _accountsService = accountsService;
            }

            public async Task<Response> Handle(GetUserGroupLinksQuery request, CancellationToken cancellationToken)
            {
                var user = await _accountsService.GetUser(request.ApplicationUserId).FirstOrDefaultAsync();

                if (user == null) throw new ServiceException()
                        .AddServiceError().WithField(nameof(request.ApplicationUserId)).WithDescription("User not found.");

                return new Response
                {
                    GroupLinks = await _accountsService.GetAssignedGroupLinks(user).ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<ApplicationUserGroup> GroupLinks { get; set; }
        }
    }
}
