using System;
using System.Collections.Immutable;

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Default <see cref="IMigrationPlanOptionsBuilder"/> implementation.
    /// </summary>
    public class MigrationPlanOptionsBuilder : IMigrationPlanOptionsBuilder
    {
        private readonly ImmutableDictionary<Type, Func<IServiceProvider, object?>>.Builder _optionFactories;

        /// <summary>
        /// Creates a new <see cref="MigrationPlanOptionsBuilder"/> object.
        /// </summary>
        public MigrationPlanOptionsBuilder()
        {
            _optionFactories = ImmutableDictionary.CreateBuilder<Type, Func<IServiceProvider, object?>>();
        }

        #region - IMigrationPlanOptionsBuilder Implementation -

        /// <inheritdoc />
        public IMigrationPlanOptionsBuilder Configure<TOptions>(TOptions opts)
            => Configure(services => opts);

        /// <inheritdoc />
        public IMigrationPlanOptionsBuilder Configure<TOptions>(Func<IServiceProvider, TOptions> factory)
        {
            _optionFactories[typeof(TOptions)] = (services) => factory(services);
            return this;
        }

        /// <inheritdoc />
        public IMigrationPlanOptionsCollection Build()
            => new MigrationPlanOptionsCollection(_optionFactories.ToImmutable());

        #endregion
    }
}
