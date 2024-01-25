using System;
using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a job response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class JobResponse : TableauServerResponse<JobResponse.JobType>
    {
        /// <summary>
        /// Gets or sets the job for the response.
        /// </summary>
        [XmlElement("job")]
        public override JobType? Item { get; set; }

        /// <summary>
        /// Class representing a job response.
        /// </summary>
        public class JobType : IRestIdentifiable
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
            /// Gets or sets the updated timestamp for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the completed timestamp for the response.
            /// </summary>
            [XmlAttribute("completedAt")]
            public string? CompletedAt { get; set; }

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

            /// <summary>
            /// Gets or sets the groups for the response.
            /// </summary>
            [XmlArray("statusNotes")]
            [XmlArrayItem("statusNote")]
            public StatusNoteType[] StatusNotes { get; set; } = Array.Empty<StatusNoteType>();

            /// <summary>
            /// Class representing a REST API job status notes response.
            /// </summary>
            public class StatusNoteType
            {
                /// <summary>
                /// Gets or sets the type for the response.
                /// </summary>
                [XmlAttribute("type")]
                public string? Type { get; set; }

                /// <summary>
                /// Gets or sets the value for the response.
                /// </summary>
                [XmlAttribute("value")]
                public string? Value { get; set; }

                /// <summary>
                /// Gets or sets the text for the response.
                /// </summary>
                [XmlAttribute("text")]
                public string? Text { get; set; }
            }
        }
    }
}
