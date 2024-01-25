using System;

namespace Tableau.Migration.Engine.Hooks
{
    /// <summary>
    /// Default <see cref="IMigrationHookFactory"/> implementation that uses an initializer function.
    /// </summary>
    /// <param name="Factory">The initializer function.</param>
    public record MigrationHookFactory(Func<IServiceProvider, object> Factory) : IMigrationHookFactory
    {
        /// <inheritdoc />
        public THook Create<THook>(IServiceProvider services)
        {
            var result = Factory(services);
            if (!(result is THook hook))
            {
                throw new InvalidCastException($"Cannot create hook type {typeof(THook)} from object of type {result.GetType()}.");
            }

            return hook;
        }
    }
}
