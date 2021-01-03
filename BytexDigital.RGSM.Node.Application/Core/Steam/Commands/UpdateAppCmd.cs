using System;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Steam.ContentDelivery.Models;
using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Steam.Commands
{

    public class UpdateAppCmd : IRequest<UpdateAppCmd.Response>
    {
        public Func<Depot, bool> DepotCondition { get; set; }

        public AppId AppId { get; set; }
        public string Id { get; set; }
        public string Directory { get; set; }
        public UpdateState UpdateState { get; set; }
        public string Branch { get; set; }
        public string BranchPassword { get; set; }
        public bool UseAnonymousUser { get; set; }

        public class Handler : IRequestHandler<UpdateAppCmd, Response>
        {
            private readonly SteamDownloadService _steamDownloadService;

            public Handler(SteamDownloadService steamDownloadService)
            {
                _steamDownloadService = steamDownloadService;
            }

            public async Task<Response> Handle(UpdateAppCmd request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    UpdateState = await _steamDownloadService.DownloadAppIdAsync(request.AppId, request.DepotCondition, request.Directory, request.Branch, request.BranchPassword, request.UseAnonymousUser)
                };
            }
        }

        public class Response
        {
            public UpdateState UpdateState { get; set; }
        }
    }
}
