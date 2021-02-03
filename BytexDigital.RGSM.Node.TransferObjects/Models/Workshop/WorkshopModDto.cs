using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Models.Workshop
{
    public class WorkshopModDto
    {
        public ulong Id { get; set; }
        public uint? OfAppId { get; set; }
        public bool Enabled { get; set; }

        public string Directory { get; set; }

        public Dictionary<string, string> Metadata { get; set; }
    }
}
