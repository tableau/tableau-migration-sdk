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

using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a server info response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ServerInfoResponse : TableauServerResponse<ServerInfoResponse.ServerInfoType>
    {
        /// <summary>
        /// Gets or sets the server info for the response.
        /// </summary>
        [XmlElement("serverInfo")]
        public override ServerInfoType? Item { get; set; }

        /// <summary>
        /// Creates a new <see cref="ServerInfoResponse"/> instance.
        /// </summary>
        public ServerInfoResponse()
        {
            //Do not remove, needed for serialization.
        }

        /// <summary>
        /// Creates a new <see cref="ServerInfoResponse"/> instance.
        /// </summary>
        /// <param name="restApiVersion">The server's REST API version.</param>
        /// <param name="productVersion">The server's product version.</param>
        /// <param name="buildVersion">The server's build version.</param>
        internal ServerInfoResponse(string restApiVersion, string productVersion, string buildVersion)
        {
            Item = new ServerInfoType(restApiVersion, productVersion, buildVersion);
        }

        /// <summary>
        /// Creates a new <see cref="ServerInfoResponse"/> instance.
        /// </summary>
        /// <param name="version">The server's version information.</param>
        internal ServerInfoResponse(TableauServerVersion version)
            : this(version.RestApiVersion, version.ProductVersion, version.BuildVersion)
        { }

        /// <summary>
        /// Creates a new <see cref="ServerInfoResponse"/> instance.
        /// </summary>
        /// <param name="error">The error for the response</param>
        internal ServerInfoResponse(Error error)
            : base(error)
        { }

        /// <summary>
        /// Class representing a server info response.
        /// </summary>
        public class ServerInfoType
        {
            /// <summary>
            /// Gets or sets the product version for the response.
            /// </summary>
            [XmlElement("productVersion")]
            public ProductVersionType? ProductVersion { get; set; }

            /// <summary>
            /// Gets or sets the REST API version for the response.
            /// </summary>
            [XmlElement("restApiVersion")]
            public RestApiVersionType? RestApiVersion { get; set; }

            /// <summary>
            /// Creates a new <see cref="ServerInfoType"/> instance.
            /// </summary>
            public ServerInfoType()
            {
                //Do not remove, needed for serialization.
            }

            /// <summary>
            /// Creates a new <see cref="ServerInfoType"/> instance.
            /// </summary>
            /// <param name="restApiVersion">The server's REST API version.</param>
            /// <param name="productVersion">The server's product version.</param>
            /// <param name="buildVersion">The server's build version.</param>
            internal ServerInfoType(string restApiVersion, string productVersion, string buildVersion)
            {
                RestApiVersion = new RestApiVersionType
                {
                    Version = restApiVersion
                };

                ProductVersion = new ProductVersionType
                {
                    Version = productVersion,
                    Build = buildVersion
                };
            }

            /// <summary>
            /// Creates a new <see cref="ServerInfoType"/> instance.
            /// </summary>
            /// <param name="version">The server's version information.</param>
            internal ServerInfoType(TableauServerVersion version)
                : this(version.RestApiVersion, version.ProductVersion, version.BuildVersion)
            { }

            /// <summary>
            /// Class representing a produce version response.
            /// </summary>
            public class ProductVersionType
            {
                /// <summary>
                /// Gets or sets the build version for the response.
                /// </summary>
                [XmlAttribute("build")]
                public string? Build { get; set; }

                /// <summary>
                /// Gets or sets the product version for the response.
                /// </summary>
                [XmlText]
                public string? Version { get; set; }
            }

            /// <summary>
            /// Class representing a REST API version response.
            /// </summary>
            public class RestApiVersionType
            {
                /// <summary>
                /// Gets or sets the version for the 
                /// </summary>
                [XmlText]
                public string? Version { get; set; }
            }
        }
    }
}
