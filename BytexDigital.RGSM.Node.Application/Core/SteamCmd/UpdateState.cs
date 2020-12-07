using System;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Core.SteamCmd
{
    public class UpdateState
    {
        public int QueuePosition { get; set; }
        public double Progress { get; set; }
        public Status State { get; set; }
        public Task Task { get; set; }
        public Exception FailureException { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }

        public enum Status
        {
            Queued,
            Processing,
            Completed
        }
    }
}
