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

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>    
    /// Class representing an error response.
    /// See <see href="https://help.tableau.com/current/api/rest_api/en-us/REST/rest_api_concepts_errors.htm">Tableau API Reference</see> 
    /// for more details.
    /// </summary>
    [XmlType]
    public class Error
    {
        /// <summary>
        /// Gets or sets the error code for the response.
        /// </summary>
        [XmlAttribute("code")]
        public string? Code { get; set; }

        /// <summary>
        /// Gets or sets the summary for the response.
        /// </summary>
        [XmlElement("summary")]
        public string? Summary { get; set; }

        /// <summary>
        /// Gets or sets a text description for the response.
        /// </summary>
        [XmlElement("detail")]
        public string? Detail { get; set; }

        /// <summary>
        /// Pretty prints the error with the <see cref="Code"/>, <see cref="Summary"/>, and <see cref="Detail"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Code} - {Summary}: {Detail}";
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Error error &&
                   Code == error.Code &&
                   Summary == error.Summary &&
                   Detail == error.Detail;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Code, Summary, Detail);
        }
    }
}
