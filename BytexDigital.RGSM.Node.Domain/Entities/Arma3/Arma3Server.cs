using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities.Arma3
{
    public class Arma3Server : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public bool IsInstalled { get; set; }

        public string InstalledVersion { get; set; }

        public string BetaBranch { get; set; }

        public string ExecutableFileName { get; set; }

        [Required]
        public string Hostname { get; set; }

        [Required]
        public int Port { get; set; }

        public string Password { get; set; }

        public string PasswordAdmin { get; set; }

        public string ServerCommandPassword { get; set; }

        public string MessageOfTheDay { get; set; }

        public int? MessageOfTheDayInterval { get; set; }

        [Required]
        public int MaxPlayers { get; set; }

        [Required]
        public int VerifySignatures { get; set; }

        public int? AllowFilePatching { get; set; }


        public virtual Server Server { get; set; }
    }
}
