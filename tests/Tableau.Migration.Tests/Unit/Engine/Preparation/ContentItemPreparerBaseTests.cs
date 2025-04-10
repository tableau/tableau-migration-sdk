//
//  Copyright (c) 2025, Salesforce, Inc.
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
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Preparation
{
    public class ContentItemPreparerBaseTests
    {
        #region - Test Classes -

        public class TestPreparer<TContent, TPrepare, TPublish> : ContentItemPreparerBase<TContent, TPrepare, TPublish>
            where TPrepare : class
            where TPublish : class
        {
            public IResult<TPrepare> PullResult { get; set; }

            public int PullCallCount { get; private set; }

            public TestPreparer(IFixture fixture,
                IMigrationPipeline pipeline,
                IContentTransformerRunner transformerRunner,
                IDestinationContentReferenceFinderFactory destinationFinderFactory,
                ISharedResourcesLocalizer localizer)
                : base(pipeline, transformerRunner, destinationFinderFactory, localizer)
            {
                PullResult = Result<TPrepare>.Succeeded(fixture.Create<TPrepare>());
            }

            protected override Task<IResult<TPrepare>> PullAsync(ContentMigrationItem<TContent> item, CancellationToken cancel)
            {
                PullCallCount++;
                return Task.FromResult(PullResult);
            }
        }

        public class TestPreparer<TContent, TPrepare> : TestPreparer<TContent, TPrepare, TPrepare>
           where TContent : class
           where TPrepare : class
        {
            public TestPreparer(IFixture fixture,
                IMigrationPipeline pipeline,
                IContentTransformerRunner transformerRunner,
                IDestinationContentReferenceFinderFactory destinationFinderFactory,
                ISharedResourcesLocalizer localizer)
                : base(fixture, pipeline, transformerRunner, destinationFinderFactory, localizer)
            { }
        }

        public class TestPreparer<TContent> : TestPreparer<TContent, TContent, TContent>
            where TContent : class
        {
            public TestPreparer(IFixture fixture,
                IMigrationPipeline pipeline,
                IContentTransformerRunner transformerRunner,
                IDestinationContentReferenceFinderFactory destinationFinderFactory,
                ISharedResourcesLocalizer localizer)
                : base(fixture, pipeline, transformerRunner, destinationFinderFactory, localizer)
            { }
        }

        public class TestPreparer : TestPreparer<TestContentType, TestPublishType>
        {
            public TestPreparer(IFixture fixture,
                IMigrationPipeline pipeline,
                IContentTransformerRunner transformerRunner, 
                IDestinationContentReferenceFinderFactory destinationFinderFactory,
                ISharedResourcesLocalizer localizer)
                : base(fixture, pipeline, transformerRunner, destinationFinderFactory, localizer)
            { }
        }

        public class TestMappableContainerContentType : MappableContainerContentBase
        {
            protected override IContentReference? MappableContainer { get; set; }

            public IContentReference? PublicContainer
            {
                get => MappableContainer;
                set => MappableContainer = value;
            }
        }

        public class TestContainerContentType : ContainerContentBase
        {
            public TestContainerContentType(IContentReference container)
                : base(container)
            { }

            public IContentReference? PublicContainer
            {
                get => MappableContainer;
                set => MappableContainer = value;
            }
        }

        #endregion

        #region - PrepareAsync -

        public class PrepareAsync : ContentItemPreparerTestBase<TestContentType, TestPublishType>
        {
            [Fact]
            public async Task PullsAndTransformsAsync()
            {
                var preparer = Create<TestPreparer<TestContentType, TestPublishType>>();
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
                var preparer = Create<TestPreparer<TestContentType, TestPublishType>>();

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
                var item = Create<ContentMigrationItem<TestMappableContainerContentType>>();

                var preparer = Create<TestPreparer<TestMappableContainerContentType>>();
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
                publishItem.PublicContainer = Create<ContentReferenceStub>();

                var sourceParentLocation = publishItem.PublicContainer.Location;

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
                Assert.Same(destinationProject, publishItem.PublicContainer);

                MockProjectFinder.Verify(x => x.FindBySourceLocationAsync(sourceParentLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task AppliesNewParentToContainerContentAsync()
            {
                var item = Create<ContentMigrationItem<TestMappableContainerContentType>>();
                var preparer = Create<TestPreparer<TestMappableContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.PublicContainer = Create<ContentReferenceStub>();

                var destinationContainerLocation = MockManifestEntry.Object.MappedLocation.Parent();
                var destinationProject = Create<ContentReferenceStub>();

                MockProjectFinder.Setup(x => x.FindByMappedLocationAsync(destinationContainerLocation, Cancel))
                    .ReturnsAsync(destinationProject);

                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                var destinationName = MockManifestEntry.Object.MappedLocation.Name;

                Assert.Equal(MockManifestEntry.Object.MappedLocation, publishItem.Location);
                Assert.Equal(destinationName, publishItem.Name);
                Assert.Same(destinationProject, publishItem.PublicContainer);

                MockProjectFinder.Verify(x => x.FindByMappedLocationAsync(destinationContainerLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task AppliesMappingToTopLevelContainerContentAsync()
            {
                var item = Create<ContentMigrationItem<TestMappableContainerContentType>>();
                var preparer = Create<TestPreparer<TestMappableContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.PublicContainer = null;

                MappedLocation = new(Create<string>());

                var result = await preparer.PrepareAsync(item, Cancel);

                result.AssertSuccess();

                var destinationName = MockManifestEntry.Object.MappedLocation.Name;

                Assert.Equal(MockManifestEntry.Object.MappedLocation, publishItem.Location);
                Assert.Equal(destinationName, publishItem.Name);
                Assert.Null(publishItem.PublicContainer);

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

            [Fact]
            public async Task PreMappedDestinationProjectNotFoundAsync()
            {
                var item = Create<ContentMigrationItem<TestContainerContentType>>();
                var preparer = Create<TestPreparer<TestContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.PublicContainer = Create<ContentReferenceStub>();

                var destinationContainerLocation = MockManifestEntry.Object.MappedLocation.Parent();
                var destinationProject = Create<ContentReferenceStub>();

                MockProjectFinder.Setup(x => x.FindByMappedLocationAsync(destinationContainerLocation, Cancel))
                    .ReturnsAsync((IContentReference?)null);

                await Assert.ThrowsAsync<Exception>(() => preparer.PrepareAsync(item, Cancel));

                MockProjectFinder.Verify(x => x.FindByMappedLocationAsync(destinationContainerLocation, Cancel), Times.Once);
            }

            [Fact]
            public async Task MappedProjectNotFoundAsync()
            {
                MockPipeline.Setup(x => x.GetItemConverter<TestContainerContentType, TestContainerContentType>())
                    .Returns(() => new DirectContentItemConverter<TestContainerContentType, TestContainerContentType>());

                var item = Create<ContentMigrationItem<TestContainerContentType>>();
                var preparer = Create<TestPreparer<TestContainerContentType>>();

                var publishItem = preparer.PullResult.Value!;
                publishItem.PublicContainer = Create<ContentReferenceStub>();

                var sourceParentLocation = publishItem.PublicContainer.Location;

                MappedLocation = sourceParentLocation.Append(Create<string>());

                var destinationContainerLocation = MockManifestEntry.Object.MappedLocation.Parent();
                var destinationProject = Create<ContentReferenceStub>();

                MockProjectFinder.Setup(x => x.FindBySourceLocationAsync(sourceParentLocation, Cancel))
                    .ReturnsAsync((IContentReference?)null);

                await Assert.ThrowsAsync<Exception>(() => preparer.PrepareAsync(item, Cancel));

                MockProjectFinder.Verify(x => x.FindBySourceLocationAsync(sourceParentLocation, Cancel), Times.Once);
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

        public class PrepareAsyncConvert : ContentItemPreparerTestBase<TestFileContentType, TestPublishType>
        {
            [Fact]
            public async Task ConvertsToPublishTypeAsync()
            {
                var convertedItem = Create<TestPublishType>();
                var mockConverter = Create<Mock<IContentItemConverter<TestFileContentType, TestPublishType>>>();
                mockConverter.Setup(x => x.ConvertAsync(It.IsAny<TestFileContentType>(), Cancel))
                    .ReturnsAsync(convertedItem);

                MockPipeline.Setup(x => x.GetItemConverter<TestFileContentType, TestPublishType>())
                    .Returns(mockConverter.Object);

                var preparer = Create<TestPreparer<TestFileContentType, TestFileContentType, TestPublishType>>();
                var result = await preparer.PrepareAsync(Item, Cancel);

                result.AssertSuccess();
                
                MockPipeline.Verify(x => x.GetItemConverter<TestFileContentType, TestPublishType>(), Times.Once);
                mockConverter.Verify(x => x.ConvertAsync(preparer.PullResult.Value!, Cancel), Times.Once);

                Assert.Same(convertedItem, result.Value);
            }

            [Fact]
            public async Task DisposesPullValueOnConverterExceptionAsync()
            {
                var ex = new Exception();
                var mockConverter = Create<Mock<IContentItemConverter<TestFileContentType, TestPublishType>>>();
                mockConverter.Setup(x => x.ConvertAsync(It.IsAny<TestFileContentType>(), Cancel))
                    .ThrowsAsync(ex);

                MockPipeline.Setup(x => x.GetItemConverter<TestFileContentType, TestPublishType>())
                    .Returns(mockConverter.Object);

                var preparer = Create<TestPreparer<TestFileContentType, TestFileContentType, TestPublishType>>();
                
                var thrown = await Assert.ThrowsAsync<Exception>(() => preparer.PrepareAsync(Item, Cancel));
                
                Assert.Same(ex, thrown);
                Assert.True(preparer.PullResult.Value!.IsDisposed);

                MockPipeline.Verify(x => x.GetItemConverter<TestFileContentType, TestPublishType>(), Times.Once);
                mockConverter.Verify(x => x.ConvertAsync(preparer.PullResult.Value!, Cancel), Times.Once);
            }
        }

        #endregion
    }
}
