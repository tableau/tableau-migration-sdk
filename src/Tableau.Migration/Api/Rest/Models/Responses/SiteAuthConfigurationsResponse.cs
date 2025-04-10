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

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// Class representing a site authentication configurations response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_site.htm#list_authentication_configurations">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SiteAuthConfigurationsResponse : TableauServerListResponse<SiteAuthConfigurationsResponse.SiteAuthConfigurationType>
    {
        /// <summary>
        /// Gets or sets the configurations for the response.
        /// </summary>
        [XmlArray("siteAuthConfigurations")]
        [XmlArrayItem("siteAuthConfiguration")]
        public override SiteAuthConfigurationType[] Items { get; set; } = Array.Empty<SiteAuthConfigurationType>();

        /// <summary>
        /// Class representing a site authentication configuration item.
        /// </summary>
        public class SiteAuthConfigurationType
        {
            /// <summary>
            /// Gets or sets the auth setting name for the item.
            /// </summary>
            [XmlAttribute("authSetting")]
            public string? AuthSetting { get; set; }

            /// <summary>
            /// Gets or sets the known provider alias for the item.
            /// </summary>
            [XmlAttribute("knownProviderAlias")]
            public string? KnownProviderAlias { get; set; }

            /// <summary>
            /// Gets or sets the IdP configuration name for the item.
            /// </summary>
            [XmlAttribute("idpConfigurationName")]
            public string? IdpConfigurationName { get; set; }

            /// <summary>
            /// Gets or sets the IdP configuration ID for the item.
            /// </summary>
            [XmlAttribute("idpConfigurationId")]
            public Guid IdpConfigurationId { get; set; }

            /// <summary>
            /// Gets or sets the enabled flag for the item.
            /// </summary>
            [XmlAttribute("enabled")]
            public bool Enabled { get; set; }
        }
    }
}
