using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Preparation
{
    /// <summary>
    /// Interface for an object that can prepare a content item for publishing.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    /// <typeparam name="TPublish">The publish type.</typeparam>
    public interface IContentItemPreparer<TContent, TPublish>
        where TPublish : class
    {
        /// <summary>
        /// Prepares a content item for publishing.
        /// </summary>
        /// <param name="item">The item to prepare.</param>
        /// <param name="cancel">A cancellation token to obye.</param>
        /// <returns>The preparation result.</returns>
        Task<IResult<TPublish>> PrepareAsync(ContentMigrationItem<TContent> item, CancellationToken cancel);
    }
}
