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
    /// Class representing a custom views response.
    /// 
    /// This is incomplete, there are more attributes than we're saving
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CustomViewsResponse : PagedTableauServerResponse<CustomViewsResponse.CustomViewResponseType>
    {
        /// <summary>
        /// Gets or sets the groups for the response.
        /// </summary>
        [XmlArray("customViews")]
        [XmlArrayItem("customView")]
        public override CustomViewResponseType[] Items { get; set; } = Array.Empty<CustomViewResponseType>();

        /// <summary>
        /// Class representing a site response.
        /// </summary>
        public class CustomViewResponseType
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
            /// Gets or sets shared flag for the response.
            /// </summary>
            [XmlAttribute("shared")]
            public bool Shared { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("view")]
            public ViewType? view { get; set; }

            /// <summary>
            /// Gets or sets the workbook for the response.
            /// </summary>
            [XmlElement("workbook")]
            public WorkbookType? Workbook { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public UserType? Owner { get; set; }

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class ViewType
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
            public class WorkbookType
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

                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }
        }
    }
}
