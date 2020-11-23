using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Games.Shared;

namespace BytexDigital.RGSM.Node.Application.Games.Arma3
{
    public class LocalArma3Server : ServerBase
    {
        public override string GlobalId { get; set; }

        public override Task<bool> CanStartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> CanStopAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task StopAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
