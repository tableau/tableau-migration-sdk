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
    /// Class representing a local group creation request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#create_group">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateLocalGroupRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the group for the request.
        /// </summary>
        [XmlElement("group")]
        public GroupType? Group { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateLocalGroupRequest"/> instance.
        /// </summary>
        public CreateLocalGroupRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateLocalGroupRequest"/> instance.
        /// </summary>
        /// <param name="name">The group name.</param>
        /// <param name="minimumSiteRole">The minimum site role.</param>
        public CreateLocalGroupRequest(string name, string? minimumSiteRole)
        {
            Group = new GroupType
            {
                Name = name,
                MinimumSiteRole = minimumSiteRole
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
            /// Gets or sets the minimum site role for the request.
            /// </summary>
            [XmlAttribute("minimumSiteRole")]
            public string? MinimumSiteRole { get; set; }
        }
    }
}
