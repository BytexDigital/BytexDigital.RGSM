using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using BytexDigital.Blazor.Components.FormValidators;
using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Client.Common.Core.Commands;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups
{
    public partial class Group : Settings._Components.SettingsComponentBase
    {
        [Parameter]
        public string GroupId { get; set; }

        [Inject]
        public GroupService GroupsService { get; set; }

        [Inject]
        public ToastService ToastsService { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        public ManualFormValidator<GroupModel> Validator { get; set; }

        public GroupModel ViewModel { get; set; }

        public GroupDto GroupDto { get; set; }
        public Dictionary<NodeDto, List<ServerDto>> ServerDtos { get; set; }

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
            var serversResponse = await ServiceResult.FromAllAsync(async () => (await Mediator.Send(new GetAllServersQuery())).Servers);

            if (!serversResponse.Succeeded) return; // TODO: Show an error
            if (!response.Succeeded) return;

            GroupDto = response.Result;
            ServerDtos = serversResponse.Result;
        }

        public async Task SaveGroupAsync()
        {
            GroupDto.DisplayName = ViewModel.DisplayName;
            GroupDto.Name = ViewModel.Name;

            var updateResult = await ServiceResult.FromAllAsync(async () => await GroupsService.UpdateGroupAsync(GroupDto));

            if (!updateResult.Succeeded)
            {
                updateResult
                    .ForServiceErrors(async x => await ToastsService.NotifyAsync(WebNotificationType.Error, "Application Error", x.Description))
                    .ForField("Name", x => Validator.ModelState.Field(x => x.Name).AddError(x.Description))
                    .ForField("DisplayName", x => Validator.ModelState.Field(x => x.DisplayName).AddError(x.Description));

                updateResult
                    .ForApplicationOrExceptionErrors(async x => await ToastsService.NotifyAsync(WebNotificationType.Error, "Application Error", x.Description));

                return;
            }

            if (updateResult.Succeeded)
            {
                await ToastsService.NotifyAsync(WebNotificationType.Success, "Saved changes.");
            }
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
