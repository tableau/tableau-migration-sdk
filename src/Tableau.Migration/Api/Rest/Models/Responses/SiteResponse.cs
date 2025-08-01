﻿//
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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a site response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SiteResponse : TableauServerResponse<SiteResponse.SiteType>
    {
        /// <summary>
        /// Gets or sets the site for the response.
        /// </summary>
        [XmlElement("site")]
        public override SiteType? Item { get; set; }

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class SiteType : IRestIdentifiable, IApiContentUrl
        {
            /// <summary>
            /// Gets or sets the ID of the site.
            /// </summary>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            /// <summary>
            /// Gets or sets the name of the site.
            /// </summary>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            /// <summary>
            /// Gets or sets the content URL.
            /// </summary>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            /// <summary>
            /// Gets or sets the site extract encryption mode.
            /// </summary>
            [XmlAttribute("extractEncryptionMode")]
            public string? ExtractEncryptionMode { get; set; }
            
            /// <summary>
            /// Gets or sets whether subscriptions are disabled.
            /// </summary>
            [XmlAttribute("disableSubscriptions")]
            public bool DisableSubscriptions { get; set; }

            /// <summary>
            /// Gets or sets whether group sets are enabled.
            /// </summary>
            [XmlAttribute("groupSetsEnabled")]
            public bool GroupSetsEnabled { get; set; }
        }
    }
}
