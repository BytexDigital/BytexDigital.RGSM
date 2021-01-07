using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class NodeService
    {
        private readonly HttpClient _httpClient;

        public NodeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool IsReachable, string FailureReason)> IsNodeReachableAsync(string baseUri)
        {
            try
            {
                var pingResponse = await _httpClient.GetAsync($"{baseUri}/API/Connectivity/Ping");

                if (!pingResponse.IsSuccessStatusCode) return (false, "The node is not reachable.");

                var authenticatedPingResponse = await _httpClient.GetAsync($"{baseUri}/API/Connectivity/AuthenticatedPing");

                if (!authenticatedPingResponse.IsSuccessStatusCode) return (false, "The node is reachable, but no authentication could be " +
                        "performed (the node is most likely incorrectly configured)");

                return (true, null);
            }
            catch
            {
                return (false, "Could not connect to node.");
            }
        }

        public async Task<List<NodeDto>> GetNodesAsync()
        {
            var response = await _httpClient.GetAsync("/API/Nodes/GetNodes");

            await response.ThrowIfInvalidAsync();

            return await response.Content.ReadFromJsonAsync<List<NodeDto>>();
        }
    }
}
