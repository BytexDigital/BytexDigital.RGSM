using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Domain.Models.Console;

namespace BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces
{
    public interface IProvidesConsoleOutput
    {
        Task<List<ConsoleOutputContent>> GetConsoleOutputAsync(List<string> types = default, int lastNLines = -1, CancellationToken cancellationToken = default);
    }
}
