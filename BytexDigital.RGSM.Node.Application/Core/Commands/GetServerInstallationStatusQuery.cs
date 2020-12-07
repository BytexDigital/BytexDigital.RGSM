using System.Threading;
using System.Threading.Tasks;

using BytexDigital.Common.Errors.Exceptions;
using BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Models.Status;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Commands
{

    public class GetServerInstallationStatusQuery : IRequest<GetServerInstallationStatusQuery.Response>
    {
        public string Id { get; set; }

        public class Handler : IRequestHandler<GetServerInstallationStatusQuery, Response>
        {
            private readonly ServerStateRegister _serverStateRegister;

            public Handler(ServerStateRegister serverStateRegister)
            {
                _serverStateRegister = serverStateRegister;
            }

            public async Task<Response> Handle(GetServerInstallationStatusQuery request, CancellationToken cancellationToken)
            {
                var state = _serverStateRegister.GetServerState(request.Id);

                if (state == null) throw new ServiceException().WithField(nameof(request.Id)).WithMessage("Server not found.");

                if (state is not IRunnable runnableState)
                    throw new ServerNotRunnableException();

                return new Response
                {
                    InstallationStatus = await runnableState.GetInstallationStatusAsync()
                };
            }
        }

        public class Response
        {
            public ServerInstallationStatus InstallationStatus { get; set; }
        }
    }
}
