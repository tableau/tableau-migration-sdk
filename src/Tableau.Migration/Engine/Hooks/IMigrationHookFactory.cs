using System;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for an object that can create a migration hook object.
    /// </summary>
    public interface IMigrationHookFactory
    {
        /// <summary>
        /// Creates a hook for the given hook type.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <param name="services">The migration-scoped DI services available.</param>
        /// <returns>The created hook.</returns>
        /// <exception cref="InvalidCastException">If the factory cannot create a hook of the hook type.</exception>
        THook Create<THook>(IServiceProvider services);
    }
}
