using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Models.Console
{
    public class ConsoleOutputContentDto
    {
        public string Type { get; set; }
        public string FromFile { get; set; }
        public List<string> Lines { get; set; }
    }
}
