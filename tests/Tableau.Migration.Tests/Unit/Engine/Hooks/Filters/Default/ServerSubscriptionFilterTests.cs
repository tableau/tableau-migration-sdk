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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public sealed class ServerSubscriptionFilterTests
    {
        public abstract class ServerSubscriptionFilterTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinder<IWorkbook>> MockWorkbookFinder;
            protected readonly Mock<IDestinationViewReferenceFinder> MockViewFinder;
            protected readonly MockSharedResourcesLocalizer MockLocalizer;
            protected readonly Mock<ILogger<IContentFilter<IServerSubscription>>> MockLogger;

            protected readonly ServerSubscriptionFilter Filter;

            public ServerSubscriptionFilterTest()
            {
                MockWorkbookFinder = Create<Mock<IDestinationContentReferenceFinder<IWorkbook>>>();
                MockViewFinder = Create<Mock<IDestinationViewReferenceFinder>>();
                MockLocalizer = new();
                MockLogger = Create<Mock<ILogger<IContentFilter<IServerSubscription>>>>();

                Filter = new ServerSubscriptionFilter(
                    MockWorkbookFinder.Object,
                    MockViewFinder.Object,
                    MockLocalizer.Object,
                    MockLogger.Object);
            }

            protected ContentMigrationItem<IServerSubscription> SetupSubscription(string contentType, Guid contentId)
            {
                var mockSubscription = Create<Mock<IServerSubscription>>();
                var mockContent = Create<Mock<ISubscriptionContent>>();

                mockContent.SetupGet(c => c.Type).Returns(contentType);
                mockContent.SetupGet(c => c.Id).Returns(contentId);
                mockSubscription.SetupGet(s => s.Content).Returns(mockContent.Object);

                return new ContentMigrationItem<IServerSubscription>(
                    mockSubscription.Object,
                    Create<IMigrationManifestEntryEditor>());
            }
        }

        public sealed class Ctor : ServerSubscriptionFilterTest
        {
            [Fact]
            public void Initializes()
            {
                var filter = new ServerSubscriptionFilter(
                    MockWorkbookFinder.Object,
                    MockViewFinder.Object,
                    MockLocalizer.Object,
                    MockLogger.Object);

                Assert.NotNull(filter);
            }
        }

        public sealed class ShouldMigrateAsync : ServerSubscriptionFilterTest
        {
            [Fact]
            public async Task ViewContentTypeDestinationFoundReturnsTrueAsync()
            {
                // Arrange
                var contentId = Create<Guid>();
                var subscription = SetupSubscription("view", contentId);
                var mockView = Create<Mock<IContentReference>>();

                MockViewFinder.Setup(x => x.FindBySourceIdAsync(contentId, Cancel))
                    .ReturnsAsync(Result<IContentReference>.Succeeded(mockView.Object));

                // Act
                var result = await Filter.ShouldMigrateAsync(subscription, Cancel);

                // Assert
                Assert.True(result);
                MockViewFinder.Verify(x => x.FindBySourceIdAsync(contentId, Cancel), Times.Once);
            }

            [Fact]
            public async Task ViewContentTypeDestinationNotFoundReturnsFalseAsync()
            {
                // Arrange
                var contentId = Create<Guid>();
                var subscription = SetupSubscription("view", contentId);

                MockViewFinder.Setup(x => x.FindBySourceIdAsync(contentId, Cancel))
                    .ReturnsAsync(Result<IContentReference>.Failed(new Exception()));

                // Act
                var result = await Filter.ShouldMigrateAsync(subscription, Cancel);

                // Assert
                Assert.False(result);
                MockViewFinder.Verify(x => x.FindBySourceIdAsync(contentId, Cancel), Times.Once);
            }

            [Fact]
            public async Task ViewContentTypeDestinationFoundButNotSuccessfulReturnsFalseAsync()
            {
                // Arrange
                var contentId = Create<Guid>();
                var subscription = SetupSubscription("view", contentId);
                var mockView = Create<Mock<IContentReference>>();

                MockViewFinder.Setup(x => x.FindBySourceIdAsync(contentId, Cancel))
                    .ReturnsAsync(Result<IContentReference>.Succeeded(mockView.Object));

                // Act
                var result = await Filter.ShouldMigrateAsync(subscription, Cancel);

                // Assert
                Assert.True(result); // This should be true because the result is successful
                MockViewFinder.Verify(x => x.FindBySourceIdAsync(contentId, Cancel), Times.Once);
            }

            [Fact]
            public async Task WorkbookContentTypeDestinationFoundReturnsTrueAsync()
            {
                // Arrange
                var contentId = Create<Guid>();
                var subscription = SetupSubscription("workbook", contentId);
                var mockWorkbook = Create<Mock<IContentReference>>();

                MockWorkbookFinder.Setup(x => x.FindBySourceIdAsync(contentId, Cancel))
                    .ReturnsAsync(mockWorkbook.Object);

                // Act
                var result = await Filter.ShouldMigrateAsync(subscription, Cancel);

                // Assert
                Assert.True(result);
                MockWorkbookFinder.Verify(x => x.FindBySourceIdAsync(contentId, Cancel), Times.Once);
            }

            [Fact]
            public async Task WorkbookContentTypeDestinationNotFoundReturnsFalseAsync()
            {
                // Arrange
                var contentId = Create<Guid>();
                var subscription = SetupSubscription("workbook", contentId);

                MockWorkbookFinder.Setup(x => x.FindBySourceIdAsync(contentId, Cancel))
                    .ReturnsAsync((IContentReference?)null);

                // Act
                var result = await Filter.ShouldMigrateAsync(subscription, Cancel);

                // Assert
                Assert.False(result);
                MockWorkbookFinder.Verify(x => x.FindBySourceIdAsync(contentId, Cancel), Times.Once);
            }

            [Theory]
            [InlineData("flow")]
            [InlineData("collection")]
            [InlineData("datasource")]
            [InlineData("project")]
            [InlineData("unknown")]
            public async Task UnsupportedContentTypeReturnsFalseAsync(string contentType)
            {
                // Arrange
                var contentId = Create<Guid>();
                var subscription = SetupSubscription(contentType, contentId);

                // Act
                var result = await Filter.ShouldMigrateAsync(subscription, Cancel);

                // Assert
                Assert.False(result);
                MockViewFinder.Verify(x => x.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
                MockWorkbookFinder.Verify(x => x.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }
    }
}
