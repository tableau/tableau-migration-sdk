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

using System.Collections.Generic;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default.Cascade;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default.Cascade
{
    public sealed class BasicCascadingFilterTests
    {
        public class TestContainerContentType : TestContentType, IContainerContent
        {
            public IContentReference Container { get; set; } = null!;
        }

        public class TestMappableContainerContentType : TestContentType, IMappableContainerContent
        {
            public IContentReference Container { get; set; } = null!;

            public void SetLocation(IContentReference? container, ContentLocation newLocation)
            {
                throw new System.NotImplementedException();
            }
        }

        public class TestOwnerContentType : TestContentType, IWithOwner
        {
            public IContentReference Owner { get; set; } = null!;
        }

        public class TestWorkbookContentType : TestContentType, IWithWorkbook
        {
            public IContentReference Workbook { get; set; } = null!;
        }

        public abstract class BasicCascadingFilterTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigrationManifestEntryCollectionEditor> MockManifestEntryCollection;
            protected readonly Mock<IMigrationManifestContentTypePartitionEditor> MockManifestPartition;

            protected Dictionary<ContentLocation, bool> CascadeSkipEntries = new();

            public BasicCascadingFilterTest()
            {
                MockManifestEntryCollection = Freeze<Mock<IMigrationManifestEntryCollectionEditor>>();
                MockManifestPartition = Freeze<Mock<IMigrationManifestContentTypePartitionEditor>>();

                bool BuildCascadeEntry(ContentLocation loc, out IMigrationManifestEntryEditor entry)
                {
                    if(CascadeSkipEntries.TryGetValue(loc, out var cascade))
                    {
                        var mockEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                        mockEntry.SetupGet(x => x.CascadeSkip).Returns(cascade);

                        entry = mockEntry.Object;
                        return true;
                    }

                    entry = null!;
                    return false;
                }

                MockManifestPartition.Setup(x => x.BySourceLocation.TryGetValue(It.IsAny<ContentLocation>(), out It.Ref<IMigrationManifestEntryEditor>.IsAny!))
                    .Returns(BuildCascadeEntry);
            }

            protected void AssertContentReferenceTypeSearched<TReference>()
            {
                MockManifestEntryCollection.Verify(x => x.GetPartition(typeof(TReference)), Times.AtLeastOnce);
            }
        }

        public sealed class Filter : BasicCascadingFilterTest
        {
            [Fact]
            public void AlreadyCascading()
            {
                var f = Create<BasicCascadingFilter<TestContainerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestContainerContentType>>();
                ctx.Status = FilterStatus.CascadeSkip;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
                MockManifestPartition.VerifyNoOtherCalls();
            }

            [Fact]
            public void CascadingReferenceCascadesFilter()
            {
                var f = Create<BasicCascadingFilter<TestContainerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestContainerContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Container.Location] = true;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
            }

            [Fact]
            public void NonCascadingReferenceDoesNotCascadeFilter()
            {
                var f = Create<BasicCascadingFilter<TestContainerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestContainerContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Container.Location] = false;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.Migrate, ctx.Status);
            }

            [Fact]
            public void InvalidReferenceType()
            {
                MockManifestEntryCollection.Setup(x => x.GetPartition(typeof(IProject)))
                    .Returns((IMigrationManifestContentTypePartitionEditor?)null);

                var f = Create<BasicCascadingFilter<TestContainerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestContainerContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Container.Location] = true;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.Migrate, ctx.Status);
            }

            [Fact]
            public void ContainerContentCascades()
            {
                var f = Create<BasicCascadingFilter<TestContainerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestContainerContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Container.Location] = true;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
                AssertContentReferenceTypeSearched<IProject>();
            }

            [Fact]
            public void MappableContainerContentCascades()
            {
                var f = Create<BasicCascadingFilter<TestMappableContainerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestMappableContainerContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Container.Location] = true;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
                AssertContentReferenceTypeSearched<IProject>();
            }

            [Fact]
            public void OwnerContentCascades()
            {
                var f = Create<BasicCascadingFilter<TestOwnerContentType>>();

                var ctx = Create<ContentFilterContextItem<TestOwnerContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Owner.Location] = true;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
                AssertContentReferenceTypeSearched<IUser>();
            }

            [Fact]
            public void WorkbookContentCascades()
            {
                var f = Create<BasicCascadingFilter<TestWorkbookContentType>>();

                var ctx = Create<ContentFilterContextItem<TestWorkbookContentType>>();
                CascadeSkipEntries[ctx.SourceItem.Workbook.Location] = true;

                f.Filter(ctx);

                Assert.Equal(FilterStatus.CascadeSkip, ctx.Status);
                AssertContentReferenceTypeSearched<IWorkbook>();
            }
        }
    }
}
