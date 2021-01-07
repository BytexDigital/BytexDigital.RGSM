using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class UpdateUserCmd : IRequest
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public class Handler : IRequestHandler<UpdateUserCmd>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Unit> Handle(UpdateUserCmd request, CancellationToken cancellationToken)
            {
                var user = await _accountService.GetApplicationUserByUserName(request.UserName).FirstOrDefaultAsync();

                if (user == null) throw ServiceException.ServiceError("User does not exist.").WithField(nameof(request.Id));

                await _accountService.UpdateApplicationUserAsync(user, request.UserName);

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<UpdateUserCmd>
        {
            public Validator(AccountService accountService)
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()

                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UserName)
                            .MustAsync(async (model, name, cancelToken) => (await accountService.GetApplicationUserByUserName(name).Where(x => x.Id == model.Id).AnyAsync()))
                            .WithMessage("Username is already taken.");
                    });
            }
        }
    }
}
