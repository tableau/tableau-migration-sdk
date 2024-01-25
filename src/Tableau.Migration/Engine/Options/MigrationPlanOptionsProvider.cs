using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tableau.Migration.Engine.Options
{
    /// <summary>
    /// Default <see cref="IMigrationPlanOptionsProvider{TOptions}"/> implementation.
    /// </summary>
    /// <typeparam name="TOptions">The options type.</typeparam>
    public class MigrationPlanOptionsProvider<TOptions> : IMigrationPlanOptionsProvider<TOptions>
        where TOptions : class, new()
    {
        private readonly IServiceProvider _services;
        private readonly IMigrationPlan _plan;

        /// <summary>
        /// Creates a new <see cref="MigrationPlanOptionsProvider{TOptions}"/>
        /// </summary>
        /// <param name="services">The migration services.</param>
        /// <param name="plan">The migration plan.</param>
        public MigrationPlanOptionsProvider(IServiceProvider services, IMigrationPlan plan)
        {
            _services = services;
            _plan = plan;
        }

        /// <inheritdoc />
        public TOptions Get()
        {
            //Try plan-registered options first.
            var planOptions = _plan.Options.Get<TOptions>(_services);
            if (planOptions is not null)
            {
                return planOptions;
            }

            var serviceOptions = _services.GetService<TOptions>();
            if (serviceOptions is not null)
            {
                return serviceOptions;
            }

            var serviceIOptions = _services.GetService<IOptions<TOptions>>();
            if (serviceIOptions is not null)
            {
                return serviceIOptions.Value;
            }

            return new();
        }
    }
}
