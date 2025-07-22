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

using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Paging
{
    internal sealed class CallbackPager<TContent> : IndexedPagerBase<TContent>
    {
        public delegate Task<IPagedResult<TContent>> Callback(int pageNumber, int pageSize, CancellationToken cancel);

        private readonly Callback _callback;

        public CallbackPager(Callback callback, int pageSize)
            : base(pageSize)
        {
            _callback = callback;
        }

        protected override async Task<IPagedResult<TContent>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await _callback(pageNumber, pageSize, cancel).ConfigureAwait(false);
    }
}
