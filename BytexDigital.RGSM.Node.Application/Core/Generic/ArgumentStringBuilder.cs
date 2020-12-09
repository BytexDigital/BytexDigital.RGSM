using System.Threading;
using System.Threading.Tasks;

namespace BytexDigital.RGSM.Node.Application.Core.Generic
{
    public class ArgumentStringBuilder
    {
        public virtual async Task<string> BuildAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(string.Empty);
        }
    }
}
