using System.Xml.Serialization;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a paged REST API response.
    /// </summary>
    public abstract class PagedTableauServerResponse<TItem> : TableauServerListResponse<TItem>, IPageInfo
    {
        /// <summary>
        /// Gets or sets the pagination for the response.
        /// </summary>
        [XmlElement("pagination")]
        public Pagination Pagination { get; set; } = new();

        int IPageInfo.PageNumber => Pagination.PageNumber;

        int IPageInfo.PageSize => Pagination.PageSize;

        int IPageInfo.TotalCount => Pagination.TotalAvailable;
    }
}
