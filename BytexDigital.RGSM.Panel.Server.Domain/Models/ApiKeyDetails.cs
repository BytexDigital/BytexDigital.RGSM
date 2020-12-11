using BytexDigital.RGSM.Panel.Server.Domain.Entities;

namespace BytexDigital.RGSM.Panel.Server.Domain.Models
{
    public class ApiKeyDetails
    {
        public bool IsValid { get; set; }
        public Node IssuedToNode { get; set; }
    }
}
