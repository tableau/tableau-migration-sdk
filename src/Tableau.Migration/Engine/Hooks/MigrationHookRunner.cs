using System;
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Hooks
{
    internal class MigrationHookRunner : MigrationHookRunnerBase, IMigrationHookRunner
    {
        /// <summary>
        /// Default constructor for this class.
        /// </summary>
        /// <param name="plan">Migration plan used to run the hooks.</param>
        /// <param name="services">Service provider context to resolve the hooks used by the runner.</param>
        public MigrationHookRunner(IMigrationPlan plan, IServiceProvider services) : base(plan, services)
        { }

        /// <summary>
        /// Abstract method to get the factory collection from the appropriate plan builder property.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <typeparam name="TContext">The hook context type.</typeparam>
        /// <returns></returns>
        protected sealed override ImmutableArray<IMigrationHookFactory> GetFactoryCollection<THook, TContext>()
        {
            return Plan.Hooks.GetHooks<THook>();
        }
    }
}
