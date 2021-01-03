using System;

namespace BytexDigital.RGSM.Node.TransferObjects.Models.ServerLogs
{
    public class LogSourceDto
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public long SizeInBytes { get; set; }
        public DateTimeOffset TimeLastUpdated { get; set; }
    }
}
