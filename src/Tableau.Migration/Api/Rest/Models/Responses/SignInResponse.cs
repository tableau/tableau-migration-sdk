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
    /// Class representing a sign-in response.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_authentication.htm#sign_in">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SignInResponse : TableauServerResponse<SignInResponse.CredentialsType>
    {
        /// <summary>
        /// Gets or sets the credentials for the response.
        /// </summary>
        [XmlElement("credentials")]
        public override CredentialsType? Item { get; set; }

        /// <summary>
        /// Creates a new <see cref="SignInResponse"/> instance.
        /// </summary>
        public SignInResponse()
        {
            //Do not remove, needed for serialization.
        }

        /// <summary>
        /// Creates a new <see cref="SignInResponse"/> instance.
        /// </summary>
        /// <param name="siteId">The current site's ID.</param>
        /// <param name="contentUrl">The current site's content URL.</param>
        /// <param name="userId">The signed-in user's ID.</param>
        /// <param name="token">The authentication token.</param>
        internal SignInResponse(Guid siteId, string contentUrl, Guid userId, string token)
        {
            Item = new CredentialsType(siteId, contentUrl, userId, token);
        }

        /// <summary>
        /// Class representing a credentials response.
        /// </summary>
        public class CredentialsType
        {
            /// <summary>
            /// Gets or sets the authentication token for the response.
            /// </summary>
            [XmlAttribute("token")]
            public string? Token { get; set; }

            /// <summary>
            /// Gets or sets the user for the response.
            /// </summary>
            [XmlElement("user")]
            public UserType? User { get; set; }

            /// <summary>
            /// Gets or sets the site for the response.
            /// </summary>
            [XmlElement("site")]
            public SiteType? Site { get; set; }

            /// <summary>
            /// Creates a new <see cref="CredentialsType"/> instance.
            /// </summary>
            public CredentialsType()
            {
                //Do not remove, needed for serialization.
            }

            /// <summary>
            /// Creates a new <see cref="CredentialsType"/> instance.
            /// </summary>
            /// <param name="siteId">The current site's ID.</param>
            /// <param name="contentUrl">The current site's content URL.</param>
            /// <param name="userId">The signed-in user's ID.</param>
            /// <param name="token">The authentication token.</param>
            internal CredentialsType(Guid siteId, string contentUrl, Guid userId, string token)
            {
                Site = new SiteType
                {
                    Id = siteId,
                    ContentUrl = contentUrl
                };

                User = new UserType
                {
                    Id = userId
                };

                Token = token;
            }

            /// <summary>
            /// Class representing a user response.
            /// </summary>
            public class UserType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }

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
                /// Gets or sets the content URL for the response.
                /// </summary>
                [XmlAttribute("contentUrl")]
                public string? ContentUrl { get; set; }
            }
        }
    }
}
