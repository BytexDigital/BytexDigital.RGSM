namespace BytexDigital.RGSM.Panel.Server.TransferObjects.Entities
{
    public class SteamLoginSupportedAppDto : EntityDto
    {
        public string SteamCredentialId { get; set; }
        public long AppId { get; set; }
        public bool SupportsWorkshop { get; set; }
    }
}
