using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a class representing the current authentication token.
    /// </summary>
    public interface IAuthenticationTokenProvider
    {
        /// <summary>
        /// Event that fires when an authentication token refresh is requested.
        /// </summary>
        event AsyncEventHandler? RefreshRequestedAsync;

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        string? Token { get; }

        /// <summary>
        /// Sets the authentication token.
        /// </summary>
        /// <param name="token">The authentication token received from the server.</param>
        void Set(string token);

        /// <summary>
        /// Clears the authentication token.
        /// </summary>
        void Clear();

        /// <summary>
        /// Requests an authentication token refresh.
        /// </summary>
        /// <param name="cancel"> A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task RequestRefreshAsync(CancellationToken cancel);
    }
}