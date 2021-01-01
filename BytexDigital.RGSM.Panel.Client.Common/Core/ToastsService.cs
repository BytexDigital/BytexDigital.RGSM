using System.Threading.Tasks;

using Microsoft.JSInterop;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class ToastsService
    {
        private readonly IJSRuntime _jsRuntime;
        private Task<IJSObjectReference> _module;
        private Task<IJSObjectReference> Module => _module ?? _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/blazor.toasts.js").AsTask();

        public ToastsService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task NotifyAsync(WebNotificationType type, string title, string body)
        {
            var module = await Module;
            await module.InvokeVoidAsync("showToast", (int)type, title, body);
        }

        public async Task NotifyAsync(WebNotificationType type, string title)
        {
            await NotifyAsync(type, title, "");
        }
    }

    public enum WebNotificationType
    {
        Success,
        Warning,
        Error,
        Info
    }
}
