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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Config;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public abstract class BulkCacheTest<TCache, TContent> : AutoFixtureTestBase
        where TCache : BulkApiContentReferenceCache<TContent>
        where TContent : class, IContentReference
    {
        private readonly Lazy<TCache> _cache;

        protected readonly Mock<IConfigReader> MockConfigReader;
        protected readonly Mock<ISitesApiClient> MockSitesApiClient;
        protected readonly Mock<IPagedListApiClient<TContent>> MockListApiClient;

        protected TCache Cache => _cache.Value;

        protected ContentTypesOptions ContentTypesOptions { get; set; } = new ContentTypesOptions();

        protected List<TContent> EndpointContent { get; set; }

        public BulkCacheTest()
        {
            _cache = new(CreateCache);

            EndpointContent = CreateMany<TContent>().ToList();
            ContentTypesOptions.BatchSize = EndpointContent.Count / 2;

            MockListApiClient = Freeze<Mock<IPagedListApiClient<TContent>>>();
            MockListApiClient.Setup(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Result<IImmutableList<TContent>>.Succeeded(EndpointContent.ToImmutableList()));

            MockSitesApiClient = Freeze<Mock<ISitesApiClient>>();
            MockSitesApiClient.Setup(x => x.GetListApiClient<TContent>())
                .Returns(MockListApiClient.Object);

            MockConfigReader = Freeze<Mock<IConfigReader>>();
            MockConfigReader.Setup(x => x.Get<TestContentType>())
                .Returns(() => ContentTypesOptions);
        }

        protected virtual TCache CreateCache() => Create<TCache>();
    }
}
