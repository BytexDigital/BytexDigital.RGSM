using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Panel.Server.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Panel.Server.Application.Core.Steam.Commands
{
    public class GetSteamLoginsQuery : IRequest<GetSteamLoginsQuery.Response>
    {
        public class Handler : IRequestHandler<GetSteamLoginsQuery, Response>
        {
            private readonly SteamLoginService _steamCredentialsService;

            public Handler(SteamLoginService steamCredentialsService)
            {
                _steamCredentialsService = steamCredentialsService;
            }

            public async Task<Response> Handle(GetSteamLoginsQuery request, CancellationToken cancellationToken)
            {
                var credentials = await _steamCredentialsService.GetLogins().ToListAsync();

                return new Response
                {
                    Credentials = credentials
                };
            }
        }

        public class Response
        {
            public List<SteamLogin> Credentials { get; set; }
        }
    }
}
