using System.Xml.Serialization;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an update connection request.
    /// </para>
    /// <para>
    /// See Tableau API Reference 
    /// <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#update_data_source_connection">data sources</see> and 
    /// <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#update_workbook_connection">workbooks</see> 
    /// documentation for details.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateConnectionRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateConnectionRequest() { }

        /// <summary>
        /// Build from required properties.
        /// </summary>
        /// <param name="updateOptions">
        /// The update connection options that are in 
        /// <see cref="IUpdateConnectionOptions"/>.
        /// </param>
        public UpdateConnectionRequest(IUpdateConnectionOptions updateOptions)
        {
            Connection = new ConnectionType(updateOptions);
        }

        /// <summary>
        /// Gets or sets the user for the request.
        /// </summary>
        [XmlElement("connection")]
        public ConnectionType? Connection { get; set; }

        /// <summary>
        /// The user type in the API request body.
        /// </summary>
        public class ConnectionType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public ConnectionType()
            { }

            /// <summary>
            /// Builds a <see cref="ConnectionType"/> from required properties.
            /// </summary>
            /// <param name="updateOptions">
            /// The update connection options that are in 
            /// <see cref="IUpdateConnectionOptions"/>.
            /// </param>         
            public ConnectionType(IUpdateConnectionOptions updateOptions)
            {
                ServerAddress = updateOptions.ServerAddress;
                ServerPort = updateOptions.ServerPort;
                ConnectionUsername = updateOptions.ConnectionUsername;
                Password = updateOptions.Password;
                EmbedPasswordFlag = updateOptions.EmbedPassword;
                QueryTaggingEnabledFlag = updateOptions.QueryTaggingEnabled;
            }

            /// <inheritdoc/>
            [XmlAttribute("serverAddress")]
            public string? ServerAddress { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("serverPort")]
            public string? ServerPort { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("userName")]
            public string? ConnectionUsername { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("password")]
            public string? Password { get; set; }

            /// <inheritdoc/>
            [XmlIgnore]
            public bool? EmbedPasswordFlag { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("embedPassword")]
            public string? EmbedPassword
            {
                get => EmbedPasswordFlag.HasValue ? EmbedPasswordFlag.ToString() : null;
                set => EmbedPasswordFlag = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?);
            }

            /// <inheritdoc/>
            [XmlIgnore]
            public bool? QueryTaggingEnabledFlag { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("queryTaggingEnabled")]
            public string? QueryTaggingEnabled
            {
                get => QueryTaggingEnabledFlag.HasValue ? QueryTaggingEnabledFlag.ToString() : null;
                set => QueryTaggingEnabledFlag = !string.IsNullOrEmpty(value) ? bool.Parse(value) : default(bool?);
            }

        }
    }
}
