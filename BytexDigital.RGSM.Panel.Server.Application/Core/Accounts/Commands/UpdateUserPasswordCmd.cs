using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class UpdateUserPasswordCmd : IRequest
    {
        public string ApplicationUserId { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }

        public class Handler : IRequestHandler<UpdateUserPasswordCmd>
        {
            private readonly AccountService _accountService;
            private readonly UserManager<ApplicationUser> _userManager;

            public Handler(AccountService accountService, UserManager<ApplicationUser> userManager)
            {
                _accountService = accountService;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(UpdateUserPasswordCmd request, CancellationToken cancellationToken)
            {
                var user = await _accountService.GetApplicationUserById(request.ApplicationUserId).FirstOrDefaultAsync();

                if (user == null) throw ServiceException.ServiceError("User not found.").WithField(nameof(request.ApplicationUserId));

                var errors = await _accountService.CheckPasswordForRequirementErrorsAsync(request.Password);

                if (errors.Count > 0)
                {
                    var exceptionBuilder = new ServiceExceptionBuilder();

                    foreach (var error in errors)
                    {
                        exceptionBuilder.AddServiceError().WithField(nameof(request.Password)).WithDescription(error);
                    }

                    throw exceptionBuilder;
                }


                if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
                {
                    throw ServiceException.ServiceError("Current password is invalid.").WithField(nameof(request.CurrentPassword));
                }

                var changeResult = await _accountService.UpdatePasswordAsync(user, request.CurrentPassword, request.Password);

                if (changeResult.Errors.Count() > 0)
                {
                    var exceptionBuilder = new ServiceExceptionBuilder();

                    changeResult.Errors.ToList().ForEach(x => exceptionBuilder.AddServiceError().WithDescription(x.Description));

                    throw exceptionBuilder;
                }
            }
        }

        public class Validator : AbstractValidator<UpdateUserPasswordCmd>
        {
            public Validator(AccountService accountService)
            {
                RuleFor(x => x.Password)
                    .NotEmpty();

                RuleFor(x => x.PasswordRepeat)
                    .NotEmpty()

                    .DependentRules(() =>
                    {
                        RuleFor(x => x.PasswordRepeat)
                            .Matches(x => x.Password)
                            .WithMessage("Password repeat must match password.");
                    });
            }
        }
    }
}
