using System;
using System.Xml;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Types
{
    /// <summary>
    /// Class that defines the Parent XML element.
    /// </summary>
    public class ParentContentType
    {
        /// <summary>
        /// The Content Type of the parent as defined in <see cref="ParentContentTypeNames"/>
        /// </summary>
        [XmlAttribute("type")]
        public string? Type { get; set; }

        /// <summary>
        /// The id of the parent.
        /// </summary>
        [XmlAttribute("id")]
        public Guid Id { get; set; }
    }
}
