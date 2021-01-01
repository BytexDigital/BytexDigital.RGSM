using System;
using System.Collections.Generic;
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

        [Parameter]
        public ModalParameters ModalParameters { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> AdditionalAttributes { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        protected override Task OnInitializedAsync()
        {
            Child = new RenderFragment(builder =>
            {
                int i = 0;

                builder.OpenComponent<T>(i++);

                if (AdditionalAttributes != null)
                {
                    foreach (var attribute in AdditionalAttributes)
                    {
                        builder.AddAttribute(i++, attribute.Key, attribute.Value);
                    }
                }

                if (ModalParameters != null)
                {
                    var type = ModalParameters.GetType();
                    var field = type.GetField("_parameters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    var parameters = field.GetValue(ModalParameters) as Dictionary<string, object>;

                    foreach (var parameter in parameters)
                    {
                        if (parameter.Key == nameof(ModalParameters)) continue;

                        builder.AddAttribute(i++, parameter.Key, parameter.Value);
                    }
                }

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
