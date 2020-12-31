using System.Collections.Generic;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Client.Extensions;
using BytexDigital.RGSM.Panel.Client.Pages.Settings._Components;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Nodes
{
    public partial class Index : SettingsComponentBase
    {
        [Inject] public NodeRegisterService NodesService { get; set; }
        [Inject] public IModalService ModalService { get; set; }


        public List<NodeDto> RegisteredNodes { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshNodesAsync();
        }

        public async Task RefreshNodesAsync()
        {
            RegisteredNodes = await NodesService.GetNodesAsync();
        }

        public async Task ShowRegisterNodeModalAsync()
        {
            var modalRef = ModalService.ShowFrontModal<_Components.RegisterNodeModal>(null);

            var result = await modalRef.Result;

            await RefreshNodesAsync();
            StateHasChanged();
        }
    }
}
