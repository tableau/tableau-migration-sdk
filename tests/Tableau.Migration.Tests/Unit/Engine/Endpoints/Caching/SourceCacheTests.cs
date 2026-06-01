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
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Caching
{
    public sealed class SourceCacheTests
    {
        public abstract class SourceCacheTest : ApiContentReferenceCacheBaseTest<SourceCache<TestContentType>, TestContentType>
        {
            protected readonly MigrationManifestContentTypePartition ManifestPartition;
            protected readonly Mock<IContentMappingRunner> MockMappingRunner;

            protected IContentReferenceCacheLoadStrategy<TestContentType> LoadStrategy { get; set; }

            public SourceCacheTest()
            {
                LoadStrategy = new BulkContentReferenceCacheLoadStrategy<TestContentType>();
                var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
                mockPipeline.Setup(x => x.CreateSourceCacheLoadStrategy<TestContentType>())
                    .Returns(() => LoadStrategy);

                ManifestPartition = new(typeof(TestContentType));

                var mockManifest = Freeze<Mock<IMigrationManifestEditor>>();
                mockManifest.Setup(x => x.Entries.GetOrCreatePartition<TestContentType>())
                    .Returns(ManifestPartition);

                MockMappingRunner = Freeze<Mock<IContentMappingRunner>>();
                MockMappingRunner.Setup(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel))
                    .ReturnsAsync((ContentMappingContext<TestContentType> ctx, CancellationToken c) =>
                    {
                        return ctx.MapTo(Create<ContentLocation>());
                    });

                var mockSourceEndpoint = Freeze<Mock<ISourceApiEndpoint>>();
                mockSourceEndpoint.Setup(x => x.SiteApi)
                    .Returns(MockSitesApiClient.Object);

                AutoFixture.Register<ISourceEndpoint>(() => mockSourceEndpoint.Object);
            }
        }

        public sealed class ItemsLoadedAsync : SourceCacheTest
        {
            [Fact]
            public async Task ManifestEntriesAlreadyExistAsync()
            {
                var manifestEntries = ManifestPartition.GetEntryBuilder(EndpointContent.Count)
                    .CreateEntries(EndpointContent, (i, e) => e, EndpointContent.Count);

                Assert.Equal(EndpointContent.Count, ManifestPartition.Count);

                await Cache.GetAllAsync(Cancel);

                Assert.Equal(EndpointContent.Count, Cache.Count);
                Assert.Equal(EndpointContent.Count, ManifestPartition.Count);
                Assert.Equal(manifestEntries, ManifestPartition);

                MockMappingRunner.Verify(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel), Times.Never);

                Assert.All(ManifestPartition, e =>
                {
                    Assert.NotEqual(MigrationManifestEntryStatus.Skipped, e.Status);
                    Assert.Null(e.CascadeSkip);
                });
            }

            [Fact]
            public async Task NewManifestEntriesCreatedAsyncAsync()
            {
                var existingEntries = ManifestPartition.GetEntryBuilder(EndpointContent.Count - 2)
                    .CreateEntries(EndpointContent.Take(EndpointContent.Count - 2).ToImmutableArray(), (i, e) => e, EndpointContent.Count - 2);

                var itemsToAdd = EndpointContent.TakeLast(2).ToImmutableHashSet();

                await Cache.GetAllAsync(Cancel);

                Assert.Equal(EndpointContent.Count, Cache.Count);
                
                foreach(var sourceItem in EndpointContent)
                {
                    var sourceStub = new ContentReferenceStub(sourceItem);
                    var manifestEntry = Assert.Single(ManifestPartition, e => sourceStub.Equals(e.Source));

                    if(itemsToAdd.Contains(sourceItem))
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Skipped, manifestEntry.Status);
                        Assert.False(manifestEntry.CascadeSkip);

                        MockMappingRunner.Verify(x => x.ExecuteAsync(It.Is<ContentMappingContext<TestContentType>>(ctx => ctx.ContentItem == sourceItem), Cancel), Times.Once);
                    }
                    else
                    {
                        Assert.NotEqual(MigrationManifestEntryStatus.Skipped, manifestEntry.Status);
                        Assert.Null(manifestEntry.CascadeSkip);

                        MockMappingRunner.Verify(x => x.ExecuteAsync(It.Is<ContentMappingContext<TestContentType>>(ctx => ctx.ContentItem == sourceItem), Cancel), Times.Never);
                    }
                }
            }
        }

        public sealed class UpdateManifestByLocationAsync : SourceCacheTest
        {
            public UpdateManifestByLocationAsync()
            {
                LoadStrategy = new LazyContentReferenceCacheLoadStrategy<TestContentType>();
            }

            [Fact]
            public async Task UsesCachedResultWithoutManifestUpdateAsync()
            {
                var manifestEntries = ManifestPartition.GetEntryBuilder(1)
                    .CreateEntries([EndpointContent[0]], (i, e) => e, 1);

                var loc = EndpointContent[0].Location;

                var result = await Cache.UpdateManifestByLocationAsync(loc, Cancel);

                Assert.NotNull(result);
                Assert.Same(manifestEntries.Single(), result);

                Assert.Single(ManifestPartition);
                MockMappingRunner.Verify(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task UpdatesManifestAsync()
            {
                var loc = EndpointContent[0].Location;

                var result = await Cache.UpdateManifestByLocationAsync(loc, Cancel);

                Assert.NotNull(result);
                Assert.Same(Assert.Single(ManifestPartition), result);

                MockMappingRunner.Verify(x => x.ExecuteAsync(It.Is<ContentMappingContext<TestContentType>>(ctx => ctx.ContentItem == EndpointContent[0]), Cancel), Times.Once);
                MockMappingRunner.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                var result = await Cache.UpdateManifestByLocationAsync(Create<ContentLocation>(), Cancel);

                Assert.Null(result);
                Assert.Empty(ManifestPartition);
            }
        }

        public sealed class UpdateManifestByIdAsync : SourceCacheTest
        {
            public UpdateManifestByIdAsync()
            {
                LoadStrategy = new LazyContentReferenceCacheLoadStrategy<TestContentType>();
            }

            [Fact]
            public async Task UsesCachedResultWithoutManifestUpdateAsync()
            {
                var manifestEntries = ManifestPartition.GetEntryBuilder(1)
                    .CreateEntries([EndpointContent[0]], (i, e) => e, 1);

                var id = EndpointContent[0].Id;

                var result = await Cache.UpdateManifestByIdAsync(id, Cancel);

                Assert.NotNull(result);
                Assert.Same(manifestEntries.Single(), result);

                Assert.Single(ManifestPartition);
                MockMappingRunner.Verify(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task UpdatesManifestAsync()
            {
                var id = EndpointContent[0].Id;

                var result = await Cache.UpdateManifestByIdAsync(id, Cancel);

                Assert.NotNull(result);
                Assert.Same(Assert.Single(ManifestPartition), result);

                MockMappingRunner.Verify(x => x.ExecuteAsync(It.Is<ContentMappingContext<TestContentType>>(ctx => ctx.ContentItem == EndpointContent[0]), Cancel), Times.Once);
                MockMappingRunner.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                var result = await Cache.UpdateManifestByIdAsync(Create<Guid>(), Cancel);

                Assert.Null(result);
                Assert.Empty(ManifestPartition);
            }
        }

        public sealed class UpdateManifestByContentUrlAsync : SourceCacheTest
        {
            public UpdateManifestByContentUrlAsync()
            {
                LoadStrategy = new LazyContentReferenceCacheLoadStrategy<TestContentType>();
            }

            [Fact]
            public async Task UsesCachedResultWithoutManifestUpdateAsync()
            {
                var manifestEntries = ManifestPartition.GetEntryBuilder(1)
                    .CreateEntries([EndpointContent[0]], (i, e) => e, 1);

                var contentUrl = EndpointContent[0].ContentUrl;

                var result = await Cache.UpdateManifestByContentUrlAsync(contentUrl, Cancel);

                Assert.NotNull(result);
                Assert.Same(manifestEntries.Single(), result);

                Assert.Single(ManifestPartition);
                MockMappingRunner.Verify(x => x.ExecuteAsync(It.IsAny<ContentMappingContext<TestContentType>>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task UpdatesManifestAsync()
            {
                var contentUrl = EndpointContent[0].ContentUrl;

                var result = await Cache.UpdateManifestByContentUrlAsync(contentUrl, Cancel);

                Assert.NotNull(result);
                Assert.Same(Assert.Single(ManifestPartition), result);

                MockMappingRunner.Verify(x => x.ExecuteAsync(It.Is<ContentMappingContext<TestContentType>>(ctx => ctx.ContentItem == EndpointContent[0]), Cancel), Times.Once);
                MockMappingRunner.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                var result = await Cache.UpdateManifestByContentUrlAsync(Create<string>(), Cancel);

                Assert.Null(result);
                Assert.Empty(ManifestPartition);
            }
        }
    }
}
