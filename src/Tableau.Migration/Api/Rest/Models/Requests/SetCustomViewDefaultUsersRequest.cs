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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing a set custom view as default for users request.
    /// </para>
    /// <para>
    /// See Tableau API Reference
    /// <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_workbooks_and_views.htm#set_custom_view_as_default_for_users"></see> 
    /// documentation for details.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SetCustomViewDefaultUsersRequest : TableauServerRequest
    {
        /// <summary>
        /// Creates a new <see cref="SetCustomViewDefaultUsersRequest"/> instance.
        /// </summary>
        public SetCustomViewDefaultUsersRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="SetCustomViewDefaultUsersRequest"/> instance.
        /// </summary>
        public SetCustomViewDefaultUsersRequest(IEnumerable<IContentReference> users)
        {
            Users = users.Select(user => new UserType(user)).ToArray();
        }

        /// <summary>
        /// Gets or sets the user for the request.
        /// </summary>
        [XmlArray("users")]
        [XmlArrayItem("user")]
        public UserType[] Users { get; set; } = Array.Empty<UserType>();

        /// <summary>
        /// The user type in the API request body.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// Gets or sets the id for the request.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Constructor to build from <see cref="IUser"/>
            /// </summary>
            /// <param name="user">The <see cref="IUser"/> object.</param>
            public UserType(IContentReference user)
            {
                Id = user.Id;
            }

            /// <summary>
            /// Constructor to build from <see cref="IUser"/>
            /// </summary>
            public UserType()
            { }
        }

    }
}