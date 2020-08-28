using System.Threading;
using System.Threading.Tasks;

namespace Alidu.Core.Startup
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}