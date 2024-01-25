using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    /// <summary>
    /// Interface for an object that can run transformations.
    /// </summary>
    public interface IContentTransformerRunner
    {
        /// <summary>
        /// Executes all transformations for the content type in order.
        /// </summary>
        /// <typeparam name="TPublish">The publishable content type.</typeparam>
        /// <param name="itemToTransform">The items to transform.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The resulting transformed item.</returns>
        Task<TPublish> ExecuteAsync<TPublish>(TPublish itemToTransform, CancellationToken cancel);
    }
}
