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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class CleanupViewsPostPublishHookTests
    {
        public class CleanupViewsPostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration;
            protected readonly Mock<ISourceApiEndpoint> MockSourceEndpoint;
            protected readonly Mock<IDestinationApiEndpoint> MockDestinationEndpoint;
            protected readonly Mock<ISitesApiClient> MockSourceSiteApiClient;
            protected readonly Mock<ISitesApiClient> MockDestinationSiteApiClient;
            protected readonly Mock<IWorkbooksApiClient> MockSourceWorkbooksClient;
            protected readonly Mock<IWorkbooksApiClient> MockDestinationWorkbooksClient;
            protected readonly Mock<IViewsApiClient> MockDestinationViewsClient;
            protected readonly TestLoggerFactory LoggerFactory;
            protected readonly ILogger<CleanupViewsPostPublishHook> Logger;
            protected readonly MockSharedResourcesLocalizer MockLocalizer;

            protected readonly CleanupViewsPostPublishHook Hook;

            public CleanupViewsPostPublishHookTest()
            {
                MockMigration = Freeze<Mock<IMigration>>();
                MockSourceEndpoint = new Mock<ISourceApiEndpoint>();
                MockDestinationEndpoint = new Mock<IDestinationApiEndpoint>();
                MockSourceSiteApiClient = new Mock<ISitesApiClient>();
                MockDestinationSiteApiClient = new Mock<ISitesApiClient>();
                MockSourceWorkbooksClient = new Mock<IWorkbooksApiClient>();
                MockDestinationWorkbooksClient = new Mock<IWorkbooksApiClient>();
                MockDestinationViewsClient = new Mock<IViewsApiClient>();
                LoggerFactory = new TestLoggerFactory();
                Logger = LoggerFactory.CreateLogger<CleanupViewsPostPublishHook>();
                MockLocalizer = new MockSharedResourcesLocalizer();

                // Setup API client hierarchy
                MockSourceSiteApiClient.Setup(x => x.Workbooks).Returns(MockSourceWorkbooksClient.Object);
                MockDestinationSiteApiClient.Setup(x => x.Workbooks).Returns(MockDestinationWorkbooksClient.Object);
                MockDestinationSiteApiClient.Setup(x => x.Views).Returns(MockDestinationViewsClient.Object);

                MockSourceEndpoint.Setup(x => x.SiteApi).Returns(MockSourceSiteApiClient.Object);
                MockDestinationEndpoint.Setup(x => x.SiteApi).Returns(MockDestinationSiteApiClient.Object);

                // Setup migration endpoints
                MockMigration.SetupGet(m => m.Source).Returns(MockSourceEndpoint.Object);
                MockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);

                Hook = new CleanupViewsPostPublishHook(MockMigration.Object, Logger, MockLocalizer.Object);
            }

            protected ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails> CreateContext()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var publishedItem = Create<IPublishableWorkbook>();
                var destinationItem = Create<IWorkbookDetails>();

                return new ContentItemPostPublishContext<IPublishableWorkbook, IWorkbookDetails>(
                    manifestEntry, publishedItem, destinationItem);
            }

            protected IWorkbookView CreateWorkbookView(string name)
            {
                var view = new Mock<IWorkbookView>();
                view.Setup(x => x.Id).Returns(Guid.NewGuid());
                view.Setup(x => x.Name).Returns(name);
                return view.Object;
            }

            protected TestLogger GetTestLogger()
            {
                var categoryName = typeof(CleanupViewsPostPublishHook).FullName!;
                return (TestLogger)LoggerFactory.Object.CreateLogger(categoryName);
            }
        }

        public class Constructor : CleanupViewsPostPublishHookTest
        {
            [Fact]
            public void EnabledWhenBothEndpointsAvailable()
            {
                // Act & Assert
                Assert.True(Hook.IsEnabled);
            }

            [Fact]
            public void DisabledWhenSourceEndpointMissing()
            {
                // Arrange
                var mockMigration = new Mock<IMigration>();
                mockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceEndpoint>().Object); // Non-API endpoint
                mockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);

                // Act
                var hook = new CleanupViewsPostPublishHook(mockMigration.Object, Logger, MockLocalizer.Object);

                // Assert
                Assert.False(hook.IsEnabled);
            }

            [Fact]
            public void DisabledWhenDestinationEndpointMissing()
            {
                // Arrange
                var mockMigration = new Mock<IMigration>();
                mockMigration.SetupGet(m => m.Source).Returns(MockSourceEndpoint.Object);
                mockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationEndpoint>().Object); // Non-API endpoint

                // Act
                var hook = new CleanupViewsPostPublishHook(mockMigration.Object, Logger, MockLocalizer.Object);

                // Assert
                Assert.False(hook.IsEnabled);
            }
        }

        public class ExecuteAsync : CleanupViewsPostPublishHookTest
        {
            [Fact]
            public async Task ReturnsContextWhenDisabledAsync()
            {
                // Arrange
                var mockMigration = new Mock<IMigration>();
                mockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceEndpoint>().Object); // Non-API endpoint
                mockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationEndpoint>().Object); // Non-API endpoint

                var disabledHook = new CleanupViewsPostPublishHook(mockMigration.Object, Logger, MockLocalizer.Object);
                var context = CreateContext();

                // Act
                var result = await disabledHook.ExecuteAsync(context, Cancel);

                // Assert
                Assert.Same(context, result);
            }

            [Theory]
            [InlineData("View1,View2", "View1,View2,View3,View4", 2)]
            [InlineData("View1,View2", "View1,View2", 0)]
            [InlineData("View1,VIEW2", "view1,View2,View3", 1)]
            [InlineData("", "View1,View2", 2)]
            [InlineData("View1,View2", "", 0)]
            public async Task SuccessAsync(string sourceViewNames, string destinationViewNames, int expectedDeleteCount)
            {
                // Arrange
                var context = CreateContext();
                var sourceViews = string.IsNullOrEmpty(sourceViewNames)
                    ? ImmutableList<IWorkbookView>.Empty
                    : sourceViewNames.Split(',').Select(CreateWorkbookView).ToImmutableList();

                var destinationViews = string.IsNullOrEmpty(destinationViewNames)
                    ? ImmutableList<IWorkbookView>.Empty
                    : destinationViewNames.Split(',').Select(CreateWorkbookView).ToImmutableList();

                MockSourceWorkbooksClient
                    .Setup(x => x.GetWorkbookViewsAsync(context.PublishedItem.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Succeeded(sourceViews));

                MockDestinationWorkbooksClient
                    .Setup(x => x.GetWorkbookViewsAsync(context.DestinationItem.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Succeeded(destinationViews));

                MockDestinationViewsClient
                    .Setup(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Succeeded());

                // Act
                var result = await Hook.ExecuteAsync(context, Cancel);

                // Assert
                Assert.Same(context, result);

                // Verify the correct number of delete calls were made
                MockDestinationViewsClient.Verify(
                    x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(expectedDeleteCount));
            }



            [Theory]
            [InlineData("source", "Source API error")]
            [InlineData("destination", "Destination API error")]
            public async Task FailureApiErrorAsync(string failingApi, string errorMessage)
            {
                // Arrange
                var context = CreateContext();
                var exception = new Exception(errorMessage);
                var sourceViews = new[] { CreateWorkbookView("View1") }.ToImmutableList();

                if (failingApi == "source")
                {
                    MockSourceWorkbooksClient
                        .Setup(x => x.GetWorkbookViewsAsync(context.PublishedItem.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Failed(exception));
                }
                else
                {
                    MockSourceWorkbooksClient
                        .Setup(x => x.GetWorkbookViewsAsync(context.PublishedItem.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Succeeded(sourceViews));

                    MockDestinationWorkbooksClient
                        .Setup(x => x.GetWorkbookViewsAsync(context.DestinationItem.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Failed(exception));
                }

                // Act
                var result = await Hook.ExecuteAsync(context, Cancel);

                // Assert
                Assert.Same(context, result);

                // Verify manifest entry was marked as failed
                Mock.Get(context.ManifestEntry).Verify(
                    x => x.SetFailed(It.Is<IEnumerable<Exception>>(errors => errors.Contains(exception))),
                    Times.Once);

                // Verify no delete calls were made
                MockDestinationViewsClient.Verify(
                    x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Never);
            }

            [Fact]
            public async Task PartialFailureSomeDeletesFailAsync()
            {
                // Arrange
                var context = CreateContext();
                var sourceViews = new[]
                {
                    CreateWorkbookView("View1")
                }.ToImmutableList();

                var destinationViews = new[]
                {
                    CreateWorkbookView("View1"),
                    CreateWorkbookView("View2"), // This delete will fail
                    CreateWorkbookView("View3")  // This delete will succeed
                }.ToImmutableList();

                var deleteException = new Exception("Delete failed");

                MockSourceWorkbooksClient
                    .Setup(x => x.GetWorkbookViewsAsync(context.PublishedItem.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Succeeded(sourceViews));

                MockDestinationWorkbooksClient
                    .Setup(x => x.GetWorkbookViewsAsync(context.DestinationItem.Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IImmutableList<IWorkbookView>>.Succeeded(destinationViews));

                // Setup View2 delete to fail
                MockDestinationViewsClient
                    .Setup(x => x.DeleteAsync(destinationViews[1].Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Failed(deleteException));

                // Setup View3 delete to succeed
                MockDestinationViewsClient
                    .Setup(x => x.DeleteAsync(destinationViews[2].Id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result.Succeeded());

                // Act
                var result = await Hook.ExecuteAsync(context, Cancel);

                // Assert
                Assert.Same(context, result);

                // Verify manifest entry was marked as failed with the delete exception
                Mock.Get(context.ManifestEntry).Verify(
                    x => x.SetFailed(It.Is<IEnumerable<Exception>>(errors => errors.Contains(deleteException))),
                    Times.Once);

                // Verify both deletes were attempted
                MockDestinationViewsClient.Verify(
                    x => x.DeleteAsync(destinationViews[1].Id, It.IsAny<CancellationToken>()),
                    Times.Once);
                MockDestinationViewsClient.Verify(
                    x => x.DeleteAsync(destinationViews[2].Id, It.IsAny<CancellationToken>()),
                    Times.Once);
            }

            [Fact]
            public async Task ExceptionHandlingAsync()
            {
                // Arrange
                var context = CreateContext();
                var exception = new InvalidOperationException("Unexpected error");

                MockSourceWorkbooksClient
                    .Setup(x => x.GetWorkbookViewsAsync(context.PublishedItem.Id, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

                // Act
                var result = await Hook.ExecuteAsync(context, Cancel);

                // Assert
                Assert.Same(context, result);

                // Verify manifest entry was marked as failed
                Mock.Get(context.ManifestEntry).Verify(
                    x => x.SetFailed((IEnumerable<Exception>)new[] { exception }),
                    Times.Once);
            }
        }
    }
}