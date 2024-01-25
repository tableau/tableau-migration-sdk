using Tableau.Migration.Engine.Hooks.Mappings;

namespace Tableau.Migration.Interop.Hooks.Mappings
{
    /// <summary>
    /// Interface for an object that can synchronously map content of a specific content type.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public interface ISyncContentMapping<TContent>
        : ISyncMigrationHook<ContentMappingContext<TContent>>, IContentMapping<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Executes a mapping callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous mapping.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next mapping or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new ContentMappingContext<TContent>? Execute(ContentMappingContext<TContent> ctx);
    }
}
