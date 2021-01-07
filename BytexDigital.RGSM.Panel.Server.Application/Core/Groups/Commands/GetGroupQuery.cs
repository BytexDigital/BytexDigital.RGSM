using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Groups.Commands
{
    public class GetGroupQuery : IRequest<GetGroupQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetGroupQuery, Response>
        {
            private readonly GroupService _groupsService;

            public Handler(GroupService groupsService)
            {
                _groupsService = groupsService;
            }

            public async Task<Response> Handle(GetGroupQuery request, CancellationToken cancellationToken)
            {
                var group = await _groupsService.GetGroupById(request.Id).FirstOrDefaultAsync();

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
