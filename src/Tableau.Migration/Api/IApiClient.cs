using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for unauthenticated API clients.
    /// </summary>
    public interface IApiClient
    {
        /// <summary>
        /// Signs into Tableau Server
        /// </summary>
        /// <param name="cancel">The cancellation token</param>
        /// <returns>An authenticated <see cref="ISitesApiClient"/></returns>
        Task<IAsyncDisposableResult<ISitesApiClient>> SignInAsync(CancellationToken cancel);

        /// <summary>
        /// Gets the version information for the Tableau Server
        /// </summary>
        /// <param name="cancel">The cancellation token</param>
        /// <returns>The information for the current Tableau Server</returns>
        Task<IResult<IServerInfo>> GetServerInfoAsync(CancellationToken cancel);
    }
}
