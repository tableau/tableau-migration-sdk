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

using System;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Tests.Unit.Api.Search;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Caching
{
    public abstract class ApiContentReferenceCacheBaseTest<TCache, TContent> : ApiContentReferenceStoreTestBase<TContent>
        where TCache : ApiContentReferenceCacheBase<TContent>
        where TContent : class, IContentReference
    {
        private readonly Lazy<TCache> _cache;

        protected TCache Cache => _cache.Value;

        public ApiContentReferenceCacheBaseTest()
        {
            _cache = new(CreateCache);
        }

        protected virtual TCache CreateCache() => Create<TCache>();
    }
}
