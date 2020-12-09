using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.Domain.Models.Console
{
    public class ConsoleOutputContent
    {
        public string Type { get; set; }
        public string FromFile { get; set; }
        public List<string> Lines { get; set; }
    }
}
