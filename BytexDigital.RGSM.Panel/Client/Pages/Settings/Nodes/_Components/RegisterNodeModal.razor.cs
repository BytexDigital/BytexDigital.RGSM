
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Blazored.Modal;
using Blazored.Modal.Services;

using BytexDigital.Blazor.Components.FormValidators;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Nodes._Components
{
    public partial class RegisterNodeModal : ComponentBase
    {
        [CascadingParameter]
        public BlazoredModalInstance ModelInstance { get; set; }

        [Inject]
        public NodeRegisterService NodeRegisterService { get; set; }

        [Inject]
        public NodesService NodeService { get; set; }

        public RegisterNodeModel ViewModel { get; set; }
        public ManualFormValidator<RegisterNodeModel> Validator { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ViewModel = new RegisterNodeModel
            {
                DisplayName = "",
                BaseUri = "",
                Name = ""
            };
        }

        public async Task RegisterAsync()
        {
            NodeDto nodeDto = default;

            try
            {
                nodeDto = await NodeRegisterService.RegisterNodeAsync(new NodeDto
                {
                    DisplayName = ViewModel.DisplayName,
                    Name = ViewModel.Name,
                    BaseUri = ViewModel.BaseUri
                });
            }
            catch (ServiceException ex)
            {
                ex
                    .ForServiceErrors()
                    .ForField("DisplayName", x => Validator.ModelState.Field(x => x.DisplayName).AddError(x.Description))
                    .ForField("Name", x => Validator.ModelState.Field(x => x.Name).AddError(x.Description))
                    .ForField("BaseUri", x => Validator.ModelState.Field(x => x.BaseUri).AddError(x.Description));

                return;
            }

            await ModelInstance.Close(ModalResult.Ok(nodeDto.Id));
        }

        public class RegisterNodeModel
        {
            [Required]
            public string DisplayName { get; set; }

            [Required]
            public string Name { get; set; }

            [Required]
            public string BaseUri { get; set; }
        }
    }
}
