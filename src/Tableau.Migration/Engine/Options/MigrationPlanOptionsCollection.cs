using System;
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Default <see cref="IMigrationPlanOptionsCollection"/> implementation.
    /// </summary>
    public class MigrationPlanOptionsCollection : IMigrationPlanOptionsCollection
    {
        /// <summary>
        /// Gets an empty <see cref="MigrationPlanOptionsCollection"/>.
        /// </summary>
        public static readonly MigrationPlanOptionsCollection Empty = new(ImmutableDictionary<Type, Func<IServiceProvider, object?>>.Empty);

        private readonly ImmutableDictionary<Type, Func<IServiceProvider, object?>> _optionFactories;

        /// <summary>
        /// Creates a new <see cref="MigrationPlanOptionsCollection"/> object.
        /// </summary>
        /// <param name="optionFactories">The options factories by option type.</param>
        public MigrationPlanOptionsCollection(ImmutableDictionary<Type, Func<IServiceProvider, object?>> optionFactories)
        {
            _optionFactories = optionFactories;
        }

        /// <inheritdoc />
        public TOptions? Get<TOptions>(IServiceProvider services)
        {
            if (_optionFactories.TryGetValue(typeof(TOptions), out var optionsFactory))
            {
                return (TOptions?)optionsFactory(services);
            }

            return default;
        }
    }
}
