namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface representing REST API responses with multiple items.
    /// </summary>
    /// <typeparam name="TItems">The response's item type.</typeparam>
    public interface ITableauServerListResponse<TItems>
    {
        /// <summary>
        /// Gets the item for the response.
        /// </summary>
        TItems[] Items { get; set; }
    }
}
