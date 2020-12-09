using System;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;

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
        public AsyncManualResetEvent ProcessedEvent { get; set; }

        public void MarkAsProcessed()
        {
            ProcessedEvent.Set();
        }

        public enum Status
        {
            Queued,
            Processing,
            Completed
        }
    }
}
