using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace BytexDigital.RGSM.Node.Application.Shared.Services
{
    public class NodeFileSystemService
    {
        public Domain.Models.Services.NodeFileSystemService.Directory GetDirectory(string path)
        {
            // If we are on Windows, show us the documents directory
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && path == "~")
            {
                path = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }

            // If we are on Windows, interpret "/" as a drive overview
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && path == "/")
            {
                return new Domain.Models.Services.NodeFileSystemService.Directory
                {
                    SubDirectories = DriveInfo.GetDrives().Select(x => new Domain.Models.Services.NodeFileSystemService.DirectoryReference
                    {
                        Path = x.RootDirectory.FullName
                    }).ToList()
                };
            }

            var filePaths = Directory.GetFiles(path);
            var directoryPaths = Directory.GetDirectories(path);

            var files = new List<Domain.Models.Services.NodeFileSystemService.File>();

            foreach (var filePath in filePaths)
            {
                try
                {
                    files.Add(new Domain.Models.Services.NodeFileSystemService.File
                    {
                        Path = filePath,
                        Size = new FileInfo(filePath).Length
                    });
                }
                catch
                {
                }
            }

            return new Domain.Models.Services.NodeFileSystemService.Directory
            {
                Files = files,
                SubDirectories = directoryPaths.Select(x => new Domain.Models.Services.NodeFileSystemService.DirectoryReference { Path = x }).ToList()
            };
        }
    }
}
