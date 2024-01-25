﻿using System;
using System.Xml.Serialization;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a connections query response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#query_data_source_connections">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ConnectionResponse : TableauServerResponse<ConnectionResponse.ConnectionType>
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public ConnectionResponse()
        { }

        /// <summary>
        /// Gets or sets the embedded connections for the response.
        /// </summary>        
        [XmlElement("connection")]
        public override ConnectionType? Item { get; set; }

        /// <summary>
        /// Class representing an embedded connection on the response.
        /// </summary>
        [XmlType("connection")]
        public class ConnectionType : IConnectionType
        {
            /// <summary>
            /// The default parameterless constructor.
            /// </summary>
            public ConnectionType()
            { }

            /// <summary>
            /// Constructor to build from <see cref="SimulatedConnection"/>.
            /// </summary>
            /// <param name="response"></param>
            public ConnectionType(SimulatedConnection response)
            {
                Id = response.Id;
                ServerAddress = response.ServerAddress;
                ServerPort = response.ServerPort;
                Type = response.ConnectionType;
                QueryTaggingEnabledFlag = response.QueryTaggingEnabled;

                var credentials = response.Credentials;
                if (credentials is not null)
                {
                    ConnectionUsername = credentials.Name;
                }
            }

            /// <inheritdoc/>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <inheritdoc/>
            [XmlAttribute("type")]
            public string? Type { get; set; }

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
