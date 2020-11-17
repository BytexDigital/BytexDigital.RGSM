using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Client.Common.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings._Components
{
    [Authorize]
    public abstract class SettingsComponentBase : ComponentBase
    {
        [Inject]
        public IAuthorizationService AuthorizationService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var user = (await AuthenticationStateTask).User;

            if (!(await AuthorizationService.AuthorizeAsync(user, null, new UserGroupRequirement { GroupName = "system_administrator" })).Succeeded)
            {
                Navigation.NavigateTo("/unauthorized");
            }
        }
    }
}
