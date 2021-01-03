using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3.Commands
{
    public class UpdateArmaServerSettingsCmd : IRequest
    {
        public int? AppId { get; set; }
        public string InstalledVersion { get; set; }
        public string Branch { get; set; }
        public string ExecutableFileName { get; set; }
        public string ProfilesPath { get; set; }
        public string BattlEyePath { get; set; }
        public string RconIp { get; set; }
        public int RconPort { get; set; }
        public string RconPassword { get; set; }
        public string AdditionalArguments { get; set; }
        public int Port { get; set; }

        public class Handler : IRequestHandler<UpdateArmaServerSettingsCmd>
        {
            public Handler()
            {

            }

            public async Task<Unit> Handle(UpdateArmaServerSettingsCmd request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
