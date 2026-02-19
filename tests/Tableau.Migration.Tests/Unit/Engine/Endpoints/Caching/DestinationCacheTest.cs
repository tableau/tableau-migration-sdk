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
using System.Linq;
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Caching
{
    public abstract class DestinationCacheTest<TCache, TContent> : ApiContentReferenceCacheBaseTest<TCache, TContent>
        where TCache : DestinationCache<TContent>
        where TContent : class, IContentReference
    {
        protected readonly Mock<IMigrationManifestEntryBuilder> MockManifestEntryBuilder;
        protected readonly Mock<IMigrationManifestContentTypePartitionEditor> MockManifestPartition;
        protected readonly Mock<IDestinationApiEndpoint> MockDestinationEndpoint;

        protected Dictionary<ContentLocation, Mock<MigrationManifestEntry>> MockManifestEntries = new();

        public DestinationCacheTest()
        {
            var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
            mockPipeline.Setup(x => x.CreateDestinationCacheLoadStrategy<TContent>())
                .Returns(new BulkContentReferenceCacheLoadStrategy<TContent>());

            MockManifestEntryBuilder = Freeze<Mock<IMigrationManifestEntryBuilder>>();

            SyncManifestEntries();

            MockManifestPartition = Freeze<Mock<IMigrationManifestContentTypePartitionEditor>>();
            MockManifestPartition.Setup(x => x.ByMappedLocation)
                .Returns(() =>
                    MockManifestEntries.Values
                    .Select(m => (IMigrationManifestEntryEditor)m.Object)
                    .ToDictionary(e => e.MappedLocation));

            var mockManifestEditor = Freeze<Mock<IMigrationManifestEditor>>();
            mockManifestEditor.Setup(x => x.Entries.GetOrCreatePartition<TContent>())
                .Returns(MockManifestPartition.Object);

            MockDestinationEndpoint = Freeze<Mock<IDestinationApiEndpoint>>();
            MockDestinationEndpoint.Setup(x => x.SiteApi)
                .Returns(MockSitesApiClient.Object);

            AutoFixture.Register<IDestinationEndpoint>(() => MockDestinationEndpoint.Object);
        }

        protected void SyncManifestEntries()
        {
            MockManifestEntries = EndpointContent
                // Convert all the TestContentTypes to Mock<MigrationManifestEntry> 
                // MappedLocation is used as a dictionary key, so it shouldn't be mocked
                .Select(x =>
                {
                    var mock = new Mock<MigrationManifestEntry>(MockManifestEntryBuilder.Object, new ContentReferenceStub(x));
                    mock.Setup(m => m.MappedLocation).CallBase();
                    return mock;
                })
                .ToDictionary(m => m.Object.MappedLocation);
        }
    }

    public abstract class BulkDestinationCacheTest<TContent> : DestinationCacheTest<DestinationCache<TContent>, TContent>
        where TContent : class, IContentReference
    { }
}
