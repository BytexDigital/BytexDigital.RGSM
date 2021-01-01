
using Blazored.Modal;
using Blazored.Modal.Services;

using BytexDigital.RGSM.Panel.Client.Shared.Modals;

using Microsoft.AspNetCore.Components;

namespace BytexDigital.RGSM.Panel.Client.Extensions
{
    public static class IModalServiceExtensions
    {
        public static IModalReference ShowConfirmation(this IModalService modalService, string text)
        {
            var parameters = new ModalParameters();
            parameters.Add("Text", text);

            return modalService.ShowFrontModal<ConfirmationModal>(parameters);
        }

        public static IModalReference ShowFrontModal<TComponent>(this IModalService modalService, ModalParameters parameters = null, ModalOptions options = null) where TComponent : ComponentBase
        {
            if (options == null)
            {
                options = new ModalOptions();
            }

            if (parameters == null)
            {
                parameters = new ModalParameters();
            }

            parameters.Add("ModalParameters", parameters);

            options.UseCustomLayout = true;

            return modalService.Show<FrontModal<TComponent>>(null, parameters, options);
        }
    }
}
