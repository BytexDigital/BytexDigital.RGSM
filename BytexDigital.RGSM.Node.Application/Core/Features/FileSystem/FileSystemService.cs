using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.FileSystem;

namespace BytexDigital.RGSM.Node.Application.Core.Features.FileSystem
{
    public class FileSystemService
    {
        public Task<DirectoryContentDetails> GetDirectoryContentAsync(string path, CancellationToken cancellationToken = default)
        {
            var directoryReference = new DirectoryContentDetails
            {
                Path = path,
                Files = new System.Collections.Generic.List<FileDetails>()
            };

            directoryReference.Directories = Directory.GetDirectories(directoryReference.Path).ToList();

            foreach (var filePath in Directory.GetFiles(directoryReference.Path))
            {
                cancellationToken.ThrowIfCancellationRequested();

                directoryReference.Files.Add(new FileDetails
                {
                    Path = filePath,
                    SizeInBytes = new FileInfo(filePath).Length
                });
            }

            return Task.FromResult(directoryReference);
        }

        public async Task<byte[]> ReadFileAsStringAsync(string path, CancellationToken cancellationToken = default)
        {
            return await File.ReadAllBytesAsync(path, cancellationToken);
        }
    }
}
