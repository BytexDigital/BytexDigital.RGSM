using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Client.Common.Core.Master;
using BytexDigital.RGSM.Panel.Client.Pages.Settings._Components;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Nodes
{
    public partial class Index : SettingsComponentBase
    {
        [Inject]
        public NodesService NodesService { get; set; }

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
    }
}
