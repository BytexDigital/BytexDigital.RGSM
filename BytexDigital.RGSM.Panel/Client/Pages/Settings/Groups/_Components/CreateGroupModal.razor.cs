using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Blazored.Modal;
using Blazored.Modal.Services;

using BytexDigital.Blazor.Components.FormValidators;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups._Components
{
    public partial class CreateGroupModal
    {
        [CascadingParameter]
        public BlazoredModalInstance ModelInstance { get; set; }

        [Inject]
        public GroupsService GroupsService { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public CreateGroupModel ViewModel { get; set; }
        public ManualFormValidator<CreateGroupModel> Validator { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ViewModel = new CreateGroupModel
            {
                DisplayName = "",
                Name = ""
            };
        }

        public async Task RegisterAsync()
        {
            var creationResult = await ServiceResult.FromAllAsync(async () => await GroupsService.CreateGroupAsync(new GroupDto
            {
                DisplayName = ViewModel.DisplayName,
                Name = ViewModel.Name
            }));

            if (!creationResult.Succeeded)
            {
                creationResult
                    .ForServiceErrors()
                    .ForField(nameof(GroupDto.Name), x => Validator.ModelState.Field(x => x.Name).AddError(x.Description))
                    .ForField(nameof(GroupDto.DisplayName), x => Validator.ModelState.Field(x => x.DisplayName).AddError(x.Description));

                return;
            }

            await ModelInstance.Close(ModalResult.Ok(creationResult.Result.Id));
        }

        public class CreateGroupModel
        {
            [Required]
            public string DisplayName { get; set; }

            [Required]
            public string Name { get; set; }
        }
    }
}
