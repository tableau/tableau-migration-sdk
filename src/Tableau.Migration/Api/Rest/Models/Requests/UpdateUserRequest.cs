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
    /// Class representing an update user request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#update_user">Tableau API Reference</see> for documentation.
    /// </para>
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
        /// <param name="newSiteRole">The new Site Role for the user.</param>
        /// <param name="newfullName">(Optional) The new Full Name for the user.</param>
        /// <param name="newEmail">(Optional) The new email address for the user.</param>
        /// <param name="newPassword">(Optional) The new password for the user.</param>
        /// <param name="newAuthSetting">(Optional) The new email Auth Setting for the user.</param>
        public UpdateUserRequest(string newSiteRole,
                                 string? newfullName = null,
                                 string? newEmail = null,
                                 string? newPassword = null,
                                 string? newAuthSetting = null)
        {
            User = new UserType { SiteRole = newSiteRole };

            if (newfullName != null)
                User.FullName = newfullName;

            if (newEmail != null)
                User.Email = newEmail;

            if (newPassword != null)
                User.Password = newPassword;

            if (newAuthSetting != null)
                User.AuthSetting = newAuthSetting;
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
            /// Gets or sets the fullName for the request.
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
            /// Gets or sets the SiteRole for the request.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// Gets or sets the authSetting for the request.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }

        }
    }
}
