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
    /// Class representing an XML element for the location of a content item that has a name.
    /// </summary>
    public interface ILocationType : IRestIdentifiable, INamedContent
    {
        /// <summary>
        /// Gets or sets the type for the response.
        /// </summary>
        [XmlAttribute("type")]
        public string? Type { get; set; }
    }
}