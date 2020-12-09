using System.ComponentModel.DataAnnotations;

namespace BytexDigital.RGSM.Node.Domain.Entities
{
    public class TrackedWorkshopMod : Entity
    {
        [Required]
        public string ServerId { get; set; }

        [Required]
        public ulong PublishedFileId { get; set; }

        public uint? OfAppId { get; set; }

        [Required]
        public bool IsLoaded { get; set; }

        public string Directory { get; set; }

        public virtual Server Server { get; set; }
    }
}
