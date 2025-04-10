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

using System.Xml.Serialization;
using System;

namespace Tableau.Migration.Api.Rest.Models.Responses
{
    /// <summary>
    /// The interface for the content type of a subscription.
    /// </summary>
    public interface ISubscriptionContentType
    {
        /// <summary>
        /// Gets or sets the ID for the response. 
        /// </summary>        
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type of content for the response. 
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the send view if empty flag for the response. 
        /// </summary>
        public bool SendIfViewEmpty { get; set; }
    }
}