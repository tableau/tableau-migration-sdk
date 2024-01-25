using System;
using System.ComponentModel.DataAnnotations;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Options;

namespace Tableau.Migration.Engine
{
    /// <summary>
    /// Default <see cref="IMigrationPlan"/> implementation.
    /// </summary>
    /// <param name="PlanId"><inheritdoc /></param>
    /// <param name="PipelineProfile"><inheritdoc /></param>
    /// <param name="Options"><inheritdoc /></param>
    /// <param name="Source"><inheritdoc /></param>
    /// <param name="Destination"><inheritdoc /></param>
    /// <param name="Hooks"><inheritdoc /></param>
    /// <param name="Mappings"><inheritdoc /></param>
    /// <param name="Filters"><inheritdoc /></param>
    /// <param name="Transformers"><inheritdoc /></param>
    public record MigrationPlan(
        Guid PlanId,
        [property: EnumDataType(typeof(PipelineProfile))] PipelineProfile PipelineProfile,
        IMigrationPlanOptionsCollection Options,
        IMigrationPlanEndpointConfiguration Source,
        IMigrationPlanEndpointConfiguration Destination,
        IMigrationHookFactoryCollection Hooks,
        IMigrationHookFactoryCollection Mappings,
        IMigrationHookFactoryCollection Filters,
        IMigrationHookFactoryCollection Transformers
    ) : IMigrationPlan
    { }
}
