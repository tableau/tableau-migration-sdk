namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface representing REST API responses with a single item.
    /// </summary>
    /// <typeparam name="TItem">The response's item type.</typeparam>
    public interface ITableauServerResponse<TItem>
    {
        /// <summary>
        /// Gets the item for the response.
        /// </summary>
        TItem? Item { get; set; }
    }
}
