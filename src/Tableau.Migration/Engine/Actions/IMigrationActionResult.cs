namespace Tableau.Migration.Engine.Actions
{
    /// <summary>
    /// <see cref="IResult"/> object for a migration action.
    /// </summary>
    public interface IMigrationActionResult : IResult
    {
        /// <summary>
        /// Gets whether or not to perform the next action in the pipeline.
        /// </summary>
        bool PerformNextAction { get; }

        /// <summary>
        /// Creates a new <see cref="IMigrationActionResult"/> object while modifying the <see cref="PerformNextAction"/> value.
        /// </summary>
        /// <param name="performNextAction">Whether or not to perform the next action in the pipeline.</param>
        /// <returns>The new <see cref="IMigrationActionResult"/> object.</returns>
        IMigrationActionResult ForNextAction(bool performNextAction);
    }
}
