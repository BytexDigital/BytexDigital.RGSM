using System.Collections.Generic;

namespace BytexDigital.RGSM.Shared.TransferObjects.Models.Services.NodeFileSystemService
{
    public class DirectoryDto
    {
        public string Path { get; set; }
        public List<FileDto> Files { get; set; }
        public List<DirectoryReferenceDto> SubDirectories { get; set; }
    }
}
