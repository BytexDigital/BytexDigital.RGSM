using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.Domain.Models.Workshop
{
    public class WorkshopMod
    {
        public ulong Id { get; set; }
        public uint? OfAppId { get; set; }
        public bool Enabled { get; set; }

        public string Directory { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
