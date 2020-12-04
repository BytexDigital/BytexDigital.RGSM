using BytexDigital.RGSM.Shared.TransferObjects.Enumerations;

namespace BytexDigital.RGSM.Shared.TransferObjects.Entities
{
    public class ServerDto
    {
        public string DisplayName { get; set; }
        public string NodeId { get; set; }
        public ServerTypeDto Type { get; set; }
        public ServerStatusDto Status { get; set; }
        public string Directory { get; set; }
    }
}
