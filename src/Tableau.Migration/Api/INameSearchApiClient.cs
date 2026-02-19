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
    /// Interface for an API client that can search for content items by name.
    /// </summary>
    /// <typeparam name="TContent">The content type.</typeparam>
    public interface INameSearchApiClient<TContent> : IFilteredPagedListApiClient<TContent>
    {
        /// <summary>
        /// Gets the filter operator to use for <see cref="INameSearchApiClient{TContent}.SearchByNameAsync(string, int, CancellationToken)"/>.
        /// </summary>
        FilterOperator NameFilterOperator { get; }

        /// <summary>
        /// Finds the content items with the given name.
        /// Multiple items will be returned if multiple items have the same name but different <see cref="ContentLocation"/>.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="pageSize">The expected maximum number of items to include in each page.</param>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The result of the search operation with the found items.</returns>
        public async Task<IResult<IImmutableList<TContent>>> SearchByNameAsync(string name, int pageSize, CancellationToken cancel)
        {
            var nameFilter = new Filter("name", NameFilterOperator, name);

            return await GetAllAsync([nameFilter], pageSize, cancel).ConfigureAwait(false);
        }
    }
}
