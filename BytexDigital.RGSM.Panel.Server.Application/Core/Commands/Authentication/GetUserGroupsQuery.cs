﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Authentication
{
    public class GetUserGroupsQuery : IRequest<GetUserGroupsQuery.Response>
    {
        public string UserId { get; set; }

        public class Handler : IRequestHandler<GetUserGroupsQuery, Response>
        {
            private readonly AccountService _accountsService;

            public Handler(AccountService accountsService)
            {
                _accountsService = accountsService;
            }

            public async Task<Response> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
            {
                var user = await _accountsService.GetUser(request.UserId).FirstOrDefaultAsync();

                if (user == null) throw new ServiceException()
                        .AddServiceError().WithField(nameof(request.UserId)).WithDescription("User not found.");

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
