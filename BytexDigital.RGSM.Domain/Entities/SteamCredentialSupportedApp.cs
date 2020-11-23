﻿using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class SteamCredentialSupportedApp : Entity
    {
        [Required]
        public string SteamCredentialId { get; set; }

        [Required]
        public long AppId { get; set; }

        [Required]
        public bool SupportsWorkshop { get; set; }
    }
}