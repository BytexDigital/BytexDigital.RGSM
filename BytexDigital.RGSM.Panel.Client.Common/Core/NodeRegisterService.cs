using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.AspNetCore;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class NodeRegisterService
    {
        private readonly HttpClient _httpClient;

        public NodeRegisterService(HttpClient httpClient)
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

        public async Task UpdateNodeAsync(NodeDto nodeDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/API/Nodes/Update/{nodeDto.Id}", nodeDto);

            if (response.IsSuccessStatusCode)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            }

            response.EnsureSuccessStatusCode();
            throw new System.Exception();
        }

        public async Task<NodeDto> RegisterNodeAsync(NodeDto nodeDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/API/Nodes/RegisterNode", nodeDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<NodeDto>();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            }

            response.EnsureSuccessStatusCode();
            throw new System.Exception();
        }

        public async Task DeleteNodeAsync(NodeDto nodeDto)
        {
            var response = await _httpClient.DeleteAsync($"/API/Nodes/Delete/{nodeDto.Id}");

            if (response.IsSuccessStatusCode)
            {
                return;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                throw await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
            }

            response.EnsureSuccessStatusCode();
            throw new System.Exception();
        }
    }
}
