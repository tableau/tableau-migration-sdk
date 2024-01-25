namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that represents a location to move Tableau data to.
    /// </summary>
    public interface IDestinationApiEndpoint : IDestinationEndpoint, IMigrationApiEndpoint
    { }
}
