using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.RGSM.Shared;

namespace BytexDigital.RGSM.Panel.Client.Common.Core.Master
{
    public class NodesService
    {
        private readonly HttpClient _httpClient;

        public NodesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NodeDto>> GetNodesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<NodeDto>>("/API/Nodes/GetNodes");
        }

        public async Task<NodeDto> GetNodeOrDefaultAsync(string nodeId)
        {
            return (await GetNodesAsync()).FirstOrDefault(x => x.Id == nodeId);
        }

        public async Task<ApiKeyDto> GetNodeApiKeyOrDefaultAsync(string nodeId)
        {
            var response = await _httpClient.GetAsync($"/API/Nodes/GetNodeKey/{nodeId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiKeyDto>();
            }

            return null;
        }

        public async Task<ServiceResult> UpdateNodeAsync(NodeDto nodeDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/API/Nodes/Update/{nodeDto.Id}", nodeDto);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return await response.Content.ReadFromJsonAsync<ApiFailureDetails>();
            }

            return false;
        }
    }
}
