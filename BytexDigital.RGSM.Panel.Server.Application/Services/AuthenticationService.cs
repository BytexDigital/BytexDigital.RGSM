using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace BytexDigital.RGSM.Panel.Server.Application.Services
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
