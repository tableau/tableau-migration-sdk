using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Default <see cref="IMigrationPipelineFactory"/> implementation.
    /// </summary>
    public class MigrationPipelineFactory : IMigrationPipelineFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Creates a new <see cref="MigrationPipelineFactory"/> object.
        /// </summary>
        /// <param name="services">A service provider to create pipelines with.</param>
        public MigrationPipelineFactory(IServiceProvider services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public IMigrationPipeline Create(IMigrationPlan plan)
        {
            switch (plan.PipelineProfile)
            {
                case PipelineProfile.ServerToCloud:
                    return _services.GetRequiredService<ServerToCloudMigrationPipeline>();
                default:
                    throw new ArgumentException($"Cannot create a migration pipeline for profile {plan.PipelineProfile}");
            }
        }
    }
}
