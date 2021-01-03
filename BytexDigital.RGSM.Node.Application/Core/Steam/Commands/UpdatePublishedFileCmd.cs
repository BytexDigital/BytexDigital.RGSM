using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Steam.Core.Structs;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Steam.Commands
{
    public class UpdatePublishedFileCmd : IRequest<UpdatePublishedFileCmd.Response>
    {
        public AppId AppId { get; set; }
        public PublishedFileId PublishedFileId { get; set; }
        public string Directory { get; set; }
        public bool UseAnonymousUser { get; set; }

        public class Handler : IRequestHandler<UpdatePublishedFileCmd, Response>
        {
            private readonly SteamDownloadService _steamDownloadService;

            public Handler(SteamDownloadService steamDownloadService)
            {
                _steamDownloadService = steamDownloadService;
            }

            public async Task<Response> Handle(UpdatePublishedFileCmd request, CancellationToken cancellationToken)
            {
                return new Response
                {
                    UpdateState = await _steamDownloadService.DownloadPublishedFileAsync(request.AppId, request.PublishedFileId, request.Directory, request.UseAnonymousUser)
                };
            }
        }

        public class Response
        {
            public UpdateState UpdateState { get; set; }
        }
    }
}
