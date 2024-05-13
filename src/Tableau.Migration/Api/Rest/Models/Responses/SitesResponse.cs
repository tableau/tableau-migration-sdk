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
    /// Class representing a site response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_site.htm#query_sites">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class SitesResponse : PagedTableauServerResponse<SitesResponse.SiteType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("sites")]
        [XmlArrayItem("site")]
        public override SiteType[] Items { get; set; } = Array.Empty<SiteType>();

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
            /// Gets or sets the adminMode for the response.
            /// </summary>
            [XmlAttribute("adminMode")]
            public string? AdminMode { get; set; }

            /// <summary>
            /// Gets or sets the disableSubscriptions for the response.
            /// </summary>
            [XmlAttribute("disableSubscriptions")]
            public bool DisableSubscriptions { get; set; }

            /// <summary>
            /// Gets or sets state for the response.
            /// </summary>
            [XmlAttribute("state")]
            public string? State { get; set; }

            /// <summary>
            /// Gets or sets the revisionHistoryEnabled for the response.
            /// </summary>
            [XmlAttribute("RevisionHistoryEnabled")]
            public bool RevisionHistoryEnabled { get; set; }

            /// <summary>
            /// Gets or sets revisionLimit for the response.
            /// </summary>
            [XmlAttribute("revisionLimit")]
            public string? RevisionLimit { get; set; }

            /// <summary>
            /// Gets or sets subscribeOthersEnabled for the response.
            /// </summary>
            [XmlAttribute("subscribeOthersEnabled")]
            public bool SubscribeOthersEnabledv { get; set; }

            /// <summary>
            /// Gets or sets allowSubscriptionAttachments for the response.
            /// </summary>
            [XmlAttribute("allowSubscriptionAttachments")]
            public bool AllowSubscriptionAttachmentsv { get; set; }

            /// <summary>
            /// Gets or sets guestAccessEnabled for the response.
            /// </summary>
            [XmlAttribute("guestAccessEnabled")]
            public bool GuestAccessEnabled { get; set; }

            /// <summary>
            /// Gets or sets commentingEnabled for the response.
            /// </summary>
            [XmlAttribute("commentingEnabled")]
            public bool CommentingEnabled { get; set; }

            /// <summary>
            /// Gets or sets editingFlowsEnabled for the response.
            /// </summary>
            [XmlAttribute("editingFlowsEnabled")]
            public bool EditingFlowsEnabled { get; set; }

            /// <summary>
            /// Gets or sets schedulingFlowsEnabled for the response.
            /// </summary>
            [XmlAttribute("schedulingFlowsEnabled")]
            public bool SchedulingFlowsEnabled { get; set; }

            /// <summary>
            /// Gets or sets extractEncryptionMode for the response.
            /// </summary>
            [XmlAttribute("extractEncryptionMode")]
            public string? ExtractEncryptionMode { get; set; }

            /// <summary>
            /// Gets or sets catalogingEnabled for the response.
            /// </summary>
            [XmlAttribute("catalogingEnabled")]
            public bool CatalogingEnabled { get; set; }

            /// <summary>
            /// Gets or sets derivedPermissionsEnabled for the response.
            /// </summary>
            [XmlAttribute("derivedPermissionsEnabled")]
            public bool DerivedPermissionsEnabled { get; set; }

            /// <summary>
            /// Gets or sets requestAccessEnabled for the response.
            /// </summary>
            [XmlAttribute("requestAccessEnabled")]
            public bool RequestAccessEnabled { get; set; }

            /// <summary>
            /// Gets or sets runNowEnabled for the response.
            /// </summary>
            [XmlAttribute("runNowEnabled")]
            public bool RunNowEnabled { get; set; }

            /// <summary>
            /// Gets or sets isDataAlertsEnabled for the response.
            /// </summary>
            [XmlAttribute("isDataAlertsEnabled")]
            public bool IsDataAlertsEnabled { get; set; }

            /// <summary>
            /// Gets or sets askDataMode for the response.
            /// </summary>
            [XmlAttribute("askDataMode")]
            public string? askDataMode { get; set; }

            /// <summary>
            /// Gets or sets useDefaultTimeZone for the response.
            /// </summary>
            [XmlAttribute("useDefaultTimeZone")]
            public bool UseDefaultTimeZone { get; set; }

            /// <summary>
            /// Gets or sets timeZone for the response.
            /// </summary>
            [XmlAttribute("timeZone")]
            public string? TimeZone { get; set; }

            /// <summary>
            /// Gets or sets dqwSubscriptionsEnabled for the response.
            /// </summary>
            [XmlAttribute("dqwSubscriptionsEnabled")]
            public string? DqwSubscriptionsEnabled { get; set; }
        }
    }
}