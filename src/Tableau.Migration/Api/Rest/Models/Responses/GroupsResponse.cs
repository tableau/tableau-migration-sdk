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
    /// Class representing a paged REST API groups response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class GroupsResponse : PagedTableauServerResponse<GroupsResponse.GroupType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("groups")]
        [XmlArrayItem("group")]
        public override GroupType[] Items { get; set; } = Array.Empty<GroupType>();

        /// <summary>
        /// Class representing a REST API group response.
        /// </summary>
        public class GroupType : IRestIdentifiable
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
            /// Gets or sets the domain for the response.
            /// </summary>
            [XmlElement("domain")]
            public DomainType? Domain { get; set; }

            /// <summary>
            /// Class representing a REST API domain response.
            /// </summary>
            public class DomainType
            {
                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }

            /// <summary>
            /// Gets or sets the import for the response.
            /// </summary>
            [XmlElement("import")]
            public ImportType? Import { get; set; }

            /// <summary>
            /// Class representing a REST API import response.
            /// </summary>
            public class ImportType
            {
                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("domainName")]
                public string? DomainName { get; set; }

                /// <summary>
                /// Gets or sets the site role for the response.
                /// </summary>
                [XmlAttribute("siteRole")]
                public string? SiteRole { get; set; }

                /// <summary>
                /// Gets or sets the grant license mode for the response.
                /// </summary>
                [XmlAttribute("grantLicenseMode")]
                public string? GrantLicenseMode { get; set; }
            }
        }
    }
}
