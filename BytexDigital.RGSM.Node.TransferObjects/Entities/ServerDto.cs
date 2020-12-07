
using BytexDigital.RGSM.Node.TransferObjects.Enumerations;

namespace BytexDigital.RGSM.Node.TransferObjects.Entities
{
    public class ServerDto
    {
        public string DisplayName { get; set; }
        public string NodeId { get; set; }
        public ServerTypeDto Type { get; set; }
        public string Directory { get; set; }
    }
}
