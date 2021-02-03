using System.Collections.Generic;

using BytexDigital.RGSM.Node.Domain.Models.Workshop;

namespace BytexDigital.RGSM.Node.Domain.Models.Arma
{
    public class ArmaServer
    {
        public bool IsInstalled { get; set; }

        public int? AppId { get; set; }

        public string InstalledVersion { get; set; }

        public string Branch { get; set; }

        public string ExecutableFileName { get; set; }

        public string ProfilesPath { get; set; }

        public string BattlEyePath { get; set; }

        public string RconIp { get; set; }

        public int RconPort { get; set; }

        public string RconPassword { get; set; }

        public string AdditionalArguments { get; set; }

        public int Port { get; set; }

        public List<uint> Depots { get; set; }

        public List<WorkshopMod> WorkshopMods { get; set; }
    }
}
