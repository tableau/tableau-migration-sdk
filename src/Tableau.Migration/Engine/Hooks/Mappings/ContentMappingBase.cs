using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    /// <summary>
    /// Base implementation for an object that can map content of a specific content type.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public abstract class ContentMappingBase<TContent> : IContentMapping<TContent>
        where TContent : IContentReference
    {
        /// <inheritdoc />
        public abstract Task<ContentMappingContext<TContent>?> ExecuteAsync(ContentMappingContext<TContent> ctx, CancellationToken cancel);
    }
}

