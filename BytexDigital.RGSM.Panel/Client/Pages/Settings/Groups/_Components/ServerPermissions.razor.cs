
using System.Collections.Generic;
using System.Linq;
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

        [Parameter]
        public GroupDto Group { get; set; }

        [Inject]
        public PermissionService PermissionsService { get; set; }

        [Inject]
        public ToastService ToastsService { get; set; }

        public Dictionary<PermissionDto, bool> Permissions { get; set; }

        public bool IsChanged { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await RefreshAsync();
            StateHasChanged();
        }

        public async Task RefreshAsync()
        {
            var result = await ServiceResult.FromAllAsync(async () => await PermissionsService.GetPermissionsAsync(Node, Server));

            if (!result.Succeeded)
            {
                return;
            }

            Permissions = result.Result.ToDictionary(x => x, x => x.GroupReferences.Any(gr => gr.GroupId == Group.Id));
        }

        public void OnPermissionValueChanged(string name, bool newValue)
        {
            var permission = Permissions.Keys.FirstOrDefault(x => x.Name == name);

            Permissions[permission] = newValue;

            IsChanged = true;

            StateHasChanged();
        }

        public async Task RevertChangesAsync()
        {
            await RefreshAsync();
            IsChanged = false;
            StateHasChanged();
        }

        public async Task SaveChangesAsync()
        {
            foreach (var permission in Permissions)
            {
                await PermissionsService.SavePermissionAsync(Node, Server, Group, permission.Key, permission.Value);
            }

            await ToastsService.NotifyAsync(WebNotificationType.Success, $"Saved changes for server {Server.DisplayName}.");

            IsChanged = false;
            StateHasChanged();
        }
    }
}
