using System;
using System.Threading.Tasks;

using Blazored.Modal;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BytexDigital.RGSM.Panel.Client.Shared.Modals
{
    public partial class FrontModal<T> where T : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance ModalInstance { get; set; }

        public RenderFragment Child { get; set; }

        public ElementReference ModalReference { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        protected override Task OnInitializedAsync()
        {
            Child = new RenderFragment(builder =>
            {
                builder.OpenComponent<T>(0);
                builder.CloseComponent();
            });

            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("BlazoredModal.activateFocusTrap", ModalReference, Id);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private void HandleBackgroundClick()
        {
            return;
        }
    }
}
