//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Xml.Serialization;
using Tableau.Migration.Api.Simulation;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a connections query response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_data_sources.htm#query_data_source_connections">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ConnectionsResponse : TableauServerListResponse<ConnectionsResponse.ConnectionType>
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public ConnectionsResponse()
        { }

        /// <summary>
        /// Gets or sets the embedded connections for the response.
        /// </summary>
        [XmlArray("connections")]
        [XmlArrayItem("connection")]
        public override ConnectionType[] Items { get; set; } = Array.Empty<ConnectionType>();

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
                if (credentials != null)
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
