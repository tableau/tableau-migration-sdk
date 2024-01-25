using System.Collections.Generic;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;

namespace Tableau.Migration.Interop.Hooks.Filters
{
    /// <summary>
    /// Interface for an object that can synchronously filter content of a specific content type, to determine which content to migrate.
    /// </summary>
    /// <typeparam name="TContent"><inheritdoc/></typeparam>
    public interface ISyncContentFilter<TContent>
        : ISyncMigrationHook<IEnumerable<ContentMigrationItem<TContent>>>, IContentFilter<TContent>
        where TContent : IContentReference
    {
        /// <summary>
        /// Executes a filter callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous filter.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next filter or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new IEnumerable<ContentMigrationItem<TContent>>? Execute(IEnumerable<ContentMigrationItem<TContent>> ctx);
    }
}
