using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Common base class for <see cref="IMigrationPipeline"/> implementations that can create and execute actions.
    /// </summary>
    public abstract class MigrationPipelineBase : IMigrationPipeline
    {
        /// <summary>
        /// Gets the service provider.
        /// </summary>
        protected IServiceProvider Services { get; }

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineBase"/> object.
        /// </summary>
        /// <param name="services">A DI service provider to create actions with.</param>
        protected MigrationPipelineBase(IServiceProvider services)
        {
            Services = services;
        }

        #region - Protected Methods -

        /// <summary>
        /// Creates an action of the given type.
        /// </summary>
        /// <typeparam name="TAction">The action type.</typeparam>
        /// <returns>The created action.</returns>
        protected TAction CreateAction<TAction>()
            where TAction : IMigrationAction
            => Services.GetRequiredService<TAction>();

        /// <summary>
        /// Creates an action to migrate content of a given type.
        /// </summary>
        /// <typeparam name="TContent">The content type to migrate.</typeparam>
        /// <returns>The created content migration action.</returns>
        protected IMigrateContentAction<TContent> CreateMigrateContentAction<TContent>()
            where TContent : class, IContentReference
            => CreateAction<MigrateContentAction<TContent>>();

        /// <summary>
        /// Builds the pipeline of actions to execute in order.
        /// </summary>
        /// <returns>The actions to execute in order.</returns>
        protected abstract IEnumerable<IMigrationAction> BuildPipeline();

        #endregion

        #region - IMigrationPipeline Implementation -

        /// <inheritdoc />
        public ImmutableArray<IMigrationAction> BuildActions() => BuildPipeline().ToImmutableArray();

        /// <inheritdoc />
        public virtual IContentMigrator<TContent> GetMigrator<TContent>()
            where TContent : class, IContentReference
        {
            return Services.GetRequiredService<ContentMigrator<TContent>>();
        }

        /// <inheritdoc />
        public virtual IContentBatchMigrator<TContent> GetBatchMigrator<TContent>()
            where TContent : class, IContentReference
        {
            return Services.GetRequiredService<ItemPublishContentBatchMigrator<TContent>>();
        }

        /// <inheritdoc />
        public virtual IContentItemPreparer<TContent, TPublish> GetItemPreparer<TContent, TPublish>()
            where TContent : class
            where TPublish : class
        {
            switch (typeof(TContent))
            {
                case Type source when source == typeof(TPublish):
                    return (IContentItemPreparer<TContent, TPublish>)Services.GetRequiredService<SourceContentItemPreparer<TContent>>();
                default:
                    return Services.GetRequiredService<EndpointContentItemPreparer<TContent, TPublish>>();
            }
        }

        /// <inheritdoc />
        public virtual IContentReferenceCache CreateDestinationCache<TContent>()
            where TContent : IContentReference
        {
            switch (typeof(TContent))
            {
                case Type project when project == typeof(IProject):
                    return Services.GetRequiredService<BulkDestinationProjectCache>();
                default:
                    return Services.GetRequiredService<BulkDestinationCache<TContent>>();
            }
            
        }

        /// <inheritdoc />
        public virtual IMappedContentReferenceFinder<TContent> CreateDestinationFinder<TContent>()
            where TContent : IContentReference
        {
            return Services.GetRequiredService<ManifestDestinationContentReferenceFinder<TContent>>();
        }

        /// <inheritdoc />
        public virtual ILockedProjectCache GetDestinationLockedProjectCache()
            => Services.GetRequiredService<BulkDestinationProjectCache>();

        #endregion
    }
}
