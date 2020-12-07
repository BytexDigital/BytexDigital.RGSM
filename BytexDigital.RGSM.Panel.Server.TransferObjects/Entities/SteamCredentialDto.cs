using System.Collections.Generic;

namespace BytexDigital.RGSM.Panel.Server.TransferObjects.Entities
{
    public class SteamCredentialDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginKey { get; set; }
        public string Sentry { get; set; }
        public List<SteamCredentialSupportedAppDto> SteamCredentialSupportedApps { get; set; }
    }
}
