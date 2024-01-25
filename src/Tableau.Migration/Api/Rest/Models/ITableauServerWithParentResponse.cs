using Tableau.Migration.Api.Rest.Models.Types;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface representing REST API responses with a single item 
    /// that has a parent.
    /// </summary>
    /// <typeparam name="TItem">The response's item type.</typeparam>
    public interface ITableauServerWithParentResponse<TItem> : ITableauServerResponse<TItem>
    {
        /// <summary>
        /// Gets the parent content item.
        /// </summary>
        ParentContentType? Parent { get; set; }
    }
}
