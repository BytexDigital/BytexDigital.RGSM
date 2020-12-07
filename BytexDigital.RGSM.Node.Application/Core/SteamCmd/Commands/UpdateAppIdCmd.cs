using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.SteamCmd.Commands
{

    public class UpdateAppIdCmd : IRequest<UpdateAppIdCmd.Response>
    {
        public string SteamUsernameUsedOverride { get; set; }

        public AppId AppId { get; set; }
        public string Id { get; set; }
        public string Directory { get; set; }
        public UpdateState UpdateState { get; set; }
        public string Branch { get; set; }
        public string BranchPassword { get; set; }

        public class Handler : IRequestHandler<UpdateAppIdCmd, Response>
        {
            private readonly SteamDownloadService _steamDownloadService;

            public Handler(SteamDownloadService steamDownloadService)
            {
                _steamDownloadService = steamDownloadService;
            }

            public async Task<Response> Handle(UpdateAppIdCmd request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    UpdateState = await _steamDownloadService.DownloadAppIdAsync(request.AppId, request.Directory, request.Branch, request.BranchPassword)
                };
            }
        }

        public class Response
        {
            public UpdateState UpdateState { get; set; }
        }
    }
}
