using System.Threading.Tasks;

using BytexDigital.ErrorHandling.Shared;
using BytexDigital.RGSM.Node.Application.Options;

using Microsoft.Extensions.Options;

namespace BytexDigital.RGSM.Node.Application.Core
{
    public class ConnectivityService
    {
        private readonly IOptions<NodeOptions> _options;
        private readonly MasterApiService _masterApiService;

        public ConnectivityService(IOptions<NodeOptions> options, MasterApiService masterApiService)
        {
            _options = options;
            _masterApiService = masterApiService;
        }

        public async Task<bool> IsConnectedToMasterAsync()
        {
            var validityResult = await ServiceResult.FromAsync(async () => await _masterApiService.GetApiKeyValidityAsync(_options.Value.MasterOptions.ApiKey));

            return validityResult.Succeeded && validityResult.Result.IsValid;
        }
    }
}
