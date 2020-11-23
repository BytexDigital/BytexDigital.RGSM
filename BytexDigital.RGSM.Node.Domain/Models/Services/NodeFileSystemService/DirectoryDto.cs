using System.Collections.Generic;

namespace BytexDigital.RGSM.Node.Domain.Models.Services.NodeFileSystemService
{
    public class Directory
    {
        public string Path { get; set; }
        public List<File> Files { get; set; }
        public List<DirectoryReference> SubDirectories { get; set; }
    }
}
