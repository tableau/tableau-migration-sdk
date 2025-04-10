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
    /// Class representing an get embedded credentials retrieve keychain request.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class RetrieveKeychainRequest : TableauServerRequest
    {
        /// <summary>
        /// Creates a new <see cref="RetrieveKeychainRequest"/> object.
        /// </summary>
        public RetrieveKeychainRequest()
        { }

        /// <summary>
        /// Creates a new <see cref="RetrieveKeychainRequest"/> object from <see cref="IDestinationSiteInfo"/>.
        /// </summary>
        public RetrieveKeychainRequest(IDestinationSiteInfo destinationSiteInfo)
        {
            DestinationSiteUrlNamespace = destinationSiteInfo.ContentUrl;
            DestinationSiteLuid = destinationSiteInfo.SiteId;
            DestinationServerUrl = destinationSiteInfo.SiteUrl;
        }

        /// <summary>
        /// Gets or sets the site name for the destination.
        /// </summary>
        [XmlElement("destinationSiteUrlNamespace")]
        public string? DestinationSiteUrlNamespace { get; set; }

        /// <summary>
        /// Gets or sets the site ID for the destination.
        /// </summary>
        [XmlElement("destinationSiteLuid")]
        public Guid DestinationSiteLuid { get; set; }

        /// <summary>
        /// Gets or sets the Url for the destination Tableau instance.
        /// </summary>
        [XmlElement("destinationServerUrl")]
        public string? DestinationServerUrl { get; set; }
    }
}
