
using System.Collections.Generic;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Accounts
{
    public partial class Index : ComponentBase
    {
        [Inject]
        public AccountService AccountService { get; set; }

        public List<ApplicationUserDto> ApplicationUsers { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await RefreshAsync();
            StateHasChanged();
        }

        public async Task RefreshAsync()
        {
            ApplicationUsers = await AccountService.GetApplicationUsersAsync();
        }
    }
}
