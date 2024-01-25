using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a list REST API response.
    /// </summary>
    public abstract class TableauServerListResponse<TItem> : TableauServerResponse, ITableauServerListResponse<TItem>
    {
        /// <inheritdoc/>
        [XmlIgnore] // Ignored so the derived class can set the XmlElement name.
        public abstract TItem[] Items { get; set; }
    }
}
