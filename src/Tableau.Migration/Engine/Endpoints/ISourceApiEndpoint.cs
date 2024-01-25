namespace Tableau.Migration.Engine.Endpoints
{
    /// <summary>
    /// Interface for an object that represents a location to move Tableau data from.
    /// </summary>
    public interface ISourceApiEndpoint : ISourceEndpoint, IMigrationApiEndpoint
    { }
}
