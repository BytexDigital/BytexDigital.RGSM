﻿namespace BytexDigital.RGSM.Node.Domain.Models.Status
{
    public class ServerInstallationStatus
    {
        public bool IsInstalled { get; set; }

        public bool RequiresUpdate { get; set; }
        public string InstalledVersion { get; set; }
        public string AvailableVersion { get; set; }

        public bool IsUpdating { get; set; }
        public double UpdateProgress { get; set; }
    }
}
