using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Authentication;
using BytexDigital.RGSM.Panel.Server.Common.Helpers;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace BytexDigital.RGSM.Panel.Server.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public LoginViewModel LoginViewModelData { get; set; }

        public LoginModel(IMediator mediator, IOptions<JwtBearerOptions> options)
        {
            _mediator = mediator;
        }

        public void OnGet(string returnUrl)
        {
            LoginViewModelData = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                await _mediator.Send(new LoginCmd { Username = LoginViewModelData.Username, Password = LoginViewModelData.Password });
            }
            catch (ServiceException ex)
            {
                ex.FailureDetails
                    .ForServiceErrors()
                    .ForField(nameof(LoginCmd.Username), x => ModelState.AddModelError(NameOf<LoginModel>.Property(p => p.LoginViewModelData.Username), x.Description))
                    .ForField(nameof(LoginCmd.Password), x => ModelState.AddModelError(NameOf<LoginModel>.Property(p => p.LoginViewModelData.Password), x.Description));

                return Page();
            }

            if (!string.IsNullOrWhiteSpace(LoginViewModelData.ReturnUrl))
            {
                return Redirect(LoginViewModelData.ReturnUrl);
            }
            else
            {
                return Redirect("/");
            }
        }

        public class LoginViewModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

            public string ReturnUrl { get; set; }
        }
    }
}
