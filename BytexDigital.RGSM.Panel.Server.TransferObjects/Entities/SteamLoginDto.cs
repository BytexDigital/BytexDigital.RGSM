using System.Collections.Generic;

namespace BytexDigital.RGSM.Panel.Server.TransferObjects.Entities
{
    public class SteamLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginKey { get; set; }
        public string Sentry { get; set; }
        public string WebApiKey { get; set; }
        public List<SteamLoginSupportedAppDto> SteamLoginSupportedApps { get; set; }
    }
}
