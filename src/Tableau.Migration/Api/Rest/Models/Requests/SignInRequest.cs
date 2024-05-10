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
    /// Class representing a sign-in request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_authentication.htm#sign_in">Tableau API Reference</see> for documentation
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SignInRequest : TableauServerRequest
    {
        /// <summary>
        /// Gets or sets the credentials for the request.
        /// </summary>
        [XmlElement("credentials")]
        public CredentialsType? Credentials { get; set; }

        /// <summary>
        /// Creates a new <see cref="SignInRequest"/> instance.
        /// </summary>
        public SignInRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="SignInRequest"/> instance.
        /// </summary>
        /// <param name="personalAccessTokenName">The personal access token name for the request</param>
        /// <param name="personalAccessToken">The personal access token secret for the request.</param>
        /// <param name="siteContentUrl">The site content URL for the request.</param>
        public SignInRequest(
            string personalAccessTokenName,
            string personalAccessToken,
            string siteContentUrl)
        {
            Credentials = new CredentialsType
            {
                PersonalAccessTokenName = personalAccessTokenName,
                PersonalAccessTokenSecret = personalAccessToken,
                Site = new CredentialsType.SiteType
                {
                    ContentUrl = siteContentUrl
                }
            };
        }

        /// <summary>
        /// Creates a new <see cref="SignInRequest"/> instance.
        /// </summary>
        /// <param name="connectionConfig">The <see cref="TableauSiteConnectionConfiguration"/> containing sign-in information.</param>
        public SignInRequest(TableauSiteConnectionConfiguration connectionConfig)
            : this(connectionConfig.AccessTokenName, connectionConfig.AccessToken, connectionConfig.SiteContentUrl)
        { }

        /// <summary>
        /// Class representing a credentials request.
        /// </summary>
        public class CredentialsType
        {
            /// <summary>
            /// Gets or sets the site for the request.
            /// </summary>
            [XmlElement("site")]
            public SiteType? Site { get; set; }

            /// <summary>
            /// Gets or sets the personal access token name for the request.
            /// </summary>
            [XmlAttribute("personalAccessTokenName")]
            public string? PersonalAccessTokenName { get; set; }

            /// <summary>
            /// Gets or sets the personal access token secret for the request.
            /// </summary>
            [XmlAttribute("personalAccessTokenSecret")]
            public string? PersonalAccessTokenSecret { get; set; }

            /// <summary>
            /// Class representing a site request.
            /// </summary>
            public class SiteType
            {
                /// <summary>
                /// Gets or sets the content URL for the request.
                /// </summary>
                [XmlAttribute("contentUrl")]
                public string? ContentUrl { get; set; }
            }
        }
    }
}
