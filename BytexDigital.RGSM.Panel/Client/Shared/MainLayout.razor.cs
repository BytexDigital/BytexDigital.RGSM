
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Client.Common.Authorization;
using BytexDigital.RGSM.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BytexDigital.RGSM.Panel.Client.Shared
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        public IAuthorizationService AuthorizationService { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public bool IsAdmin { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var user = await AuthenticationStateProvider.GetAuthenticationStateAsync();

            IsAdmin = (await AuthorizationService.AuthorizeAsync(
                user.User,
                null,
                new UserGroupRequirement { GroupName = GroupsConstants.DEFAULT_SYSTEM_ADMINISTRATOR_GROUP_NAME })).Succeeded;
        }
    }
}
