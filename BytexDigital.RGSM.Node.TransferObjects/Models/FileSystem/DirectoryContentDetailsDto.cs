using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.TransferObjects.Models.FileSystem
{
    public class DirectoryContentDetailsDto
    {
        public string Path { get; set; }

        public List<string> Directories { get; set; }
        public List<FileDetailsDto> Files { get; set; }
    }
}
