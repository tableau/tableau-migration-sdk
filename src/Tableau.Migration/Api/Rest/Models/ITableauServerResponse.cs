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

namespace Tableau.Migration.Api.Rest.Models
{
    /// <summary>
    /// Interface representing REST API responses.
    /// </summary>
    public interface ITableauServerResponse
    {
        /// <summary>
        /// Gets or sets the error for the response.
        /// </summary>
        Error? Error { get; set; }
    }

    /// <summary>
    /// Interface representing REST API responses with a single item.
    /// </summary>
    /// <typeparam name="TItem">The response's item type.</typeparam>
    public interface ITableauServerResponse<TItem> : ITableauServerResponse
    {
        /// <summary>
        /// Gets the item for the response.
        /// </summary>
        TItem? Item { get; set; }
    }
}
