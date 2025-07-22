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
    /// Class representing a paged REST API workbooks response.
    /// </summary>
    [XmlType(XmlTypeName)]
    public class CollectionsResponse : PagedTableauServerResponse<CollectionsResponse.CollectionType>
    {
        /// <summary>
        /// Gets or sets the workbooks for the response.
        /// </summary>
        [XmlArray("collections")]
        [XmlArrayItem("collection")]
        public override CollectionType[] Items { get; set; } = Array.Empty<CollectionType>();

        /// <summary>
        /// Class representing a REST API workbook response.
        /// </summary>
        public class CollectionType
        {
            ///<inheritdoc/>
            [XmlAttribute("id")]
            public Guid Id { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("name")]
            public string? Name { get; set; }

            ///<inheritdoc/>
            [XmlAttribute("contentUrl")]
            public string? ContentUrl { get; set; }

            /// <summary>
            /// Gets or sets the owner for the response.
            /// </summary>
            [XmlElement("owner")]
            public OwnerType? Owner { get; set; }

            /// <summary>
            /// Class representing a REST API user response.
            /// </summary>
            public class OwnerType : IRestIdentifiable
            {
                /// <inheritdoc/>
                [XmlAttribute("id")]
                public Guid Id { get; set; }

                /// <summary>
                /// The default parameterless constructor.
                /// </summary>            
                public OwnerType()
                { }

                /// <summary>
                /// Constructor to build from <see cref="IRestIdentifiable"/>
                /// </summary>
                /// <param name="owner">The owner to copy from.</param>
                public OwnerType(IRestIdentifiable owner)
                {
                    Id = owner.Id;
                }
            }

        }
    }
}
