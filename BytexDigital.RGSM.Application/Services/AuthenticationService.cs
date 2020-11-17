using BytexDigital.RGSM.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace BytexDigital.RGSM.Application.Services
{
    public class AuthenticationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticationService(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
    }
}
