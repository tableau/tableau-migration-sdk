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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class ManifestDestinationContentReferenceFinderTests
    {
        public class LocationDestinationContentFinderTest : AutoFixtureTestBase
        {
            protected readonly IMigrationManifestEditor Manifest;
            protected readonly Mock<IContentReferenceCache> MockCache;

            protected readonly ManifestDestinationContentReferenceFinder<TestContentType> Finder;

            public LocationDestinationContentFinderTest()
            {
                Manifest = Create<MigrationManifest>();

                MockCache = Freeze<Mock<IContentReferenceCache>>();

                var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
                mockPipeline.Setup(x => x.CreateDestinationCache<TestContentType>())
                    .Returns(MockCache.Object);

                Finder = new ManifestDestinationContentReferenceFinder<TestContentType>(Manifest, mockPipeline.Object);
            }
        }

        #region - FindDestinationReferenceAsync -

        public class FindDestinationReferenceAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithCachedMappedLocationAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                entry.Single().MapToDestination(mappedLoc);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(mappedLoc, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(mappedLoc, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsById()
            {
                var sourceItem = Create<TestContentType>();
                sourceItem.Location = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceIdAsync(sourceItem.Id, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(sourceItem.Location, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsNullWhenLocationNotFound()
            {
                var result = await Finder.FindBySourceLocationAsync(Create<ContentLocation>(), Cancel);

                Assert.Null(result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task ReturnsNullWhenIdNotFound()
            {
                var result = await Finder.FindBySourceIdAsync(Create<Guid>(), Cancel);

                Assert.Null(result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task FindsByContentUrl()
            {
                var sourceItem = Create<TestContentType>();
                sourceItem.Location = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceContentUrlAsync(sourceItem.ContentUrl, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(sourceItem.Location, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsNullWhenContentUrlNotFound()
            {
                var result = await Finder.FindBySourceContentUrlAsync(Create<string>(), Cancel);

                Assert.Null(result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion

        #region - FindMappedDestinationReferenceAsync -

        public class FindMappedDestinationReferenceAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithMappedLocationFromManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entryBuilder = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1);
                var entries = entryBuilder
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                var mockMapping = Freeze<Mock<IContentMappingRunner>>();
                mockMapping.Setup(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel))
                    .ReturnsAsync((ContentMappingContext<TestContentType> ctx, CancellationToken cancel)
                        => ctx.MapTo(mappedLoc));

                var mockDestinationRef = Create<Mock<IContentReference>>();
                mockDestinationRef.SetupGet(x => x.Location).Returns(mappedLoc);

                await entryBuilder.MapEntriesAsync(new[] { sourceItem }, mockMapping.Object, Cancel);
                entries.Single().DestinationFound(mockDestinationRef.Object);

                var result = await Finder.FindByMappedLocationAsync(mappedLoc, Cancel);

                Assert.Same(mockDestinationRef.Object, result);

                MockCache.Verify(x => x.ForLocationAsync(mappedLoc, Cancel), Times.Never);
            }

            [Fact]
            public async Task FindsWithMappedLocationNoDestinationAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entryBuilder = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1);
                var entries = entryBuilder
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                var mockMapping = Freeze<Mock<IContentMappingRunner>>();
                mockMapping.Setup(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel))
                    .ReturnsAsync((ContentMappingContext<TestContentType> ctx, CancellationToken cancel)
                        => ctx.MapTo(mappedLoc));

                await entryBuilder.MapEntriesAsync(new[] { sourceItem }, mockMapping.Object, Cancel);

                var cacheItem = Create<IContentReference>();
                MockCache.Setup(x => x.ForLocationAsync(mappedLoc, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindByMappedLocationAsync(mappedLoc, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(mappedLoc, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsWithCachedDestinationLocationAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entries = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                var entry = entries.Single().MapToDestination(mappedLoc);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(mappedLoc, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindByMappedLocationAsync(entry.MappedLocation, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(mappedLoc, Cancel), Times.Once);
            }
        }

        #endregion

        #region - FindByIdAsync -

        public class FindByIdAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithManifestEntryDestinationInfoAsync()
            {
                var sourceItem = Create<TestContentType>();
                var destinationInfo = Create<IContentReference>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e);

                entry.Single().DestinationFound(destinationInfo);

                var result = await Finder.FindByIdAsync(destinationInfo.Id, Cancel);

                Assert.Same(destinationInfo, result);

                MockCache.Verify(x => x.ForIdAsync(It.IsAny<Guid>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task FallsBackToCacheAsync()
            {
                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForIdAsync(cacheItem.Id, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindByIdAsync(cacheItem.Id, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForIdAsync(cacheItem.Id, Cancel), Times.Once);
            }
        }

        #endregion
    }
}
