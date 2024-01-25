using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for a content typed API client that can publish multiple items as a batch.
    /// </summary>
    /// <typeparam name="TPublish">The content publish type.</typeparam>
    public interface IBatchPublishApiClient<TPublish> : IContentApiClient
    {
        /// <summary>
        /// Publishes a batch of content items.
        /// </summary>
        /// <param name="items">The content items to publish.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The results of the publishing.</returns>
        Task<IResult> PublishBatchAsync(IEnumerable<TPublish> items, CancellationToken cancel);
    }
}
