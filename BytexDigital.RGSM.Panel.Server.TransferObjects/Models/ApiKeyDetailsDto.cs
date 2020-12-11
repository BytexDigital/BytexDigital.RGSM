using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;

namespace BytexDigital.RGSM.Panel.Server.TransferObjects.Models
{
    public class ApiKeyDetailsDto
    {
        public bool IsValid { get; set; }
        public NodeDto IssuedToNode { get; set; }
    }
}
