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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an import users to site from CSV request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#import_users_to_site_from_csv">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ImportUsersFromCsvRequest : TableauServerRequest
    {
        /// <summary>
        /// Creates a new <see cref="ImportUsersFromCsvRequest"/> object with a single empty user, 
        /// which is required for import if no other user payload is provided.
        /// </summary>
        public ImportUsersFromCsvRequest()
            : this([new UserType()])
        { }

        /// <summary>
        /// Creates a new <see cref="ImportUsersFromCsvRequest"/> with a single user to specify a default authentication type.
        /// </summary>
        /// <param name="authentication">The default authentication type to request.</param>
        public ImportUsersFromCsvRequest(UserAuthenticationType authentication)
            : this([new UserType(null, authentication)])
        { }

        /// <summary>
        /// Creates a new <see cref="ImportUsersFromCsvRequest"/> with per-user authentication settings.
        /// </summary>
        /// <param name="users">The users to include in the payload.</param>
        public ImportUsersFromCsvRequest(IEnumerable<UserType> users)
        {
            Users = users.ToArray();
        }

        /// <summary>
        /// Gets or sets the users for the request.
        /// </summary>
        [XmlElement("user")]
        public UserType[] Users { get; set; } = Array.Empty<UserType>();

        /// <summary>
        /// The user type in the API request body.
        /// </summary>
        public class UserType
        {
            /// <summary>
            /// Creates a new <see cref="UserType"/>.
            /// </summary>
            public UserType()
            { }

            /// <summary>
            /// Creates a new <see cref="UserType"/>.
            /// </summary>
            /// <param name="name">The user name.</param>
            /// <param name="authentication">The user authentication type.</param>
            public UserType(string? name, UserAuthenticationType authentication)
            {
                Name = name;
                AuthSetting = authentication.AuthenticationType;
                IdpConfigurationId = authentication.IdpConfigurationId?.ToString();
            }

            /// <summary>
            /// Gets or sets the username for the item.
            /// Use a null name to apply an empty user or default authentication type.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the authentication type for the item.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }

            /// <summary>
            /// Gets or sets the IdP configuration ID for the item.
            /// </summary>
            [XmlAttribute("idpConfigurationId")]
            public string? IdpConfigurationId { get; set; }
        }
    }
}
