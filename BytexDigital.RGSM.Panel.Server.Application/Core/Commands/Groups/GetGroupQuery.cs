using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Groups
{
    public class GetGroupQuery : IRequest<GetGroupQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetGroupQuery, Response>
        {
            private readonly GroupsService _groupsService;

            public Handler(GroupsService groupsService)
            {
                _groupsService = groupsService;
            }

            public async Task<Response> Handle(GetGroupQuery request, CancellationToken cancellationToken)
            {
                var group = await _groupsService.GetGroup(request.Id).FirstOrDefaultAsync();

                if (group == null) throw ServiceException.ServiceError("Group not found.").WithField(nameof(request.Id));

                return new Response
                {
                    Group = group
                };
            }
        }

        public class Response
        {
            public Group Group { get; set; }
        }
    }
}
