using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;

using MediatR;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands
{
    public class GetApplicationUsersQuery : IRequest<GetApplicationUsersQuery.Response>
    {
        public class Handler : IRequestHandler<GetApplicationUsersQuery, Response>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Response> Handle(GetApplicationUsersQuery request, CancellationToken cancellationToken)
            {

            }
        }

        public class Response
        {

        }
    }
}
