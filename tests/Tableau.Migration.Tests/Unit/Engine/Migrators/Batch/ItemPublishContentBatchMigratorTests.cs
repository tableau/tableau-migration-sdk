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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Migrators.Batch;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ItemPublishContentBatchMigratorTests
    {
        public class MigratePreparedItemAsync : ParallelContentBatchMigratorBatchTestBase<TestContentType, TestPublishType>
        {
            private readonly Mock<IDestinationEndpoint> _mockDestination;
            private readonly ItemPublishContentBatchMigrator<TestContentType, TestPublishType> _migrator;

            public MigratePreparedItemAsync()
            {
                _mockDestination = Freeze<Mock<IDestinationEndpoint>>();
                _mockDestination.Setup(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => Result<TestContentType>.Succeeded(Create<TestContentType>()));

                _migrator = Create<ItemPublishContentBatchMigrator<TestContentType, TestPublishType>>();
            }

            [Fact]
            public async Task PublishesEachItemAsync()
            {
                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()), Times.Exactly(Items.Length));

                Assert.All(MockManifestEntries, e => e.Verify(x => x.DestinationFound(It.IsAny<IContentReference>()), Times.Once));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Once));
            }

            [Fact]
            public async Task PublishingFailsAsync()
            {
                var errors = new Exception[] { new(), new() };
                _mockDestination.Setup(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(() => Result<TestContentType>.Failed(errors));

                var result = await _migrator.MigrateAsync(Items, Cancel);

                result.AssertSuccess();

                _mockDestination.Verify(x => x.PublishAsync<TestPublishType, TestContentType>(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()), Times.Exactly(Items.Length));

                Assert.All(MockManifestEntries, e => e.Verify(x => x.DestinationFound(It.IsAny<IContentReference>()), Times.Never));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetMigrated(), Times.Never));
                Assert.All(MockManifestEntries, e => e.Verify(x => x.SetFailed((IEnumerable<Exception>)errors), Times.Once));
            }
        }
    }
}
