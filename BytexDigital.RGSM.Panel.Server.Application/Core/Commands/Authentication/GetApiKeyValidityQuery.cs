using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Application.Core.Authentication.ApiKeys;
using BytexDigital.RGSM.Panel.Server.Domain.Models;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Authentication
{
    public class GetApiKeyValidityQuery : IRequest<GetApiKeyValidityQuery.Response>
    {
        public string KeyValue { get; set; }

        public class Handler : IRequestHandler<GetApiKeyValidityQuery, Response>
        {
            private readonly ApiKeyService _authenticationService;

            public Handler(ApiKeyService authenticationService)
            {
                _authenticationService = authenticationService;
            }

            public async Task<Response> Handle(GetApiKeyValidityQuery request, CancellationToken cancellationToken)
            {
                var key = await _authenticationService.GetApiKey(request.KeyValue).FirstOrDefaultAsync();

                return new Response
                {
                    ApiKeyDetails = new ApiKeyDetails
                    {
                        IsValid = key != null,
                        IssuedToNode = key?.Node
                    }
                };
            }
        }

        public class Response
        {
            public ApiKeyDetails ApiKeyDetails { get; set; }
        }
    }
}
