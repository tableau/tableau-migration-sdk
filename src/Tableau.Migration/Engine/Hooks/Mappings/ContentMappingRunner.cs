using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Mappings
{
    internal class ContentMappingRunner
        : MigrationHookRunnerBase, IContentMappingRunner
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the mappings.</param>
        /// <param name="services">Service provider context to resolve the mappings used by the runner.</param>
        public ContentMappingRunner(IMigrationPlan plan, IServiceProvider services)
            : base(plan, services)
        { }

        /// <inheritdoc />
        public async Task<ContentMappingContext<TContent>> ExecuteAsync<TContent>(ContentMappingContext<TContent> location, CancellationToken cancel)
            where TContent : IContentReference
            => await ExecuteAsync<IContentMapping<TContent>, ContentMappingContext<TContent>>(location, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Mappings.GetHooks<THook>();
    }
}
