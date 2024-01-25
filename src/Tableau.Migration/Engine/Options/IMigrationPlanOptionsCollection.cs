using System;

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Interface for an object that contains plan-specific options objects.
    /// </summary>
    public interface IMigrationPlanOptionsCollection
    {
        /// <summary>
        /// Gets the options for the given type, 
        /// or null if no options for the given type have been registered.
        /// </summary>
        /// <typeparam name="TOptions">The options type.</typeparam>
        /// <param name="services">A service provider.</param>
        /// <returns>The options for the given type, or null.</returns>
        TOptions? Get<TOptions>(IServiceProvider services);
    }
}
