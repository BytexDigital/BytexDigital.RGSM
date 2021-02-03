using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.ServerLogs;
using BytexDigital.RGSM.Node.Domain.Models.Logs;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public partial class ArmaServerState : IServerLogs
    {
        public async Task<List<LogSource>> GetLogSourcesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var profilesFolder = await GetProfilesPathAsync(cancellationToken);
                if (!Path.IsPathRooted(profilesFolder)) profilesFolder = System.IO.Path.Combine(BaseDirectory, profilesFolder);
                if (!Directory.Exists(profilesFolder)) return new List<LogSource>();

                List<LogSource> sources = new List<LogSource>();

                // RGSM logs
                var rgsmLogsDirectory = Path.Combine(BaseDirectory, ".rgsm", "logs");

                if (Directory.Exists(rgsmLogsDirectory))
                {
                    var rgsmLogs = Directory.GetFiles(rgsmLogsDirectory, "*.log");

                    foreach (var rgsmLogFile in rgsmLogs)
                    {
                        sources.Add(new LogSource
                        {
                            Type = "rgsm",
                            Name = $"rgsm:{Path.GetFileNameWithoutExtension(rgsmLogFile)}",
                            SizeInBytes = new FileInfo(rgsmLogFile).Length,
                            TimeLastUpdated = new DateTimeOffset(File.GetLastWriteTimeUtc(rgsmLogFile), TimeSpan.Zero),
                            MetaValues = new Dictionary<string, string>
                            {
                                { "path", rgsmLogFile }
                            }
                        });
                    }
                }

                // console_xxxxx.log files
                var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.log");

                foreach (var consoleFilePath in consoleFiles)
                {
                    sources.Add(new LogSource
                    {
                        Type = "console_file",
                        Name = Path.GetFileNameWithoutExtension(consoleFilePath),
                        SizeInBytes = new FileInfo(consoleFilePath).Length,
                        TimeLastUpdated = new DateTimeOffset(File.GetLastWriteTimeUtc(consoleFilePath), TimeSpan.Zero),
                        MetaValues = new Dictionary<string, string>
                        {
                            { "path", consoleFilePath }
                        }
                    });
                }

                // RPT files
                var rptFiles = System.IO.Directory.GetFiles(profilesFolder, "*.rpt");

                foreach (var rptFilePath in rptFiles)
                {
                    sources.Add(new LogSource
                    {
                        Type = "rpt_file",
                        Name = Path.GetFileNameWithoutExtension(rptFilePath),
                        SizeInBytes = new FileInfo(rptFilePath).Length,
                        TimeLastUpdated = new DateTimeOffset(File.GetLastWriteTimeUtc(rptFilePath), TimeSpan.Zero),
                        MetaValues = new Dictionary<string, string>
                        {
                            { "path", rptFilePath }
                        }
                    });
                }

                return sources;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fetching log sources failed.");
                throw;
            }
        }

        public async Task<LogSource> GetPrimaryLogSourceOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var primarySource = (await GetLogSourcesAsync(cancellationToken))
                    .Where(x => x.Type == "rpt_file")
                    .OrderByDescending(x => x.TimeLastUpdated)
                    .FirstOrDefault();

                return primarySource;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fetching primary log source failed.");
                throw;
            }
        }

        public async Task<LogContent> GetLogContentOrDefaultAsync(string logSourceName, int limitLines = 0, CancellationToken cancellationToken = default)
        {
            try
            {
                var profilesFolder = await GetProfilesPathAsync(cancellationToken);
                if (!System.IO.Path.IsPathRooted(profilesFolder)) profilesFolder = System.IO.Path.Combine(BaseDirectory, profilesFolder);

                var sources = await GetLogSourcesAsync(cancellationToken);
                var requestSource = sources.FirstOrDefault(x => x.Name == logSourceName);

                if (requestSource == default) return default;

                string fileToReadPath = default;

                if (requestSource.Type == "rgsm")
                {
                    var rgsmFiles = System.IO.Directory.GetFiles(Path.Combine(BaseDirectory, ".rgsm", "logs"), "*.log");
                    var requestedRgsmFile = rgsmFiles.FirstOrDefault(x =>
                        Path.GetFileNameWithoutExtension(x) == Path.GetFileNameWithoutExtension(requestSource.MetaValues["path"]));

                    fileToReadPath = requestedRgsmFile;
                }
                else if (requestSource.Type == "console_file")
                {
                    var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.log");
                    var requestedConsoleFile = consoleFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == requestSource.Name);

                    fileToReadPath = requestedConsoleFile;
                }
                else if (requestSource.Type == "rpt_file")
                {
                    var consoleFiles = System.IO.Directory.GetFiles(profilesFolder, "*.rpt");
                    var requestedConsoleFile = consoleFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == requestSource.Name);

                    fileToReadPath = requestedConsoleFile;
                }

                if (!File.Exists(fileToReadPath)) return null;

                using var fileStream = new FileStream(fileToReadPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(fileStream);

                var content = await reader.ReadToEndAsync();
                var lines = content.Split(Environment.NewLine).ToList();

                if (limitLines > 0)
                {
                    lines = lines.TakeLast(limitLines).ToList();
                }

                return new LogContent
                {
                    Lines = lines
                };
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Fetching log content failed.");
                throw;
            }
        }
    }
}
