using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Games.Shared;

namespace BytexDigital.RGSM.Node.Application.Games.Arma3
{
    public class LocalArma3Server : ServerBase
    {
        public override string GlobalId { get; set; }
        public override string Directory { get; set; }

        public override Task OnStartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task OnStopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
