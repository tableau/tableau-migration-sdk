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

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing a group import request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#create_group">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ImportGroupRequest : TableauServerRequest
    {
        internal const string ActiveDirectorySource = "ActiveDirectory";

        /// <summary>
        /// Gets or sets the group for the request.
        /// </summary>
        [XmlElement("group")]
        public GroupType? Group { get; set; }

        /// <summary>
        /// Creates a new <see cref="ImportGroupRequest"/> instance.
        /// </summary>
        public ImportGroupRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="ImportGroupRequest"/> instance.
        /// </summary>
        /// <param name="name">The group name to import.</param>
        /// <param name="source">The import's source.</param>
        /// <param name="domainName">The import's domain name.</param>
        /// <param name="minimumSiteRole">The minimum site role for imported users.</param>
        /// <param name="grantLicenseMode">The mode for automatically applying licenses for group members.</param>
        public ImportGroupRequest(string name, string source, string domainName, string minimumSiteRole, string? grantLicenseMode)
        {
            Group = new GroupType
            {
                Name = name,
                Import = new GroupType.ImportType
                {
                    Source = source,
                    DomainName = domainName,
                    GrantLicenseMode = grantLicenseMode,
                    SiteRole = minimumSiteRole,
                }
            };
        }

        /// <summary>
        /// Class representing a group request.
        /// </summary>
        public class GroupType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the import for the response.
            /// </summary>
            [XmlElement("import")]
            public ImportType? Import { get; set; }

            /// <summary>
            /// Class representing a REST API import request.
            /// </summary>
            public class ImportType
            {
                /// <summary>
                /// Gets or sets the source for the request.
                /// </summary>
                [XmlAttribute("source")]
                public string? Source { get; set; }

                /// <summary>
                /// Gets or sets the source for the request.
                /// </summary>
                [XmlAttribute("domainName")]
                public string? DomainName { get; set; }

                /// <summary>
                /// Gets or sets the grant license mode for the request.
                /// </summary>
                [XmlAttribute("grantLicenseMode")]
                public string? GrantLicenseMode { get; set; }

                /// <summary>
                /// Gets or sets the site role for the request.
                /// </summary>
                [XmlAttribute("siteRole")]
                public string? SiteRole { get; set; }
            }
        }
    }
}
