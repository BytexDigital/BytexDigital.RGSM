using System.Collections.Generic;
using System.Threading.Tasks;

using Blazored.Modal.Services;

using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Client.Common.Core;
using BytexDigital.RGSM.Panel.Client.Common.Core.Commands;
using BytexDigital.RGSM.Panel.Client.Extensions;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

using MediatR;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Pages.Settings.Groups
{
    public partial class Index : Settings._Components.SettingsComponentBase
    {
        [Inject]
        public GroupsService GroupsService { get; set; }

        [Inject]
        public IModalService ModalService { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        public List<GroupDto> Groups { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshGroupsAsync();
            StateHasChanged();
        }

        public async Task RefreshGroupsAsync()
        {
            Groups = await GroupsService.GetGroupsAsync();
        }

        public async Task CreateGroupAsync()
        {
            var modalRef = ModalService.ShowFrontModal<_Components.CreateGroupModal>(null, new Blazored.Modal.ModalOptions { UseCustomLayout = true });
            var modalResult = await modalRef.Result;

            if (modalResult.Cancelled) return;

            await RefreshGroupsAsync();
            StateHasChanged();
        }
    }
}
