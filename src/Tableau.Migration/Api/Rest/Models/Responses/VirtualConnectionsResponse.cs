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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a virtual connection response.
    /// 
    /// This is incomplete, there are more attributes than we're saving
    /// </summary>
    [XmlType(XmlTypeName)]
    public class VirtualConnectionsResponse : PagedTableauServerResponse<VirtualConnectionsResponse.VirtualConnectionType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("virtualConnections")]
        [XmlArrayItem("virtualConnection")]
        public override VirtualConnectionType[] Items { get; set; } = Array.Empty<VirtualConnectionType>();

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class VirtualConnectionType
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the webpage URL for the response.
            /// </summary>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

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
            /// Gets or sets the Has Extracts for the response.
            /// </summary>
            [XmlAttribute("hasExtracts")]
            public bool HasExtracts { get; set; }

            /// <summary>
            /// Gets or sets the isCertified for the response.
            /// </summary>
            [XmlAttribute("isCertified")]
            public bool IsCertified { get; set; }
        }
    }
}
