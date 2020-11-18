using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Application.Exceptions;
using BytexDigital.RGSM.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace BytexDigital.RGSM.Panel.Server.Application.Commands.Authentication
{

    public class LoginCmd : IRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public class Handler : IRequestHandler<LoginCmd>
        {
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly UserManager<ApplicationUser> _userManager;

            public Handler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
            {
                _signInManager = signInManager;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(LoginCmd request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user == null) throw new ServiceValidationException(nameof(request.Username), "This user does not exist.");

                var response = await _signInManager.PasswordSignInAsync(user, request.Password, true, true);

                if (response.Succeeded) return Unit.Value;

                if (response.IsLockedOut) throw new ServiceValidationException(nameof(request.Username), "This user is locked out.");
                if (response.IsNotAllowed) throw new ServiceValidationException(nameof(request.Password), "This user is not allowed to sign in.");

                throw new ServiceValidationException(nameof(request.Password), "This provided credentials are invalid.");
            }
        }
    }
}
