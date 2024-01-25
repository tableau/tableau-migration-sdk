namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for configuration necessary to connect to a migration endpoint defined in the a <see cref="IMigrationPlan"/>.
    /// </summary>
    public interface IMigrationPlanEndpointConfiguration
    {
        /// <summary>
        /// Validates that the endpoint configuration has enough information to connect.
        /// </summary>
        /// <returns>The validation result.</returns>
        IResult Validate();
    }
}
