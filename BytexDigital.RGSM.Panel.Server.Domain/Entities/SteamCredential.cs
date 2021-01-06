using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class SteamCredential : Entity
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string LoginKey { get; set; }

        public string Sentry { get; set; }

        public virtual ICollection<SteamCredentialSupportedApp> SteamCredentialSupportedApps { get; set; }
    }
}
