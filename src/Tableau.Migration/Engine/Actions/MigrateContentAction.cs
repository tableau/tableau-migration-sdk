using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// Default <see cref="IMigrateContentAction{TContent}"/> implementation.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public class MigrateContentAction<TContent> : IMigrateContentAction<TContent>
        where TContent : class, IContentReference
    {
        private readonly IContentMigrator<TContent> _contentMigrator;

        /// <summary>
        /// Creates a new <see cref="MigrateContentAction{TContent}"/> object.
        /// </summary>
        /// <param name="pipeline">A pipeline to use to get the content migrator.</param>
        public MigrateContentAction(IMigrationPipeline pipeline)
        {
            _contentMigrator = pipeline.GetMigrator<TContent>();
        }

        /// <inheritdoc />
        public async Task<IMigrationActionResult> ExecuteAsync(CancellationToken cancel)
        {
            var migrateResult = await _contentMigrator.MigrateAsync(cancel).ConfigureAwait(false);

            return MigrationActionResult.FromResult(migrateResult);
        }
    }
}
