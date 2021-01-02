
using System.Collections.Generic;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups._Components
{
    public partial class ServerPermissions : ComponentBase
    {
        [Parameter]
        public NodeDto Node { get; set; }

        [Parameter]
        public ServerDto Server { get; set; }

        [Inject]
        public PermissionsService PermissionsService { get; set; }

        public List<PermissionDto> Permissions { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var result = await ServiceResult.FromAllAsync(async () => await PermissionsService.GetPermissionsAsync(Node, Server));

            if (!result.Succeeded)
            {
                return;
            }

            Permissions = result;
            StateHasChanged();
        }
    }
}
