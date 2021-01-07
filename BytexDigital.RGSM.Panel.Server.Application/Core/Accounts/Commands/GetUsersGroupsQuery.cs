using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class GetUsersGroupsQuery : IRequest<GetUsersGroupsQuery.Response>
    {
        public string ApplicationUserId { get; set; }

        public class Handler : IRequestHandler<GetUsersGroupsQuery, Response>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Response> Handle(GetUsersGroupsQuery request, CancellationToken cancellationToken)
            {
                var user = await _accountService.GetApplicationUserById(request.ApplicationUserId).FirstOrDefaultAsync();

                if (user == null) throw ServiceException.ServiceError("User does not exist.").WithField(nameof(request.ApplicationUserId));

                return new Response
                {
                    Groups = await _accountService.GetAssignedGroups(user).ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<Group> Groups { get; set; }
        }
    }
}
