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
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing an update user request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#update_user">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateUserRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public UpdateUserRequest() { }

        /// <summary>
        /// Builds the Update request for a user.
        /// </summary>
        /// <param name="newSiteRole">The new site role for the user.</param>
        /// <param name="newFullName">(Optional) The new full name for the user.</param>
        /// <param name="newEmail">(Optional) The new email address for the user.</param>
        /// <param name="newPassword">(Optional) The new password for the user.</param>
        /// <param name="newAuthentication">(Optional) The new authentication for the user.</param>
        public UpdateUserRequest(string newSiteRole,
                                 string? newFullName = null,
                                 string? newEmail = null,
                                 string? newPassword = null,
                                 UserAuthenticationType? newAuthentication = null)
        {
            User = new UserType { SiteRole = newSiteRole };

            if (newFullName is not null)
                User.FullName = newFullName;

            if (newEmail is not null)
                User.Email = newEmail;

            if (newPassword is not null)
                User.Password = newPassword;

            if (newAuthentication is not null)
            {
                // IdP configuration ID and auth setting are mutually exclusive, set the ID if available.
                if (newAuthentication.Value.IdpConfigurationId is not null)
                {
                    User.IdpConfigurationId = newAuthentication.Value.IdpConfigurationId.ToString();
                }
                else
                {
                    User.AuthSetting = newAuthentication.Value.AuthenticationType;
                }
            }
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
            /// Gets or sets the full name for the request.
            /// </summary>
            [XmlAttribute("fullName")]
            public string? FullName { get; set; }

            /// <summary>
            /// Gets or sets the email for the request.
            /// </summary>
            [XmlAttribute("email")]
            public string? Email { get; set; }

            /// <summary>
            /// Gets or sets the password for the request.
            /// </summary>
            [XmlAttribute("password")]
            public string? Password { get; set; }

            /// <summary>
            /// Gets or sets the site role for the request.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// Gets or sets the auth setting for the request.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }

            /// <summary>
            /// Gets or sets the IdP configuration ID for the request.
            /// </summary>
            [XmlAttribute("idpConfigurationId")]
            public string? IdpConfigurationId { get; set; }
        }
    }
}
