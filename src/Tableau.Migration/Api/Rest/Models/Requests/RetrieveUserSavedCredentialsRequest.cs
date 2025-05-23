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
using System.Xml.Serialization;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api.Rest.Models.Requests
{
    /// <summary>
    /// Class representing a retrieve saved creds request.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class RetrieveUserSavedCredentialsRequest : TableauServerRequest
    {
        /// <summary>
        /// The default parameterless constructor.
        /// </summary>
        public RetrieveUserSavedCredentialsRequest()
        { }

        /// <summary>
        /// Builds <see cref="RetrieveUserSavedCredentialsRequest"/> from <see cref="IDestinationSiteInfo"/>.
        /// </summary>
        public RetrieveUserSavedCredentialsRequest(IDestinationSiteInfo options)
        {
            DestinationSiteUrlNamespace = options.ContentUrl;
            DestinationSiteLuid = options.SiteId;
            DestinationServerUrl = options.SiteUrl;
        }

        /// <summary>
        /// Gets or sets the destinationSiteUrlNamespace for the request.
        /// </summary>
        [XmlElement("destinationSiteUrlNamespace")]
        public string? DestinationSiteUrlNamespace { get; set; }

        /// <summary>
        /// Gets or sets the destinationSiteLuid for the request.
        /// </summary>
        [XmlElement("destinationSiteLuid")]
        public Guid DestinationSiteLuid { get; set; }

        /// <summary>
        /// Gets or sets the destinationServerUrl for the request.
        /// </summary>
        [XmlElement("destinationServerUrl")]
        public string? DestinationServerUrl { get; set; }
    }
}
