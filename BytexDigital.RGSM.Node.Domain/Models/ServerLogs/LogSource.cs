using System;

namespace BytexDigital.RGSM.Node.Domain.Models.ServerLogs
{
    public class LogSource
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public long SizeInBytes { get; set; }
        public DateTimeOffset TimeLastUpdated { get; set; }
    }
}
