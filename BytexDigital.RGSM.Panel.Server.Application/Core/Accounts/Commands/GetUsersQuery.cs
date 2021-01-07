﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Application.Core.Accounts;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class GetUsersQuery : IRequest<GetUsersQuery.Response>
    {
        public class Handler : IRequestHandler<GetUsersQuery, Response>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Response> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    ApplicationUsers = await _accountService.GetApplicationUsers().ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<ApplicationUser> ApplicationUsers { get; set; }
        }
    }
}