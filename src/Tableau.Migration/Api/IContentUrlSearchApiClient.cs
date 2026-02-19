//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net.Rest.Filtering;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for an API client that can search for content items by content URL.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface IContentUrlSearchApiClient<TContent> : IFilteredPagedListApiClient<TContent>
    {
        /// <summary>
        /// Finds the content items with the given content URL.
        /// </summary>
        /// <param name="contentUrl">The content URL to search for.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the search operation with the found item, or null.</returns>
        public async Task<IResult<IImmutableList<TContent>>> SearchByContentUrlAsync(string contentUrl, CancellationToken cancel)
        {
            var contentUrlFilter = new Filter("contentUrl", FilterOperator.Equal, contentUrl);

            // Content URLs are site-unique, so page size of one works for all content types.
            return await GetAllAsync([contentUrlFilter], 1, cancel).ConfigureAwait(false);
        }
    }
}
