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
    /// Class representing a users response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_users_and_groups.htm#get_users_on_site">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UsersResponse : PagedTableauServerResponse<UsersResponse.UserType>
    {
        /// <summary>
        /// Gets or sets the users for the response.
        /// </summary>
        [XmlArray("users")]
        [XmlArrayItem("user")]
        public override UserType[] Items { get; set; } = Array.Empty<UserType>();

        /// <summary>
        /// Class representing a user on the response.
        /// </summary>
        public class UserType : IRestIdentifiable
        {
            /// <summary>
            /// Gets or sets the ID for the response.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the name for the response.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the fullName for the response.
            /// </summary>
            [XmlAttribute("fullName")]
            public string? FullName { get; set; }

            /// <summary>
            /// Gets or sets the email for the response.
            /// </summary>
            [XmlAttribute("email")]
            public string? Email { get; set; }

            /// <summary>
            /// Gets or sets the SiteRole for the response.
            /// </summary>
            [XmlAttribute("siteRole")]
            public string? SiteRole { get; set; }

            /// <summary>
            /// Gets or sets the lastLogin for the response.
            /// </summary>
            [XmlAttribute("lastLogin")]
            public string? LastLogin { get; set; }

            /// <summary>
            /// Gets or sets the externalAuthUserId for the response.
            /// </summary>
            [XmlAttribute("externalAuthUserId")]
            public string? ExternalAuthUserId { get; set; }

            /// <summary>
            /// Gets or sets the authSetting for the response.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }

            /// <summary>
            /// Gets or sets the language for the response.
            /// </summary>
            [XmlAttribute("language")]
            public string? Language { get; set; }

            /// <summary>
            /// Gets or sets the locale for the response.
            /// </summary>
            [XmlAttribute("locale")]
            public string? Locale { get; set; }

            /// <summary>
            /// Gets or sets the domain for the response.
            /// </summary>
            [XmlElement("domain")]
            public DomainType? Domain { get; set; }

            #region - Object specific types -

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class DomainType
            {
                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }
            #endregion
        }
    }
}