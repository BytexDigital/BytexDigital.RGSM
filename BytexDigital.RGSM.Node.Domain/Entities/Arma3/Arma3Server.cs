using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities.Arma3
{
    public class Arma3Server : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public bool IsInstalled { get; set; }

        public int? AppId { get; set; }

        public string InstalledVersion { get; set; }

        public string Branch { get; set; }

        public string ExecutableFileName { get; set; }

        public string ProfilesPath { get; set; }

        public string BattlEyePath { get; set; }

        [Required]
        public string RconIp { get; set; }

        [Required]
        public int RconPort { get; set; }

        [Required]
        public string RconPassword { get; set; }

        public string AdditionalArguments { get; set; }

        [Required]
        public int Port { get; set; }


        public virtual Server Server { get; set; }
    }
}
