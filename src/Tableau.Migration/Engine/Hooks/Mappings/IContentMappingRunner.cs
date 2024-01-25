using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Interface for an object that can run mappings.
    /// </summary>
    public interface IContentMappingRunner
    {
        /// <summary>
        /// Executes all mappings for the content type in order.
        /// </summary>
        /// <typeparam name="TContent">The content type.</typeparam>
        /// <param name="context">The mapping context.</param>
        /// <param name="cancel">A cancellation token to obey.</param>
        /// <returns>The result context with the mapped content location.</returns>
        Task<ContentMappingContext<TContent>> ExecuteAsync<TContent>(ContentMappingContext<TContent> context, CancellationToken cancel)
            where TContent : IContentReference;
    }
}
