
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Domain.Entities;

namespace BytexDigital.RGSM.Domain.Games.Arma3.Entities
{
    public class Arma3Server : Entity
    {
        [Required]
        public string ServerId { get; set; }

        public string BetaBranch { get; set; }

        [Required]
        public string Hostname { get; set; }

        public string? Password { get; set; }

        public string? PasswordAdmin { get; set; }

        public string? ServerCommandPassword { get; set; }

        public string MessageOfTheDay { get; set; }

        public int? MessageOfTheDayInterval { get; set; }

        [Required]
        public int MaxPlayers { get; set; }

        [Required]
        public int VerifySignatures { get; set; }

        public int? AllowFilePatching { get; set; }
    }
}
