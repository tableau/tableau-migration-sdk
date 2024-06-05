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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class ContentItemPreparerBaseTests
    {
        #region - Test Classes -

        public class TestPreparer<TContent, TPublish> : ContentItemPreparerBase<TContent, TPublish>
            where TPublish : class, new()
        {
            public IResult<TPublish> PullResult { get; set; } = Result<TPublish>.Succeeded(new());

            public int PullCallCount { get; private set; }

            public TestPreparer(
                IContentTransformerRunner transformerRunner,
                IDestinationContentReferenceFinderFactory destinationFinderFactory)
                : base(transformerRunner, destinationFinderFactory)
            { }

            protected override Task<IResult<TPublish>> PullAsync(ContentMigrationItem<TContent> item, CancellationToken cancel)
            {
                PullCallCount++;
                return Task.FromResult(PullResult);
            }
        }

        public class TestPreparer<TContent> : TestPreparer<TContent, TContent>
            where TContent : class, new()
        {
            public TestPreparer(
                IContentTransformerRunner transformerRunner,
                IDestinationContentReferenceFinderFactory destinationFinderFactory)
                : base(transformerRunner, destinationFinderFactory)
            { }
        }

        public class TestPreparer : TestPreparer<TestContentType, TestPublishType>
        {
            public TestPreparer(
                IContentTransformerRunner transformerRunner, 
                IDestinationContentReferenceFinderFactory destinationFinderFactory)
                : base(transformerRunner, destinationFinderFactory)
            { }
        }

        public class TestMappableContentType : TestContentType, IMappableContent
        {
            public void SetLocation(ContentLocation newLocation)
            {
                Location = newLocation;
            }
        }

        public class TestMappableContainerContentType : TestContentType, IMappableContainerContent
        {
            public IContentReference? Container { get; set; }

            public void SetLocation(IContentReference? container, ContentLocation newLocation)
            {
                Container = container;
                Location = newLocation;
            }
        }

        #endregion

        #region - PrepareAsync -

        public class PrepareAsync : ContentItemPreparerTestBase<TestPublishType>
        {
            [Fact]
            public async Task PullsAndTransformsAsync()
            {
                var preparer = Create<TestPreparer>();
                var result = await preparer.PrepareAsync(Item, Cancel);

                result.AssertSuccess();
                Assert.Same(preparer.PullResult.Value, result.Value);
                Assert.Equal(1, preparer.PullCallCount);

                MockTransformerRunner.Verify(x => x.ExecuteAsync(preparer.PullResult.Value, Cancel), Times.Once);

                MockManifestEntry.Verify(x => x.SetFailed(preparer.PullResult.Errors), Times.Never);
            }

            [Fact]
            public async Task PullsFailsAsync()
            {
                var preparer = Create<TestPreparer>();

                var errors = new Exception[] { new(), new() };
                preparer.PullResult = Result<TestPublishType>.Failed(errors);

                var result = await preparer.PrepareAsync(Item, Cancel);

                result.AssertFailure();
                Assert.Equal(errors, result.Errors);
                Assert.Null(result.Value);
                Assert.Equal(1, preparer.PullCallCount);

                MockTransformerRunner.Verify(x => x.ExecuteAsync(It.IsAny<TestPublishType>(), It.IsAny<CancellationToken>()), Times.Never);

                MockManifestEntry.Verify(x => x.SetFailed(preparer.PullResult.Errors), Times.Once);
            }

            [Fact]
            public async Task AppliesMappingToContentAsync()
            {
                var item = Create<ContentMigrationItem<TestMappableContentType>>();

                var preparer = Create<TestPreparer<TestMappableContentType>>();
                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                Assert.NotNull(preparer.PullResult.Value);
                Assert.Equal(MockManifestEntry.Object.MappedLocation, preparer.PullResult.Value.Location);
            }

            [Fact]
            public async Task AppliesMappingToContainerContentAsync()
            {
                var item = Create<ContentMigrationItem<TestMappableContainerContentType>>();
                var preparer = Create<TestPreparer<TestMappableContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.Container = Create<ContentReferenceStub>();

                var sourceParentLocation = publishItem.Container.Location;

                MappedLocation = sourceParentLocation.Append(Create<string>());

                var destinationContainerLocation = MockManifestEntry.Object.MappedLocation.Parent();
                var destinationProject = Create<ContentReferenceStub>();

                MockProjectFinder.Setup(x => x.FindBySourceLocationAsync(sourceParentLocation, Cancel))
                    .ReturnsAsync(destinationProject);

                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                var destinationName = MockManifestEntry.Object.MappedLocation.Name;

                Assert.Equal(MockManifestEntry.Object.MappedLocation, publishItem.Location);
                Assert.Equal(destinationName, publishItem.Name);
                Assert.Same(destinationProject, publishItem.Container);

                MockProjectFinder.Verify(x => x.FindBySourceLocationAsync(sourceParentLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task AppliesNewParentToContainerContentAsync()
            {
                var item = Create<ContentMigrationItem<TestMappableContainerContentType>>();
                var preparer = Create<TestPreparer<TestMappableContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.Container = Create<ContentReferenceStub>();

                var destinationContainerLocation = MockManifestEntry.Object.MappedLocation.Parent();
                var destinationProject = Create<ContentReferenceStub>();

                MockProjectFinder.Setup(x => x.FindByMappedLocationAsync(destinationContainerLocation, Cancel))
                    .ReturnsAsync(destinationProject);

                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                var destinationName = MockManifestEntry.Object.MappedLocation.Name;

                Assert.Equal(MockManifestEntry.Object.MappedLocation, publishItem.Location);
                Assert.Equal(destinationName, publishItem.Name);
                Assert.Same(destinationProject, publishItem.Container);

                MockProjectFinder.Verify(x => x.FindByMappedLocationAsync(destinationContainerLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task AppliesMappingToTopLevelContainerContentAsync()
            {
                var item = Create<ContentMigrationItem<TestMappableContainerContentType>>();
                var preparer = Create<TestPreparer<TestMappableContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.Container = null;

                MappedLocation = new(Create<string>());

                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                var destinationName = MockManifestEntry.Object.MappedLocation.Name;

                Assert.Equal(MockManifestEntry.Object.MappedLocation, publishItem.Location);
                Assert.Equal(destinationName, publishItem.Name);
                Assert.Null(publishItem.Container);

                MockProjectFinder.Verify(x => x.FindBySourceLocationAsync(It.IsAny<ContentLocation>(), Cancel), Times.Never);
            }

            [Fact]
            public async Task ReturnsTransformedObjectAsync()
            {
                var transformedResult = new TestPublishType();
                MockTransformerRunner.Setup(x => x.ExecuteAsync(It.IsAny<TestPublishType>(), Cancel))
                    .ReturnsAsync(transformedResult);

                var preparer = Create<TestPreparer>();
                var result = await preparer.PrepareAsync(Item, Cancel);

                result.AssertSuccess();
                Assert.Same(transformedResult, result.Value);
                Assert.Equal(1, preparer.PullCallCount);

                MockTransformerRunner.Verify(x => x.ExecuteAsync(preparer.PullResult.Value, Cancel), Times.Once);

                MockManifestEntry.Verify(x => x.SetFailed(preparer.PullResult.Errors), Times.Never);
            }
        }

        public class PrepareAsyncFile : ContentItemPreparerTestBase<TestFileContentType>
        {
            [Fact]
            public async Task FinalizesFileContentAsync()
            {
                var item = Create<ContentMigrationItem<TestFileContentType>>();
                var preparer = Create<TestPreparer<TestFileContentType>>();

                var publishItem = preparer.PullResult.Value!;

                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                MockFileStore.Verify(x => x.CloseTableauFileEditorAsync(publishItem.File, Cancel), Times.Once);
            }

            [Fact]
            public async Task FinalizesAndDisposesOnExceptionAsync()
            {
                var item = Create<ContentMigrationItem<TestFileContentType>>();
                var preparer = Create<TestPreparer<TestFileContentType>>();

                var publishItem = preparer.PullResult.Value!;

                var ex = new Exception();
                MockTransformerRunner.Setup(x => x.ExecuteAsync(publishItem, Cancel))
                    .ThrowsAsync(ex);

                var thrown = await Assert.ThrowsAsync<Exception>(() => preparer.PrepareAsync(item, Cancel));

                Assert.Same(ex, thrown);

                MockFileStore.Verify(x => x.CloseTableauFileEditorAsync(publishItem.File, Cancel), Times.Once);
                Assert.True(publishItem.IsDisposed);
            }

            [Fact]
            public async Task DisposesOnFinalizeExceptionAsync()
            {
                var item = Create<ContentMigrationItem<TestFileContentType>>();
                var preparer = Create<TestPreparer<TestFileContentType>>();

                var publishItem = preparer.PullResult.Value!;

                var ex = new Exception();
                MockFileStore.Setup(x => x.CloseTableauFileEditorAsync(publishItem.File, Cancel))
                    .ThrowsAsync(ex);

                var thrown = await Assert.ThrowsAsync<Exception>(() => preparer.PrepareAsync(item, Cancel));

                Assert.Same(ex, thrown);

                MockFileStore.Verify(x => x.CloseTableauFileEditorAsync(publishItem.File, Cancel), Times.Once);
                Assert.True(publishItem.IsDisposed);
            }

            [Fact]
            public async Task DisposesOnCancellationExceptionAsync()
            {
                var item = Create<ContentMigrationItem<TestFileContentType>>();
                var preparer = Create<TestPreparer<TestFileContentType>>();

                var publishItem = preparer.PullResult.Value!;

                var transformEx = new OperationCanceledException();
                MockTransformerRunner.Setup(x => x.ExecuteAsync(publishItem, Cancel))
                    .ThrowsAsync(transformEx);

                var closeEx = new OperationCanceledException();
                MockFileStore.Setup(x => x.CloseTableauFileEditorAsync(publishItem.File, Cancel))
                    .ThrowsAsync(closeEx);

                var thrown = await Assert.ThrowsAsync<AggregateException>(() => preparer.PrepareAsync(item, Cancel));

                Assert.True(thrown.IsCancellationException());
                Assert.Contains(transformEx, thrown.InnerExceptions);
                Assert.Contains(closeEx, thrown.InnerExceptions);

                MockFileStore.Verify(x => x.CloseTableauFileEditorAsync(publishItem.File, Cancel), Times.Once);
                Assert.True(publishItem.IsDisposed);
            }
        }

        #endregion
    }
}
