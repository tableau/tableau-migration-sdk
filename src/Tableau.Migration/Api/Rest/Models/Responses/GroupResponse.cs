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
    /// Class representing a group response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class GroupResponse : TableauServerResponse<GroupResponse.GroupType>
    {
        /// <summary>
        /// Gets or sets the group for the response.
        /// </summary>
        [XmlElement("group")]
        public override GroupType? Item { get; set; }

        /// <summary>
        /// Class representing a group response.
        /// </summary>
        public class GroupType : IRestIdentifiable
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
            public string? Name { get; }

            /// <summary>
            /// Gets or sets the domain for the response.
            /// </summary>
            [XmlElement("domain")]
            public DomainType? Domain { get; set; }

            /// <summary>
            /// Class representing a domain response.
            /// </summary>
            public class DomainType
            {
                /// <summary>
                /// Gets or sets the name for the response.
                /// </summary>
                [XmlAttribute("name")]
                public string? Name { get; set; }
            }
        }
    }
}
