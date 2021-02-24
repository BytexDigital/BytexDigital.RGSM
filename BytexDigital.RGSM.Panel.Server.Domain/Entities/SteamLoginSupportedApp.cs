using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class SteamLoginSupportedApp : Entity
    {
        [Required]
        public long AppId { get; set; }

        [Required]
        public bool SupportsWorkshop { get; set; }
    }
}
