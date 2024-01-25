using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration
{
    /// <summary>
    /// Delegate for asynchronous events.
    /// </summary>
    /// <param name="cancel"> A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns></returns>
    public delegate Task AsyncEventHandler(CancellationToken cancel);
}
