using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Client.Common.Authorization;
using BytexDigital.RGSM.Shared;

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

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            if (!(await AuthorizationService.AuthorizeAsync(
                authenticationState.User,
                null,
                new UserGroupRequirement { GroupName = GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME })).Succeeded)
            {
                Navigation.NavigateTo("/unauthorized");
            }
        }
    }
}
