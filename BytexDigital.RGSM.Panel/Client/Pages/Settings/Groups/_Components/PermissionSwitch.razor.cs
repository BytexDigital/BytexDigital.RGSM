
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups._Components
{
    public partial class PermissionSwitch : ComponentBase
    {
        [Parameter]
        public PermissionDto Permission { get; set; }

        [Parameter]
        public EventCallback<bool> OnValueChanged { get; set; }

        [CascadingParameter]
        public Dictionary<PermissionDto, bool> Permissions { get; set; }

        public bool Value => Permissions[Permission];

        public string UiId { get; set; } = Guid.NewGuid().ToString();

        public async Task ValueChangedAsync(ChangeEventArgs args)
        {
            await OnValueChanged.InvokeAsync((bool)args.Value);
        }
    }
}
