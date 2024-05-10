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
    /// Class representing a metrics response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_ref_metrics.htm#list_metrics_for_site">Tableau API Reference</see> for documentation.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class MetricsResponse : PagedTableauServerResponse<MetricsResponse.MetricType>
    {
        /// <summary>
        /// Gets or sets the metrics for the response.
        /// </summary>
        [XmlArray("metrics")]
        [XmlArrayItem("metric")]
        public override MetricType[] Items { get; set; } = Array.Empty<MetricType>();

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class MetricType
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
            /// Gets or sets the webpage URL for the response.
            /// </summary>
            [XmlAttribute("webpageUrl")]
            public string? WebpageUrl { get; set; }

            /// <summary>
            /// Gets or sets the description for the response.
            /// </summary>
            [XmlAttribute("description")]
            public string? Description { get; set; }

            /// <summary>
            /// Gets or sets the suspended for the response.
            /// </summary>
            [XmlAttribute("suspended")]
            public bool Suspended { get; set; }

            /// <summary>
            /// Gets or sets the created timestamp for the response.
            /// </summary>
            [XmlAttribute("createdAt")]
            public string? CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the updated timestamp for the response.
            /// </summary>
            [XmlAttribute("updatedAt")]
            public string? UpdatedAt { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("project")]
            public ProjectType? Project { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public UserType? Owner { get; set; }

            /// <summary>
            /// Gets or sets the underlyingView for the response.
            /// </summary>
            [XmlElement("underlyingView")]
            public UnderlyingViewType? UnderlyingView { get; set; }

            /// <summary>
            /// Gets or sets the tags for the response.
            /// </summary>
            [XmlArray("tags")]
            [XmlArrayItem("tag")]
            public TagType[] Tags { get; set; } = Array.Empty<TagType>();

            #region Object specific types
            /// <summary>
            /// Class representing a REST API Project response.
            /// </summary>
            public class ProjectType
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
            }

            /// <summary>
            /// Class representing a REST API user response.
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
            /// Class representing a REST API tag response.
            /// </summary>
            public class TagType
            {
                /// <summary>
                /// Gets or sets the label for the response.
                /// </summary>
                [XmlAttribute("label")]
                public string? Label { get; set; }
            }

            /// <summary>
            /// Class representing a REST API underlyingView response.
            /// </summary>
            public class UnderlyingViewType
            {
                /// <summary>
                /// Gets or sets the ID for the response.
                /// </summary>
                [XmlAttribute("id")]
                public Guid Id { get; set; }
            }
            #endregion
        }
    }
}