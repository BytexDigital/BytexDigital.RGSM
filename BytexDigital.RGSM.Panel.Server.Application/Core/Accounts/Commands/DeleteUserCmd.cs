using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class DeleteUserCmd : IRequest
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<DeleteUserCmd>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Unit> Handle(DeleteUserCmd request, CancellationToken cancellationToken)
            {
                var user = await _accountService.GetApplicationUserById(request.Id).FirstOrDefaultAsync();

                if (user == null) throw ServiceException.ServiceError("User does not exist.").WithField(nameof(request.Id));

                await _accountService.DeleteApplicationUserAsync(user);

                return Unit.Value;
            }
        }
    }
}
