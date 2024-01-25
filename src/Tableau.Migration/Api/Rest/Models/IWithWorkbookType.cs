namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface for an object that has an owner user reference.
    /// </summary>
    public interface IWithWorkbookReferenceType
    {
        /// <summary>
        /// Gets the owner for the response.
        /// </summary>
        IWorkbookReferenceType? Workbook { get; }
    }
}
