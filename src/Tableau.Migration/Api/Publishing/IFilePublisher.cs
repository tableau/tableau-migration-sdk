using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Publishing
{
    /// <summary>
    /// Interface for file publisher classes.
    /// </summary>
    public interface IFilePublisher<TPublishOptions, TPublishResult>
        where TPublishOptions : IPublishFileOptions
        where TPublishResult : class, IContentReference
    {
        /// <summary>
        /// Publishes the file with the specified options.
        /// </summary>
        /// <param name="options">The publish options</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        Task<IResult<TPublishResult>> PublishAsync(TPublishOptions options, CancellationToken cancel);
    }
}