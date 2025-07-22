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
    /// Class representing a paged REST API group set response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class GroupSetsResponse : PagedTableauServerResponse<GroupSetsResponse.GroupSetType>
    {
        /// <summary>
        /// Gets or sets the group sets for the response.
        /// </summary>
        [XmlArray("groupSets")]
        [XmlArrayItem("groupSet")]
        public override GroupSetType[] Items { get; set; } = Array.Empty<GroupSetType>();

        /// <summary>
        /// Class representing a REST API group set response.
        /// </summary>
        public class GroupSetType : IGroupSetType
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
            /// Gets or sets the count of groups in the group set.
            /// </summary>
            [XmlAttribute("groupCount")]
            public int GroupCount { get; set; }
        }
    }
}
