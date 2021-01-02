
using System;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups._Components
{
    public partial class PermissionSwitch : ComponentBase
    {
        [Parameter]
        public PermissionDto Permission { get; set; }

        [Parameter]
        public EventCallback<bool> OnValueChanged { get; set; }

        public string UiId { get; set; } = Guid.NewGuid().ToString();

        public async Task ValueChangedAsync(ChangeEventArgs args)
        {
            await OnValueChanged.InvokeAsync((bool)args.Value);
        }
    }
}
