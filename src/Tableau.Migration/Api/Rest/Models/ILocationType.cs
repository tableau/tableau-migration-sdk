using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Class representing an XML element for the location of a content item that has a name.
    /// </summary>
    public interface ILocationType : IRestIdentifiable, INamedContent
    {
        /// <summary>
        /// Gets or sets the type for the response.
        /// </summary>
        [XmlAttribute("type")]
        public string? Type { get; set; }
    }
}