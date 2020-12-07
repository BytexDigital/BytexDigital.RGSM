using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Core.Generic
{
    public class ProcessMonitor
    {
        private readonly string _executablePath;
        private readonly ArgumentStringBuilder _argumentStringBuilder;
        private Process _process;

        public ProcessMonitor(string executablePath, ArgumentStringBuilder argumentStringBuilder)
        {
            _executablePath = executablePath;
            _argumentStringBuilder = argumentStringBuilder;
        }

        public async Task RunAsync()
        {
            var psi = new ProcessStartInfo(_executablePath);
            psi.Arguments = await _argumentStringBuilder.BuildAsync();

            _process = Process.Start(psi);
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            if (_process == null || _process.HasExited) return;

            _process.Close();

            await _process.WaitForExitAsync(
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token).Token);

            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }

        public async Task<bool> IsRunningAsync()
        {
            // We should add some smarter detection here since we might lose the link to this process at some point e.g. when our daemon restarts
            return await Task.FromResult(_process != null && !_process.HasExited);
        }
    }
}
