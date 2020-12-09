
using BytexDigital.RGSM.Node.TransferObjects.Enumerations;

namespace BytexDigital.RGSM.Node.TransferObjects.Entities
{
    public class ServerDto : EntityDto
    {
        public string DisplayName { get; set; }
        public ServerTypeDto Type { get; set; }
        public string Directory { get; set; }
    }
}
