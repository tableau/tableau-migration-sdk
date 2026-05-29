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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public sealed class ManifestDestinationContentReferenceFinderTests
    {
        public abstract class LocationDestinationContentFinderTest : AutoFixtureTestBase
        {
            protected readonly IMigrationManifestEditor Manifest;
            protected readonly Mock<IContentReferenceCache> MockCache;
            protected readonly Mock<IManifestUpdateSourceContentReferenceCache<TestContentType>> MockManifestUpdateCache;

            protected readonly ManifestDestinationContentReferenceFinder<TestContentType> Finder;

            public LocationDestinationContentFinderTest()
            {
                Manifest = Create<MigrationManifest>();

                MockCache = Freeze<Mock<IContentReferenceCache>>();

                var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
                mockPipeline.Setup(x => x.CreateDestinationCache<TestContentType>())
                    .Returns(MockCache.Object);

                MockManifestUpdateCache = Freeze<Mock<IManifestUpdateSourceContentReferenceCache<TestContentType>>>();

                Finder = new ManifestDestinationContentReferenceFinder<TestContentType>(Manifest, mockPipeline.Object, MockManifestUpdateCache.Object);
            }

            protected IMigrationManifestEntryEditor CreateManifestEntry(TestContentType item)
            {
                var entryBuilder = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1);
                var entry = new MigrationManifestEntry(entryBuilder, new ContentReferenceStub(item));
                entry.MapToDestination(Create<ContentLocation>());

                return entry;
            }
        }

        #region - FindBySourceLocationAsync -

        public sealed class FindBySourceLocationAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithCachedMappedLocationAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

                entry.Single().MapToDestination(mappedLoc);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(mappedLoc, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(mappedLoc, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsWithDynamicManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var newManifestEntry = CreateManifestEntry(sourceItem);

                MockManifestUpdateCache.Setup(x => x.UpdateManifestByLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(newManifestEntry);

                var cacheItem = Create<IContentReference>();
                MockCache.Setup(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(cacheItem, result);

                MockManifestUpdateCache.Verify(x => x.UpdateManifestByLocationAsync(sourceItem.Location, Cancel), Times.Once);
                MockCache.Verify(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsNullWhenLocationNotFoundAsync()
            {
                MockManifestUpdateCache.Setup(x => x.UpdateManifestByLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .ReturnsAsync((IMigrationManifestEntryEditor?)null);

                var result = await Finder.FindBySourceLocationAsync(Create<ContentLocation>(), Cancel);

                Assert.Null(result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion

        #region - FindResultBySourceLocationAsync -

        public sealed class FindResultBySourceLocationAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithCachedMappedLocationAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0).Single();

                entry.MapToDestination(mappedLoc);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(mappedLoc, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindResultBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(cacheItem, result.Destination);
                Assert.Equal(entry.Status, result.Status);

                MockCache.Verify(x => x.ForLocationAsync(mappedLoc, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsWithDynamicManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var newManifestEntry = CreateManifestEntry(sourceItem);

                MockManifestUpdateCache.Setup(x => x.UpdateManifestByLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(newManifestEntry);

                var cacheItem = Create<IContentReference>();
                MockCache.Setup(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindResultBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(cacheItem, result.Destination);
                Assert.Equal(newManifestEntry.Status, result.Status);

                MockManifestUpdateCache.Verify(x => x.UpdateManifestByLocationAsync(sourceItem.Location, Cancel), Times.Once);
                MockCache.Verify(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsEmptyWhenLocationNotFoundAsync()
            {
                MockManifestUpdateCache.Setup(x => x.UpdateManifestByLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .ReturnsAsync((IMigrationManifestEntryEditor?)null);

                var result = await Finder.FindResultBySourceLocationAsync(Create<ContentLocation>(), Cancel);

                Assert.Same(DestinationContentReferenceResult.Empty, result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion

        #region - FindBySourceIdAsync -

        public sealed class FindBySourceIdAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsByIdAsync()
            {
                var sourceItem = Create<TestContentType>();
                sourceItem.Location = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceIdAsync(sourceItem.Id, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(sourceItem.Location, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsWithDynamicManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var newManifestEntry = CreateManifestEntry(sourceItem);

                MockManifestUpdateCache.Setup(x => x.UpdateManifestByIdAsync(sourceItem.Id, Cancel))
                    .ReturnsAsync(newManifestEntry);

                var cacheItem = Create<IContentReference>();
                MockCache.Setup(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceIdAsync(sourceItem.Id, Cancel);

                Assert.Same(cacheItem, result);

                MockManifestUpdateCache.Verify(x => x.UpdateManifestByIdAsync(sourceItem.Id, Cancel), Times.Once);
                MockCache.Verify(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsNullWhenIdNotFoundAsync()
            {
                MockManifestUpdateCache.Setup(x => x.UpdateManifestByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((IMigrationManifestEntryEditor?)null);

                var result = await Finder.FindBySourceIdAsync(Create<Guid>(), Cancel);

                Assert.Null(result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion

        #region - FindResultBySourceIdAsync -

        public sealed class FindResultBySourceIdAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsByIdAsync()
            {
                var sourceItem = Create<TestContentType>();
                sourceItem.Location = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0).Single();

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindResultBySourceIdAsync(sourceItem.Id, Cancel);

                Assert.Same(cacheItem, result.Destination);
                Assert.Equal(entry.Status, result.Status);

                MockCache.Verify(x => x.ForLocationAsync(sourceItem.Location, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsWithDynamicManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var newManifestEntry = CreateManifestEntry(sourceItem);

                MockManifestUpdateCache.Setup(x => x.UpdateManifestByIdAsync(sourceItem.Id, Cancel))
                    .ReturnsAsync(newManifestEntry);

                var cacheItem = Create<IContentReference>();
                MockCache.Setup(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindResultBySourceIdAsync(sourceItem.Id, Cancel);

                Assert.Same(cacheItem, result.Destination);
                Assert.Equal(newManifestEntry.Status, result.Status);

                MockManifestUpdateCache.Verify(x => x.UpdateManifestByIdAsync(sourceItem.Id, Cancel), Times.Once);
                MockCache.Verify(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsNullWhenIdNotFoundAsync()
            {
                MockManifestUpdateCache.Setup(x => x.UpdateManifestByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((IMigrationManifestEntryEditor?)null);

                var result = await Finder.FindResultBySourceIdAsync(Create<Guid>(), Cancel);

                Assert.Same(DestinationContentReferenceResult.Empty, result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion

        #region - FindBySourceContentUrlAsync -

        public sealed class FindBySourceContentUrlAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsByContentUrlAsync()
            {
                var sourceItem = Create<TestContentType>();
                sourceItem.Location = Create<ContentLocation>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

                var cacheItem = Create<IContentReference>();

                MockCache.Setup(x => x.ForLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceContentUrlAsync(sourceItem.ContentUrl, Cancel);

                Assert.Same(cacheItem, result);

                MockCache.Verify(x => x.ForLocationAsync(sourceItem.Location, Cancel), Times.Once);
            }

            [Fact]
            public async Task FindsWithDynamicManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var newManifestEntry = CreateManifestEntry(sourceItem);

                MockManifestUpdateCache.Setup(x => x.UpdateManifestByContentUrlAsync(sourceItem.ContentUrl, Cancel))
                    .ReturnsAsync(newManifestEntry);

                var cacheItem = Create<IContentReference>();
                MockCache.Setup(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel))
                    .ReturnsAsync(cacheItem);

                var result = await Finder.FindBySourceContentUrlAsync(sourceItem.ContentUrl, Cancel);

                Assert.Same(cacheItem, result);

                MockManifestUpdateCache.Verify(x => x.UpdateManifestByContentUrlAsync(sourceItem.ContentUrl, Cancel), Times.Once);
                MockCache.Verify(x => x.ForLocationAsync(newManifestEntry.MappedLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task ReturnsNullWhenContentUrlNotFoundAsync()
            {
                MockManifestUpdateCache.Setup(x => x.UpdateManifestByContentUrlAsync(It.IsAny<string>(), Cancel))
                    .ReturnsAsync((IMigrationManifestEntryEditor?)null);

                var result = await Finder.FindBySourceContentUrlAsync(Create<string>(), Cancel);

                Assert.Null(result);

                MockCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion

        #region - FindByMappedLocationAsync -

        public sealed class FindByMappedLocationAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithMappedLocationFromManifestAsync()
            {
                var sourceItem = Create<TestContentType>();
                var mappedLoc = Create<ContentLocation>();

                var entryBuilder = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1);
                var entries = entryBuilder
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

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
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

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
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

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

        public sealed class FindByIdAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsWithManifestEntryDestinationInfoAsync()
            {
                var sourceItem = Create<TestContentType>();
                var destinationInfo = Create<IContentReference>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e, 0);

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

        #region - FindAllAsync -

        public sealed class FindAllAsync : LocationDestinationContentFinderTest
        {
            [Fact]
            public async Task FindsAllAsync()
            {
                IImmutableList<IContentReference> cacheResult = CreateMany<IContentReference>().ToImmutableArray();
                MockCache.Setup(x => x.GetAllAsync(Cancel)).ReturnsAsync(cacheResult);

                var result = await Finder.FindAllAsync(Cancel);

                Assert.Same(cacheResult, result);
                MockCache.Verify(x => x.GetAllAsync(Cancel), Times.Once);
            }
        }

        #endregion
    }
}
