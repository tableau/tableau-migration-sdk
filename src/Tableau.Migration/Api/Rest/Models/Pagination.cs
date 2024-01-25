using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Class representing a paged REST API response's pagination.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Gets or sets the response's current page number.
        /// </summary>
        [XmlAttribute("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the response's page size.
        /// </summary>
        [XmlAttribute("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of available items.
        /// </summary>
        [XmlAttribute("totalAvailable")]
        public int TotalAvailable { get; set; }
    }
}
