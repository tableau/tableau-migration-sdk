using System;

namespace Tableau.Migration.Engine.Pipelines
{
    /// <summary>
    /// Interface for an object that can build <see cref="IMigrationPipeline"/> objects.
    /// </summary>
    public interface IMigrationPipelineFactory
    {
        /// <summary>
        /// Creates a pipeline for the given plan.
        /// </summary>
        /// <param name="plan">The plan to create the pipeline for.</param>
        /// <returns>The created pipeline.</returns>
        /// <exception cref="ArgumentException">If the plan's pipeline profile is not supported by the factory.</exception>
        IMigrationPipeline Create(IMigrationPlan plan);
    }
}
