using System;
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Interface for an object that contains <see cref="MigrationHookFactory"/>s registered for each hook type.
    /// </summary>
    public interface IMigrationHookFactoryCollection
    {
        /// <summary>
        /// Gets the <see cref="MigrationHookFactory"/>s for the given hook type.
        /// </summary>
        /// <typeparam name="THook">The hook type.</typeparam>
        /// <returns>An immutable array of the registered hook factories for the given type.</returns>
        ImmutableArray<IMigrationHookFactory> GetHooks<THook>();

        /// <summary>
        /// Gets the <see cref="MigrationHookFactory"/>s for the given hook type.
        /// </summary>
        /// <param name="hookType">The hook type.</param>
        /// <returns>An immutable array of the registered hook factories for the given type.</returns>
        ImmutableArray<IMigrationHookFactory> GetHooks(Type hookType);
    }
}
