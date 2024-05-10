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

using System.Xml.Serialization;

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Class representing a paged REST API response's pagination.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Gets or sets the response's current page number.
        /// </summary>
        [XmlAttribute("pageNumber")]
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the response's page size.
        /// </summary>
        [XmlAttribute("pageSize")]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of available items.
        /// </summary>
        [XmlAttribute("totalAvailable")]
        public int TotalAvailable { get; set; }
    }
}
