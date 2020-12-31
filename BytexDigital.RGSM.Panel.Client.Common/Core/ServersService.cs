using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.TransferObjects.Entities;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Client.Common.Core
{
    public class ServersService
    {
        private readonly HttpClient _httpClient;

        public ServersService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ServerDto>> GetServersAsync(NodeDto nodeDto)
        {
            return await _httpClient.GetFromJsonAsync<List<ServerDto>>($"{nodeDto.BaseUri}/API/Servers/GetServers");
        }
    }
}
