using Tableau.Migration.Engine.Hooks.Transformers;

namespace Tableau.Migration.Interop.Hooks.Transformers
{
    /// <summary>
    /// Interface for an object that can synchronously modify or transform content of a specific content type during a migration.
    /// </summary>
    /// <typeparam name="TPublish"><inheritdoc/></typeparam>
    public interface ISyncContentTransformer<TPublish>
        : ISyncMigrationHook<TPublish>, IContentTransformer<TPublish>
    {
        /// <summary>
        /// Executes a transformer callback.
        /// </summary>
        /// <param name="ctx">The input context from the migration engine or previous transformer.</param>
        /// <returns>
        /// The context, 
        /// potentially modified to pass on to the next transformer or migration engine, 
        /// or null to continue passing the same context as <paramref name="ctx"/>.
        /// </returns>
        new TPublish? Execute(TPublish ctx);
    }
}
