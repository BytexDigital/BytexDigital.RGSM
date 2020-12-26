
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using BytexDigital.Blazor.Components.FormValidators;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Client.Common.Core.Master;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Nodes
{
    public partial class Node : ComponentBase
    {
        [Parameter]
        public string NodeId { get; set; }

        [Inject]
        public NodesService NodesService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        public NodeEditViewModel NodeModel { get; set; }
        public ManualFormValidator<NodeEditViewModel> NodeModelValidator { get; set; }

        public NodeDto NodeDto { get; set; }
        public ApiKeyDto ApiKeyDto { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await RefreshDataAsync();

                if (NodeDto == null)
                {
                    Navigation.NavigateTo("/settings/nodes");
                    return;
                }

                StateHasChanged();
            }
        }

        public async Task RefreshDataAsync()
        {
            NodeDto = await NodesService.GetNodeOrDefaultAsync(NodeId);
            ApiKeyDto = await NodesService.GetNodeApiKeyOrDefaultAsync(NodeId);

            if (NodeDto != null)
            {
                NodeModel = new NodeEditViewModel
                {
                    DisplayName = NodeDto.DisplayName,
                    BaseUri = NodeDto.BaseUri,
                    Name = NodeDto.Name
                };
            }
        }

        public async Task SaveAsync()
        {
            NodeDto.DisplayName = NodeModel.DisplayName;
            NodeDto.BaseUri = NodeModel.BaseUri;
            NodeDto.Name = NodeModel.Name;

            var updateResult = await NodesService.UpdateNodeAsync(NodeDto);

            if (!updateResult.Succeeded && updateResult.FailureDetails != null)
            {
                updateResult.FailureDetails.Errors.ForField(null).Do(x => NodeModelValidator.ModelState.Field(x => x.ErrorField).AddError(x.Message));
                updateResult.FailureDetails.Errors.ForField("BaseUri").Do(x => NodeModelValidator.ModelState.Field(x => x.BaseUri).AddError(x.Message));
            }
        }

        public class NodeEditViewModel
        {
            [Required, StringLength(40)]
            public string DisplayName { get; set; }

            [Required, StringLength(40)]
            public string Name { get; set; }

            [Required, DataType(DataType.Url)]
            public string BaseUri { get; set; }

            public object ErrorField { get; set; }
        }
    }
}
