namespace BytexDigital.RGSM.Node.TransferObjects.Models.Status
{
    public class ServerInstallationStatusDto
    {
        public bool IsInstalled { get; set; }

        public bool RequiresUpdate { get; set; }

        public bool IsUpdating { get; set; }
        public double UpdateProgress { get; set; }
    }
}
