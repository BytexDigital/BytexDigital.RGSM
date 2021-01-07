using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Groups.Commands
{
    public class GetGroupsQuery : IRequest<GetGroupsQuery.Response>
    {
        public class Handler : IRequestHandler<GetGroupsQuery, Response>
        {
            private readonly GroupService _groupsService;

            public Handler(GroupService groupsService)
            {
                _groupsService = groupsService;
            }

            public async Task<Response> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    Groups = await _groupsService.GetGroups().ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<Group> Groups { get; set; }
        }
    }
}
