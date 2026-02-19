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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Search;
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Search
{
    public sealed class ApiContentReferenceStoreTests
    {
        public abstract class ApiContentReferenceStoreTest : ApiContentReferenceStoreTestBase<TestContentType>
        { }

        #region - LoadAllAsync -

        public sealed class LoadAllAsync : ApiContentReferenceStoreTest
        {
            [Fact]
            public async Task NotSupportedAsync()
            {
                MockSitesApiClient.Setup(x => x.GetListApiClient<TestContentType>())
                    .Returns(() => null!);

                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAllAsync(Cancel);

                Assert.Empty(result);
            }

            [Fact]
            public async Task LoadsAllAsync()
            {
                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAllAsync(Cancel);

                Assert.Equal(EndpointContent, result);
                MockListApiClient.Verify(x => x.GetAllAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        #endregion

        #region - LoadAsync -

        public sealed class LoadAsync : ApiContentReferenceStoreTest
        {
            [Fact]
            public async Task IdNotSupportedAsync()
            {
                MockSitesApiClient.Setup(x => x.GetReadApiClient<TestContentType>())
                    .Returns(() => null);

                var id = Guid.NewGuid();
                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAsync(id, Cancel);

                Assert.Equal(ContentReferenceLoadResult<TestContentType>.Unsupported, result);
            }

            [Fact]
            public async Task IdAsync()
            {
                var id = EndpointContent.First().Id;
                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAsync(id, Cancel);

                Assert.True(result.IsSupported);
                Assert.Equal([EndpointContent.First()], result.Items);

                MockReadApiClient.Verify(x => x.GetByIdAsync(id, Cancel), Times.Once);
            }

            [Fact]
            public async Task LocationNotSupportedAsync()
            {
                MockSitesApiClient.Setup(x => x.GetNameSearchApiClient<TestContentType>())
                    .Returns(() => null);

                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAsync(Create<ContentLocation>(), Cancel);

                Assert.Equal(ContentReferenceLoadResult<TestContentType>.Unsupported, result);
            }

            [Fact]
            public async Task LocationAsync()
            {
                var loc = EndpointContent.First().Location;
                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAsync(loc, Cancel);

                Assert.True(result.IsSupported);
                Assert.Equal([EndpointContent.First()], result.Items);

                MockNameSearchApiClient.Verify(x => x.SearchByNameAsync(loc.Name, ContentTypesOptions.BatchSize, Cancel), Times.Once);
            }

            [Fact]
            public async Task ContentUrlNotSupportedAsync()
            {
                MockSitesApiClient.Setup(x => x.GetContentUrlSearchApiClient<TestContentType>())
                    .Returns(() => null);

                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAsync(Create<string>(), Cancel);

                Assert.Equal(ContentReferenceLoadResult<TestContentType>.Unsupported, result);
            }

            [Fact]
            public async Task ContentUrlAsync()
            {
                var url = EndpointContent.First().ContentUrl;
                var store = Create<ApiContentReferenceStore<TestContentType>>();
                var result = await store.LoadAsync(url, Cancel);

                Assert.True(result.IsSupported);
                Assert.Equal([EndpointContent.First()], result.Items);

                MockContentUrlSearchApiClient.Verify(x => x.SearchByContentUrlAsync(url, Cancel), Times.Once);
            }
        }

        #endregion
    }
}
