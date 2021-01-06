using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Commands.Steam
{
    public class GetSteamCredentialsQuery : IRequest<GetSteamCredentialsQuery.Response>
    {
        public class Handler : IRequestHandler<GetSteamCredentialsQuery, Response>
        {
            private readonly SteamCredentialService _steamCredentialsService;

            public Handler(SteamCredentialService steamCredentialsService)
            {
                _steamCredentialsService = steamCredentialsService;
            }

            public async Task<Response> Handle(GetSteamCredentialsQuery request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    Credentials = await _steamCredentialsService.GetCredentials().ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<SteamCredential> Credentials { get; set; }
        }
    }
}
