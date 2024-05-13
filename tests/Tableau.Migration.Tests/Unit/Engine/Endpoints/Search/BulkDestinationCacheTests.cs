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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class BulkDestinationCacheTests
    {
        public class TestBulkDestinationCache : BulkDestinationCache<TestContentType>
        {
            public List<TestContentType> ItemLoadedCalls { get; } = new();

            public TestBulkDestinationCache(IDestinationEndpoint endpoint, IConfigReader configReader, IMigrationManifestEditor manifest) 
                : base(endpoint, configReader, manifest)
            { }

            protected override void ItemLoaded(TestContentType item)
            {
                base.ItemLoaded(item);

                ItemLoadedCalls.Add(item);
            }
        }

        public class LoadStoreAsync : BulkDestinationCacheTest<TestBulkDestinationCache, TestContentType>
        {
            [Fact]
            public async Task PopulatesAllPagesFromEndpointAsync()
            {
                var item = EndpointContent[1];

                var result = await Cache.ForLocationAsync(item.Location, Cancel);

                var resultStub = Assert.IsType<ContentReferenceStub>(result);
                Assert.Equal(new ContentReferenceStub(item), resultStub);

                MockListApiClient.Verify(x => x.GetAllAsync(ContentTypesOptions.BatchSize, It.IsAny<CancellationToken>()), Times.Once);

                Assert.Equal(EndpointContent.Count, Cache.Count);
            }

            [Fact]
            public async Task SetsManifestDestinationInfoAsync()
            {
                var result = await Cache.ForLocationAsync(EndpointContent[1].Location, Cancel);

                foreach (var item in EndpointContent)
                {
                    var mockEntry = MockManifestEntries[item.Location];

                    mockEntry.Verify(x => x.DestinationFound(It.IsAny<IContentReference>()), Times.Once);
                }
            }

            [Fact]
            public async Task LoadsOnlyOnceAsync()
            {
                foreach (var item in EndpointContent)
                {
                    var result = await Cache.ForLocationAsync(item.Location, Cancel);

                    Assert.NotSame(item, result);

                    var resultStub = Assert.IsType<ContentReferenceStub>(result);
                    Assert.Equal(new ContentReferenceStub(item), resultStub);
                }

                MockListApiClient.Verify(x => x.GetAllAsync(ContentTypesOptions.BatchSize, It.IsAny<CancellationToken>()), Times.Once);

                Assert.Equal(EndpointContent.Count, Cache.Count);
            }

            [Fact]
            public async Task InvokesCallbackAsync()
            {
                var item = EndpointContent[1];

                var result = await Cache.ForLocationAsync(item.Location, Cancel);

                var resultStub = Assert.IsType<ContentReferenceStub>(result);
                Assert.Equal(new ContentReferenceStub(item), resultStub);

                Assert.Equal(EndpointContent.Count, Cache.ItemLoadedCalls.Count);
                Assert.All(EndpointContent, i => Cache.ItemLoadedCalls.Contains(i));
            }
        }
    }
}
