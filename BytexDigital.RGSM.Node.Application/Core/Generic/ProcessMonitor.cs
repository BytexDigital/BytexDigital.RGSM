using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Core.Generic
{
    public class ProcessMonitor
    {
        private string _serverDirectory;
        private string _executablePath;
        private readonly ArgumentStringBuilder _argumentStringBuilder;
        private readonly TimeSpan _shutdownTimeout;
        private Process _process;

        private string PidFilePath => Path.Combine(_serverDirectory, ".rgsm", "pid");

        public ProcessMonitor(ArgumentStringBuilder argumentStringBuilder, TimeSpan? shutdownTimeout = default)
        {
            _argumentStringBuilder = argumentStringBuilder;
            _shutdownTimeout = shutdownTimeout ?? TimeSpan.FromSeconds(10);
        }

        public Task ConfigureAsync(string serverDirectory, string executablePath)
        {
            _serverDirectory = serverDirectory;
            _executablePath = executablePath;

            if (_process == default)
            {
                // Detect if there is a PID file, in case so, read it and try to get the process object by PID
                if (File.Exists(PidFilePath))
                {
                    try
                    {
                        var readPid = int.Parse(File.ReadAllText(PidFilePath));

                        _process = Process.GetProcessById(readPid);
                    }
                    catch
                    {
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            var psi = new ProcessStartInfo(_executablePath);
            psi.Arguments = await _argumentStringBuilder.BuildAsync();

            _process = Process.Start(psi);

            try
            {
                File.WriteAllText(PidFilePath, _process.Id.ToString());
            }
            catch
            {
            }
        }

        public async Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            if (_process == null || _process.HasExited) return;

            try
            {
                _process.CloseMainWindow();
            }
            catch
            {
            }

            var timeoutAt = DateTimeOffset.UtcNow.Add(_shutdownTimeout);

            while (DateTimeOffset.UtcNow < timeoutAt)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                try
                {
                    if (_process.HasExited) break;
                }
                catch
                {

                }
            }

            try
            {
                _process.Kill();
            }
            catch
            {

            }

            try
            {
                if (File.Exists(PidFilePath)) File.Delete(PidFilePath);
            }
            catch
            {
            }

            _process = default;
        }

        public async Task<bool> IsRunningAsync()
        {
            try
            {
                return await Task.FromResult(_process != null && !_process.HasExited);
            }
            catch
            {
                return false;
            }
        }
    }
}
