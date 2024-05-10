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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Paging;

namespace Tableau.Migration.Api
{
    internal sealed class ApiListPager<TContent> : IndexedPagerBase<TContent>
    {
        private readonly IApiPageAccessor<TContent> _listClient;

        public ApiListPager(IApiPageAccessor<TContent> listClient, int pageSize)
            : base(pageSize)
        {
            _listClient = listClient;
        }

        /// <inheritdoc />
        protected override async Task<IPagedResult<TContent>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _listClient.GetPageAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
