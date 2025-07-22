//
//  Copyright (c) 2025, Salesforce, Inc.
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
    /// Class representing a group set creation request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#create_group_set">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CreateGroupSetRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the group set for the request.
        /// </summary>
        [XmlElement("groupSet")] // API is case sensitive.
        public GroupSetType? GroupSet { get; set; }

        /// <summary>
        /// Creates a new <see cref="CreateGroupSetRequest"/> instance.
        /// </summary>
        public CreateGroupSetRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="CreateGroupSetRequest"/> instance.
        /// </summary>
        /// <param name="name">The group set name.</param>
        public CreateGroupSetRequest(string name)
        {
            GroupSet = new() { Name = name };
        }

        /// <summary>
        /// Class representing a group set request.
        /// </summary>
        public class GroupSetType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }
        }
    }
}
