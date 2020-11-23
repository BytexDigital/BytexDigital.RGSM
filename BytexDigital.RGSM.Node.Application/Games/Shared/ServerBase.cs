using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Games.Shared
{
    public abstract class ServerBase
    {
        public abstract string GlobalId { get; set; }
        public abstract string Directory { get; set; }
        public abstract Task StartAsync();
        public abstract Task StopAsync();
        public abstract Task<bool> CanStartAsync();
        public abstract Task<bool> CanStopAsync();
    }
}
