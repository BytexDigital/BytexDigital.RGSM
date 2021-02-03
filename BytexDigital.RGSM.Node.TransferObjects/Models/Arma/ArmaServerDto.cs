using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Models.Arma
{
    public class ArmaServerDto
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
    }
}
