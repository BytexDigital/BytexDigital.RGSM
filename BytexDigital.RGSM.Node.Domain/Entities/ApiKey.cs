using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class ApiKey : Entity
    {
        [Required]
        public string Key { get; set; }

        public string Remarks { get; set; }
    }
}
