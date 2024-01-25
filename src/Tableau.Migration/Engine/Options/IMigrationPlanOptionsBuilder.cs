using System;

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Interface for an object that can build a set of per-plan options.
    /// </summary>
    public interface IMigrationPlanOptionsBuilder
    {
        /// <summary>
        /// Sets the configuration for a given options type.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="opts">The options.</param>
        /// <returns>The same options builder, for fluent API usage.</returns>
        IMigrationPlanOptionsBuilder Configure<TOptions>(TOptions opts);

        /// <summary>
        /// Sets the configuration for a given options type.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="factory">A factory function to create the options type.</param>
        /// <returns>The same options builder, for fluent API usage.</returns>
        IMigrationPlanOptionsBuilder Configure<TOptions>(Func<IServiceProvider, TOptions> factory);

        /// <summary>
        /// Builds the options collection.
        /// </summary>
        /// <returns>The options collection.</returns>
        IMigrationPlanOptionsCollection Build();
    }
}
