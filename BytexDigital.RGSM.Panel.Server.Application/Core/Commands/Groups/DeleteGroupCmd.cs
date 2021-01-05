using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Shared;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Groups
{
    public class DeleteGroupCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteGroupCmd>
        {
            private readonly GroupService _groupsService;

            public Handler(GroupService groupsService)
            {
                _groupsService = groupsService;
            }

            public async Task<Unit> Handle(DeleteGroupCmd request, CancellationToken cancellationToken)
            {
                var group = await _groupsService.GetGroup(request.Id).FirstOrDefaultAsync();

                if (group == null) throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription("Group does not exist.");

                if (group.Id == GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_ID)
                {
                    throw new ServiceException().AddServiceError().WithField(nameof(request.Id)).WithDescription("Administrator group cannot be deleted.");
                }

                await _groupsService.DeleteGroupAsync(group);

                return Unit.Value;
            }
        }
    }
}
