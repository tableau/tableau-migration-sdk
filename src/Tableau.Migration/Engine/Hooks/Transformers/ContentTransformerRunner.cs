using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks.Transformers
{
    internal class ContentTransformerRunner : MigrationHookRunnerBase, IContentTransformerRunner
    {
        public ContentTransformerRunner(IMigrationPlan plan, IServiceProvider services)
            : base(plan, services)
        { }

        public async Task<TPublish> ExecuteAsync<TPublish>(TPublish itemToTransform, CancellationToken cancel)
            => await ExecuteAsync<IContentTransformer<TPublish>, TPublish>(itemToTransform, cancel).ConfigureAwait(false);

        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
            => Plan.Transformers.GetHooks<THook>();
    }
}
