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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api.Paging
{
    internal sealed class ApiFilteredListPager<TContent> : ApiListPager<TContent>
    {
        private readonly IApiFilteredPageAccessor<TContent> _listClient;
        private readonly IEnumerable<Filter> _filters;

        public ApiFilteredListPager(IApiFilteredPageAccessor<TContent> listClient, IEnumerable<Filter> filters, int pageSize)
            : base(listClient, pageSize)
        {
            _listClient = listClient;
            _filters = filters;
        }

        /// <inheritdoc />
        protected override async Task<IPagedResult<TContent>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _listClient.GetPageAsync(_filters, pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
