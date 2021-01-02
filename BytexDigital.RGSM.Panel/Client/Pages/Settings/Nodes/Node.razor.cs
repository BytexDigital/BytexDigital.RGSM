using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using BytexDigital.Blazor.Components.FormValidators;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Client.Extensions;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Nodes
{
    public partial class Node : ComponentBase
    {
        [Parameter]
        public string NodeId { get; set; }

        [Inject]
        public NodeRegisterService NodeService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }

        [Inject]
        public ToastsService ToastsService { get; set; }

        public NodeEditViewModel NodeModel { get; set; }
        public ManualFormValidator<NodeEditViewModel> NodeModelValidator { get; set; }

        public NodeDto NodeDto { get; set; }
        public ApiKeyDto ApiKeyDto { get; set; }

        public Node()
        {
            NodeModel = new NodeEditViewModel
            {
                DisplayName = "",
                BaseUri = "",
                Name = ""
            };
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var nodeId = NodeId;
            var newNodeId = parameters.GetValueOrDefault<string>(nameof(NodeId));

            await base.SetParametersAsync(parameters);

            if (nodeId != newNodeId)
            {
                await RefreshDataAsync();

                if (NodeDto == null)
                {
                    Navigation.NavigateTo("/settings/nodes");
                    return;
                }

                NodeModel = new NodeEditViewModel
                {
                    DisplayName = NodeDto.DisplayName,
                    BaseUri = NodeDto.BaseUri,
                    Name = NodeDto.Name
                };

                StateHasChanged();
            }
        }

        public async Task RefreshDataAsync()
        {
            NodeDto = await NodeService.GetNodeOrDefaultAsync(NodeId);
            ApiKeyDto = await NodeService.GetNodeApiKeyOrDefaultAsync(NodeId);
        }

        public async Task SaveAsync()
        {
            NodeDto.DisplayName = NodeModel.DisplayName;
            NodeDto.BaseUri = NodeModel.BaseUri;
            NodeDto.Name = NodeModel.Name;

            var updateResult = await ServiceResult.FromAsync(async () => await NodeService.UpdateNodeAsync(NodeDto));

            if (!updateResult.Succeeded && updateResult.FailureDetails != null)
            {
                updateResult
                    .ForServiceErrors()
                    .ForNoField(x => NodeModelValidator.ModelState.Field(x => x.ErrorField).AddError(x.Description))
                    .ForField("BaseUri", x => NodeModelValidator.ModelState.Field(x => x.BaseUri).AddError(x.Description))
                    .ForField("Name", x => NodeModelValidator.ModelState.Field(x => x.Name).AddError(x.Description))
                    .ForField("DisplayName", x => NodeModelValidator.ModelState.Field(x => x.DisplayName).AddError(x.Description));

                await updateResult
                    .AsAsync()
                    .ForServiceErrors(async x => await ToastsService.NotifyAsync(WebNotificationType.Error, "Application Error", x.Description))
                    .WaitAsync();
            }

            if (updateResult.Succeeded)
            {
                await ToastsService.NotifyAsync(WebNotificationType.Success, "Saved changes.");
            }
        }

        public async Task DeleteAsync()
        {
            var modalRef = ModalService.ShowConfirmation("Are you sure that you want to delete this node?");
            var result = await modalRef.Result;

            if (result.Cancelled) return;

            var deleteResult = await ServiceResult.FromAsync(async () => await NodeService.DeleteNodeAsync(NodeDto));

            if (deleteResult.Succeeded)
            {
                Navigation.NavigateTo("/settings/nodes");
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
