using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Application.Core.Groups;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class UpdateUserGroupAffinityCmd : IRequest
    {
        public string ApplicationUserId { get; set; }
        public string GroupId { get; set; }
        public bool IsInGroup { get; set; }

        public class Handler : IRequestHandler<UpdateUserGroupAffinityCmd>
        {
            private readonly AccountService _accountService;
            private readonly GroupService _groupService;

            public Handler(AccountService accountService, GroupService groupService)
            {
                _accountService = accountService;
                _groupService = groupService;
            }

            public async Task<Unit> Handle(UpdateUserGroupAffinityCmd request, CancellationToken cancellationToken)
            {
                var user = await _accountService.GetApplicationUserById(request.ApplicationUserId).FirstOrDefaultAsync();
                var group = await _groupService.GetGroupById(request.GroupId).FirstOrDefaultAsync();

                if (user == null) throw ServiceException.ServiceError("User does not exist.").WithField(nameof(request.ApplicationUserId));
                if (group == null) throw ServiceException.ServiceError("Group does not exist.").WithField(nameof(request.GroupId));

                await _accountService.UpdateApplicationUserGroupAffinityAsync(user, group, request.IsInGroup);

                return Unit.Value;
            }
        }
    }
}
