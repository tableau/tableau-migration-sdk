namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for an object that has an owner user reference.
    /// </summary>
    public interface IWithOwnerType
    {
        /// <summary>
        /// Gets the owner for the response.
        /// </summary>
        IOwnerType? Owner { get; }
    }
}
