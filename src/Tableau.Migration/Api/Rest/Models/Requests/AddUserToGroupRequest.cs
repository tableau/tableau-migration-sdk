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

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an add user to group request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#add_user_to_group">Tableau API Reference</see>  for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddUserToGroupRequest : TableauServerRequest
    {
        /// <summary>
        /// Default parameterless constructor.
        /// </summary>
        public AddUserToGroupRequest() { }

        /// <summary>
        /// Creates a new <see cref="AddUserToSiteRequest"/> instance.
        /// </summary>
        /// <param name="id">The username.</param>        
        public AddUserToGroupRequest(Guid id)
        {
            User = new UserType
            {
                Id = id
            };
        }

        /// <summary>
        /// Gets or sets the user for the request.
        /// </summary>
        [XmlElement("user")]
        public UserType? User { get; set; }

        /// <summary>
        /// The user type in the API request body.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// Gets or sets the name for the request.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

        }
    }
}
