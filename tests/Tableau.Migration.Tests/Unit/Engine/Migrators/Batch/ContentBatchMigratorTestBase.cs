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

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ContentBatchMigratorTestBase<TContent, TPublish> : AutoFixtureTestBase
        where TContent : class
        where TPublish : class
    {
        protected readonly Mock<IContentItemPreparer<TContent, TPublish>> MockPreparer;
        protected readonly ImmutableArray<Mock<MigrationManifestEntry>> MockManifestEntries;
        protected readonly ImmutableArray<ContentMigrationItem<TContent>> Items;

        public ContentBatchMigratorTestBase()
        {
            MockPreparer = Freeze<Mock<IContentItemPreparer<TContent, TPublish>>>();
            MockPreparer.Setup(x => x.PrepareAsync(It.IsAny<ContentMigrationItem<TContent>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Result<TPublish>.Succeeded(Create<TPublish>()));

            var mockPipeline = Freeze<Mock<IMigrationPipeline>>();
            mockPipeline.Setup(x => x.GetItemPreparer<TContent, TPublish>())
                .Returns(MockPreparer.Object);

            MockManifestEntries = CreateMany<Mock<MigrationManifestEntry>>().ToImmutableArray();
            foreach (var manifestEntry in MockManifestEntries)
            {
                manifestEntry.Setup(e => e.Destination).CallBase();
                manifestEntry.Setup(e => e.MappedLocation).CallBase();
                manifestEntry.Setup(e => e.Source).CallBase();
                manifestEntry.Setup(e => e.Status).CallBase();
            }
            Items = MockManifestEntries.Select(me => new ContentMigrationItem<TContent>(Create<TContent>(), me.Object)).ToImmutableArray();
        }
    }
}
