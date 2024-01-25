using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a workbook update response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateWorkbookResponse : TableauServerResponse<UpdateWorkbookResponse.WorkbookType>
    {
        /// <summary>
        /// Gets or sets the workbook object.
        /// </summary>
        [XmlElement("workbook")]
        public override WorkbookType? Item { get; set; }

        /// <summary>
        /// Type for the workbook object.
        /// </summary>
        public class WorkbookType : IRestIdentifiable
        {
            /// <summary>
            /// Gets or sets the id for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }
            
            /// <summary>
            /// Gets or sets the description for the response.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the content URL for the response.
            /// </summary>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            /// <summary>
            /// Gets or sets the show tabs option for the response.
            /// </summary>
            [XmlAttribute("showTabs")]
            public bool ShowTabs { get; set; }

            /// <summary>
            /// Gets or sets the creation date/time for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the update date/time for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the encrypt extracts flag for the response.
            /// </summary>
            [XmlAttribute("encryptExtracts")]
            public bool EncryptExtracts { get; set; }
        }
    }
}
