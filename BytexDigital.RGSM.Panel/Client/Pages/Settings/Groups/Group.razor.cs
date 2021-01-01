using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using BytexDigital.Blazor.Components.FormValidators;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups
{
    public partial class Group : Settings._Components.SettingsComponentBase
    {
        [Parameter]
        public string GroupId { get; set; }

        [Inject]
        public GroupsService GroupsService { get; set; }

        public ManualFormValidator<GroupModel> Validator { get; set; }

        public GroupModel ViewModel { get; set; }

        public GroupDto GroupDto { get; set; }

        public Group()
        {
            ViewModel = new GroupModel();
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var oldGroupId = GroupId;
            var newGroupId = parameters.GetValueOrDefault<string>(nameof(GroupId));

            await base.SetParametersAsync(parameters);

            if (newGroupId != oldGroupId)
            {
                await RefreshAsync();

                if (GroupDto == null)
                {
                    Navigation.NavigateTo("/settings/groups");
                    return;
                }

                ViewModel = new GroupModel
                {
                    DisplayName = GroupDto.DisplayName,
                    Name = GroupDto.Name
                };

                StateHasChanged();
            }
        }

        public async Task RefreshAsync()
        {
            var response = await ServiceResult.FromAllAsync(async () => await GroupsService.GetGroupAsync(GroupId));

            if (!response.Succeeded) return;

            GroupDto = response.Result;
        }

        public async Task SaveGroupAsync()
        {

        }

        public async Task DeleteGroupAsync()
        {

        }

        public class GroupModel
        {
            [Required]
            public string DisplayName { get; set; }

            [Required]
            public string Name { get; set; }
        }
    }
}
