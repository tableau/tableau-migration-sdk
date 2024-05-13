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
    /// <para>
    /// Class representing a server session response.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_server.htm#get-current-server-session">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class ServerSessionResponse : TableauServerResponse<ServerSessionResponse.SessionType>
    {
        /// <summary>
        /// Gets or sets the session for the response.
        /// </summary>
        [XmlElement("session")]
        public override SessionType? Item { get; set; }

        /// <summary>
        /// Class representing a session response.
        /// </summary>
        public class SessionType
        {
            /// <summary>
            /// Gets or sets the site for the response.
            /// </summary>
            [XmlElement("site")]
            public SiteType? Site { get; set; }

            /// <summary>
            /// Gets or sets the user for the response.
            /// </summary>
            [XmlElement("user")]
            public UserType? User { get; set; }

            /// <summary>
            /// Class representing a site response.
            /// </summary>
            public class SiteType
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
                /// Gets or sets the content URL for the response.
                /// </summary>
                [XmlAttribute("contentUrl")]
                public string? ContentUrl { get; set; }

                /// <summary>
                /// Gets or sets the extract encryption mode for the response.
                /// </summary>
                [XmlAttribute("extractEncryptionMode")]
                public string? ExtractEncryptionMode { get; set; }
            }

            /// <summary>
            /// Class representing a user response.
            /// </summary>
            public class UserType
            {
                /// <summary>
                /// Gets or sets the authentication setting for the response.
                /// </summary>
                [XmlAttribute("authSetting")]
                public string? AuthSetting { get; set; }

                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// Gets or sets the external authentication ID for the response.
                /// </summary>
                [XmlAttribute("externalAuthUserId")]
                public string? ExternalAuthUserId { get; set; }

                /// <summary>
                /// Gets or sets the last login for the response.
                /// </summary>
                [XmlAttribute("lastLogin")]
                public string? LastLogin { get; set; }

                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }

                /// <summary>
                /// Gets or sets the site role for the response.
                /// </summary>
                [XmlAttribute("siteRole")]
                public string? SiteRole { get; set; }
            }
        }
    }
}
