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
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class ManifestSourceContentReferenceFinderTests
    {
        public class ManifestSourceContentReferenceFinderTest : AutoFixtureTestBase
        {
            protected readonly IMigrationManifestEditor Manifest;
            protected readonly Mock<IMigrationPipeline> Pipeline;
            protected readonly Mock<IContentReferenceCache> ContentReferenceCache;

            protected readonly ManifestSourceContentReferenceFinder<TestContentType> Finder;

            public ManifestSourceContentReferenceFinderTest()
            {
                Manifest = Create<MigrationManifest>();
                Pipeline = Create<Mock<IMigrationPipeline>>();
                ContentReferenceCache = Create<Mock<IContentReferenceCache>>();

                ContentReferenceCache.Setup(x => x.ForIdAsync(It.IsAny<Guid>(), Cancel))
                    .Returns(Task.FromResult<IContentReference?>(null));

                ContentReferenceCache.Setup(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), Cancel))
                    .Returns(Task.FromResult<IContentReference?>(null));

                Pipeline.Setup(x => x.CreateSourceCache<TestContentType>())
                    .Returns(ContentReferenceCache.Object);

                Finder = new ManifestSourceContentReferenceFinder<TestContentType>(Manifest, Pipeline.Object);
            }
        }

        public class FindByIdAsync : ManifestSourceContentReferenceFinderTest
        {
            [Fact]
            public async Task FindsManifestReferenceAsync()
            {
                var sourceItem = Create<TestContentType>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e)
                    .Single();

                var result = await Finder.FindByIdAsync(sourceItem.Id, Cancel);

                Assert.Same(entry.Source, result);
                ContentReferenceCache.Verify(x => x.ForIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task FindCacheReferenceAsync()
            {
                var sourceItem = Create<TestContentType>();

                ContentReferenceCache.Setup(x => x.ForIdAsync(sourceItem.Id, Cancel))
                    .ReturnsAsync(sourceItem);

                var result = await Finder.FindByIdAsync(sourceItem.Id, Cancel);

                Assert.Same(sourceItem, result);
                ContentReferenceCache.Verify(x => x.ForIdAsync(It.IsAny<Guid>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                var sourceItem = Create<TestContentType>();

                var result = await Finder.FindByIdAsync(sourceItem.Id, Cancel);

                Assert.Null(result);
                ContentReferenceCache.Verify(x => x.ForIdAsync(It.IsAny<Guid>(), Cancel), Times.Once);
            }
        }

        public class FindBySourceLocationAsync : ManifestSourceContentReferenceFinderTest
        {
            [Fact]
            public async Task FindsManifestReferenceAsync()
            {
                var sourceItem = Create<TestContentType>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e)
                    .Single();

                var result = await Finder.FindBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(entry.Source, result);
                ContentReferenceCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task FindCacheReferenceAsync()
            {
                var sourceItem = Create<TestContentType>();

                ContentReferenceCache.Setup(x => x.ForLocationAsync(sourceItem.Location, Cancel))
                    .ReturnsAsync(sourceItem);

                var result = await Finder.FindBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Same(sourceItem, result);
                ContentReferenceCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), Cancel), Times.Once);
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                var sourceItem = Create<TestContentType>();

                var result = await Finder.FindBySourceLocationAsync(sourceItem.Location, Cancel);

                Assert.Null(result);
                ContentReferenceCache.Verify(x => x.ForLocationAsync(It.IsAny<ContentLocation>(), Cancel), Times.Once);
            }
        }
    }
}
