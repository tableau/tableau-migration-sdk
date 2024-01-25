using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Base implementation for <see cref="IMigrationHookRunner"/>
    /// </summary>
    internal abstract class MigrationHookRunnerBase
    {
        protected readonly IServiceProvider Services;
        protected readonly IMigrationPlan Plan;

        /// <summary>
        /// Default constructor for this base class.
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="services"></param>
        protected MigrationHookRunnerBase(IMigrationPlan plan, IServiceProvider services)
        {
            Plan = plan;
            Services = services;
        }

        /// <inheritdoc/>
        public async Task<TContext> ExecuteAsync<THook, TContext>(TContext context, CancellationToken cancel) where THook : IMigrationHook<TContext>
        {
            var currentContext = context;

            var hookFactories = GetFactoryCollection<THook, TContext>();
            foreach (var hookFactory in hookFactories)
            {
                var hook = hookFactory.Create<IMigrationHook<TContext>>(Services);

                var inputContext = currentContext;
                currentContext = (await hook.ExecuteAsync(inputContext, cancel).ConfigureAwait(false)) ?? inputContext;
            }

            return currentContext;
        }

        /// <summary>
        /// Abstract method to get the factory collection from the appropriate plan builder property.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <typeparam name="TContext">The hook context type.</typeparam>
        /// <returns></returns>
        protected abstract ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>() where THook : IMigrationHook<TContext>;
    }
}