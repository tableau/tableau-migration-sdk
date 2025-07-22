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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public sealed class DestinationViewReferenceFinderTests
    {
        public abstract class DestinationViewReferenceFinderTest : AutoFixtureTestBase
        {
            protected readonly Mock<IViewsContentClient> MockSourceViewContentClient;

            protected readonly Mock<IDestinationContentReferenceFinder<IWorkbook>> MockDestinationWorkbookFinder;
            protected readonly Mock<IWorkbooksContentClient> MockDestinationWorkbookContentClient;

            protected readonly DestinationViewReferenceFinder Finder;

            protected IView SourceView { get; set; }

            protected List<IView> DestinationWorkbookViews { get; set; }

            public DestinationViewReferenceFinderTest()
            {
                SourceView = Create<IView>();

                MockSourceViewContentClient = Freeze<Mock<IViewsContentClient>>();
                MockSourceViewContentClient.Setup(x => x.GetByIdAsync(SourceView.Id, Cancel))
                    .ReturnsAsync(() => Result<IView>.Succeeded(SourceView));

                var mockSource = Freeze<Mock<ISourceEndpoint>>();
                mockSource.CallBase = true;
                mockSource.Setup(x => x.GetContentClient<IViewsContentClient, IView>()).Returns(MockSourceViewContentClient.Object);

                var destinationWorkbook = Create<IContentReference>();

                var mockDestinationViews = CreateMany<Mock<IView>>();
                mockDestinationViews.PickRandom().SetupGet(x => x.Name).Returns(SourceView.Name);
                DestinationWorkbookViews = mockDestinationViews.Select(x => x.Object).ToList();

                MockDestinationWorkbookFinder = Freeze<Mock<IDestinationContentReferenceFinder<IWorkbook>>>();
                MockDestinationWorkbookFinder.Setup(x => x.FindBySourceIdAsync(SourceView.ParentWorkbook.Id, Cancel))
                    .ReturnsAsync(destinationWorkbook);

                MockDestinationWorkbookContentClient = Freeze<Mock<IWorkbooksContentClient>>();
                MockDestinationWorkbookContentClient.Setup(x => x.GetViewsForWorkbookIdAsync(destinationWorkbook.Id, Cancel))
                    .ReturnsAsync(() => Result<IImmutableList<IView>>.Succeeded(DestinationWorkbookViews.ToImmutableArray()));

                var mockDestination = Freeze<Mock<IDestinationEndpoint>>();
                mockDestination.Setup(x => x.GetContentClient<IWorkbooksContentClient, IWorkbook>()).Returns(MockDestinationWorkbookContentClient.Object);

                Finder = Create<DestinationViewReferenceFinder>();
            }
        }

        public sealed class FindBySourceIdAsync : DestinationViewReferenceFinderTest
        {
            [Fact]
            public async Task SourceViewLookupFailsAsync()
            {
                var errorResult = Result<IView>.Failed(CreateMany<Exception>());

                MockSourceViewContentClient.Setup(x => x.GetByIdAsync(SourceView.Id, Cancel))
                    .ReturnsAsync(() => errorResult);

                var result = await Finder.FindBySourceIdAsync(SourceView.Id, Cancel);

                result.AssertFailure();
                Assert.Equal(errorResult.Errors, result.Errors);
            }

            [Fact]
            public async Task MappedWorkbookLookupFailsAsync()
            {
                MockDestinationWorkbookFinder.Setup(x => x.FindBySourceIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((IContentReference?)null);

                var result = await Finder.FindBySourceIdAsync(SourceView.Id, Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task DestinationViewListLookupFailsAsync()
            {
                var errorResult = Result<IImmutableList<IView>>.Failed(CreateMany<Exception>());

                MockDestinationWorkbookContentClient.Setup(x => x.GetViewsForWorkbookIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync(errorResult);

                var result = await Finder.FindBySourceIdAsync(SourceView.Id, Cancel);

                result.AssertFailure();
                Assert.Equal(errorResult.Errors, result.Errors);
            }

            [Fact]
            public async Task DestinationViewMatchFailsAsync()
            {
                DestinationWorkbookViews = CreateMany<IView>().ToList();

                var result = await Finder.FindBySourceIdAsync(SourceView.Id, Cancel);

                result.AssertFailure();
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var expectedView = DestinationWorkbookViews.Single(v => string.Equals(v.Name, SourceView.Name, StringComparison.Ordinal));

                var result = await Finder.FindBySourceIdAsync(SourceView.Id, Cancel);

                result.AssertSuccess();
                Assert.Same(expectedView, result.Value);
            }
        }
    }
}
