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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that can get a content item.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IReadApiClient<TContent>
        where TContent : class
    {
        /// <summary>
        /// Gets the content item by Id.
        /// </summary>
        /// <param name="contentId">The content item Id to get.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the get operation with the content item.</returns>
        Task<IResult<TContent>> GetByIdAsync(Guid contentId, CancellationToken cancel);
    }
}
