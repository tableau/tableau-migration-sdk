using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that can pull information to publish with.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public interface IPullApiClient<TContent, TPublish>
        where TPublish : class
    {
        /// <summary>
        /// Pulls enough information to publish the content item.
        /// </summary>
        /// <param name="contentItem">The content item to pull.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The result of the pull operation with the item to publish.</returns>
        Task<IResult<TPublish>> PullAsync(TContent contentItem, CancellationToken cancel);
    }
}
