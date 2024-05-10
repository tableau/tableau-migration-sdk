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
    /// Class representing an add user to site request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#add_user_to_site">Tableau API Reference</see> /// for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class AddUserToSiteRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public AddUserToSiteRequest() { }

        /// <summary>
        /// Creates a new <see cref="AddUserToSiteRequest"/> instance.
        /// </summary>
        /// <param name="name">The username.</param>
        /// <param name="siteRole">The user's site role.</param>
        /// <param name="authSetting">The user's authentication type.</param>
        public AddUserToSiteRequest(string name, string siteRole, string? authSetting)
        {
            User = new UserType
            {
                Name = name,
                SiteRole = siteRole,
                AuthSetting = authSetting
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
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the site role for the request.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// Gets or sets the authentication type for the request.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }

        }
    }
}
