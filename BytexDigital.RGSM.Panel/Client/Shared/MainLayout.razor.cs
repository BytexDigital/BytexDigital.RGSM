using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BytexDigital.RGSM.Panel.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        public SignOutSessionStateManager SignOutManager { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            AuthenticationState = await AuthenticationStateTask;
        }

        private async Task SignoutAsync()
        {
            await SignOutManager.SetSignOutState();
            Navigation.NavigateTo("authentication/logout");
        }
    }
}
