using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.Domain.Models.FileSystem
{
    public class DirectoryContentDetails
    {
        public string Path { get; set; }

        public List<string> Directories { get; set; }
        public List<FileDetails> Files { get; set; }
    }
}
