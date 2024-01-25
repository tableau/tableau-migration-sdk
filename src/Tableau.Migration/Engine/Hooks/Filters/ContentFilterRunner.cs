using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Filters
{
    internal class ContentFilterRunner : MigrationHookRunnerBase, IContentFilterRunner
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the filters.</param>
        /// <param name="services">Service provider context to resolve the filters used by the runner.</param>
        public ContentFilterRunner(IMigrationPlan plan, IServiceProvider services) : base(plan, services)
        { }

        public async Task<IEnumerable<ContentMigrationItem<TContent>>> ExecuteAsync<TContent>(IEnumerable<ContentMigrationItem<TContent>> context, CancellationToken cancel)
            where TContent : IContentReference
            => await ExecuteAsync<IContentFilter<TContent>, IEnumerable<ContentMigrationItem<TContent>>>(context, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Filters.GetHooks<THook>();
    }
}
