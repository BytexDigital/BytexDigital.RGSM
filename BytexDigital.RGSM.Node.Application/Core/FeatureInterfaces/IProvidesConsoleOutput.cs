using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Core.FeatureInterfaces
{
    public interface IProvidesConsoleOutput
    {
        Task<(string, string)> GetConsoleOutputAsync(CancellationToken cancellationToken);
    }
}
