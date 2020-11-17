
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using BytexDigital.RGSM.Domain.Interfaces;

using Microsoft.AspNetCore.Identity;

namespace BytexDigital.RGSM.Domain.Entities
{
    public class ApplicationUser : IdentityUser, IHasCreationTimestamp
    {
        [Required]
        public DateTimeOffset TimeCreated { get; set; }

        public virtual ICollection<ApplicationUserGroup> Groups { get; set; }
    }
}
