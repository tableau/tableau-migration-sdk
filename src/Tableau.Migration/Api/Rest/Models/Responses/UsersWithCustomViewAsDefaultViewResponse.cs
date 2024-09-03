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
    /// Class representing a list of users with a custom view as the default view response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UsersWithCustomViewAsDefaultViewResponse : PagedTableauServerResponse<UsersWithCustomViewAsDefaultViewResponse.UserType>
    {
        /// <summary>
        /// Gets or sets the users for the response.
        /// </summary>
        [XmlArray("users")]
        [XmlArrayItem("user")]
        public override UserType[] Items { get; set; } = Array.Empty<UserType>();

        /// <summary>
        /// Class representing a REST API user response.
        /// </summary>
        public class UserType : IRestIdentifiable
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }
        }
    }
}
