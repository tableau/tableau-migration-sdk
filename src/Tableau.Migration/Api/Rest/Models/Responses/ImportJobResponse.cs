using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing an import job response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ImportJobResponse : TableauServerResponse<ImportJobResponse.ImportJobType>
    {
        /// <summary>
        /// Gets or sets the job for the response.
        /// </summary>
        [XmlElement("job")]
        public override ImportJobType? Item { get; set; }

        /// <summary>
        /// Class representing a job response.
        /// </summary>
        public class ImportJobType : IRestIdentifiable
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the mode for the response.
            /// </summary>
            [XmlAttribute("mode")]
            public string? Mode { get; set; }

            /// <summary>
            /// Gets or sets the type for the response.
            /// </summary>
            [XmlAttribute("type")]
            public string? Type { get; set; }

            /// <summary>
            /// Gets or sets the created timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }
            /// <summary>
            /// Gets or sets the finish code for the response.
            /// </summary>
            [XmlAttribute("finishCode")]
            public int FinishCode { get; set; }

            /// <summary>
            /// Gets or sets the progress for the response.
            /// </summary>
            [XmlAttribute("progress")]
            public int Progress { get; set; }
        }
    }
}
