using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task ThrowIfInvalidAsync(this HttpResponseMessage httpResponseMessage)
        {
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    throw await httpResponseMessage.Content.ReadFromJsonAsync<ApiProblemDetails>();
                }

                httpResponseMessage.EnsureSuccessStatusCode();
            }
        }
    }
}
