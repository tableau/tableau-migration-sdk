namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for an object that has a project reference.
    /// </summary>
    public interface IWithProjectType
    {
        /// <summary>
        /// Gets the project for the response.
        /// </summary>
        IProjectReferenceType? Project { get; }
    }
}
