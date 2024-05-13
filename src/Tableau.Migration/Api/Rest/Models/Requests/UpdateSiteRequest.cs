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
using Tableau.Migration.Content;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// <para>
    /// Class representing an update site request.
    /// </para>
    /// <para>
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_site.htm#update_site">Tableau API Reference</see> for documentation.
    /// </para>
    /// </summary>
    [XmlType(XmlTypeName)]
    public class UpdateSiteRequest : TableauServerRequest
    {
        /// <summary>
        /// Creates a new <see cref="UpdateSiteRequest"/> object.
        /// </summary>
        public UpdateSiteRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="UpdateSiteRequest"/> object.
        /// </summary>
        /// <param name="update">The settings to update.</param>
        public UpdateSiteRequest(ISiteSettingsUpdate update)
        {
            Site = new()
            {
                ExtractEncryptionMode = update.ExtractEncryptionMode
            };
        }

        /// <summary>
        /// Gets or sets the site for the request.
        /// </summary>
        [XmlElement("site")]
        public SiteType? Site { get; set; }

        /// <summary>
        /// The site type in the API request body.
        /// </summary>
        public class SiteType
        {
            /// <summary>
            /// Gets or sets the extract encryption mode for the request.
            /// </summary>
            [XmlAttribute("extractEncryptionMode")]
            public string? ExtractEncryptionMode { get; set; }
        }
    }
}
