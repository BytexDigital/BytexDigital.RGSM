using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Panel.Server.Domain.Entities
{
    public class SteamLogin : Entity
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string LoginKey { get; set; }

        public string Sentry { get; set; }

        public string WebApiKey { get; set; }

        public virtual ICollection<SteamLoginSupportedApp> SteamLoginSupportedApps { get; set; }
    }
}
