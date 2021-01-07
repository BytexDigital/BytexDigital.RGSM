using System.Threading;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Accounts.Commands
{
    public class CreateUserCmd : IRequest<CreateUserCmd.Response>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }

        public class Handler : IRequestHandler<CreateUserCmd, Response>
        {
            private readonly AccountService _accountService;

            public Handler(AccountService accountService)
            {
                _accountService = accountService;
            }

            public async Task<Response> Handle(CreateUserCmd request, CancellationToken cancellationToken)
            {
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

                var user = await (await _accountService.CreateApplicationUserAsync(request.UserName, request.Password)).FirstAsync();

                return new Response
                {
                    ApplicationUser = user
                };
            }
        }

        public class Response
        {
            public ApplicationUser ApplicationUser { get; set; }
        }

        public class Validator : AbstractValidator<CreateUserCmd>
        {
            public Validator(AccountService accountService)
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()

                    .DependentRules(() =>
                    {
                        RuleFor(x => x.UserName)
                            .MustAsync(async (name, cancelToken) =>
                            {
                                return !await accountService.GetApplicationUserByUserName(name).AnyAsync();
                            })
                            .WithMessage("Username is already taken.");
                    });

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
