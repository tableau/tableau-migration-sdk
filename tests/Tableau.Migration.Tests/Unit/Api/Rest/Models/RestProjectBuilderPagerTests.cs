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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;
using Xunit;

using RestProject = Tableau.Migration.Api.Rest.Models.Responses.ProjectsResponse.ProjectType;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class RestProjectBuilderPagerTests
    {
        public class GetPageAsync : AutoFixtureTestBase
        {
            private const int PAGE_SIZE = 5;

            private List<RestProject> Projects { get; }

            private readonly Mock<IProjectsResponseApiClient> _mockApiClient;
            private readonly Mock<IContentReferenceFinder<IUser>> _mockUserFinder;

            private readonly RestProjectBuilderPager _pager;

            private readonly CancellationToken _cancel = new();

            public GetPageAsync()
            {
                Projects = CreateMany<RestProject>(12).ToList();

                _mockApiClient = Create<Mock<IProjectsResponseApiClient>>();
                _mockApiClient.Setup(x => x.GetPager(PAGE_SIZE))
                    .Returns((int pageSize) => new MemoryPager<RestProject>(Projects, pageSize));

                _mockUserFinder = Create<Mock<IContentReferenceFinder<IUser>>>();

                _pager = new(_mockApiClient.Object, _mockUserFinder.Object, PAGE_SIZE);
            }

            [Fact]
            public async Task LoadsHierarchyOnceAsync()
            {
                var page1Result = await _pager.NextPageAsync(_cancel);
                var page2Result = await _pager.NextPageAsync(_cancel);

                _mockApiClient.Verify(x => x.GetPager(It.IsAny<int>()), Times.Once);
            }

            [Fact]
            public async Task BuildsPagedProjectsAsync()
            {
                var page1Result = await _pager.NextPageAsync(_cancel);
                var page1ProjectIds = Projects.Take(PAGE_SIZE).Select(p => p.Id).ToImmutableArray();

                page1Result.AssertSuccess();
                Assert.Equal(PAGE_SIZE, page1Result.Value!.Count);
                Assert.All(page1Result.Value, p => page1ProjectIds.Contains(p.Id));

                var page2Result = await _pager.NextPageAsync(_cancel);
                var page2ProjectIds = Projects.Skip(PAGE_SIZE).Take(PAGE_SIZE).Select(p => p.Id).ToImmutableArray();

                page2Result.AssertSuccess();
                Assert.Equal(PAGE_SIZE, page2Result.Value!.Count);
                Assert.All(page2Result.Value, p => page2ProjectIds.Contains(p.Id));

                var page3Result = await _pager.NextPageAsync(_cancel);
                var page3ProjectIds = Projects.Skip(2 * PAGE_SIZE).Take(PAGE_SIZE).Select(p => p.Id).ToImmutableArray();

                page3Result.AssertSuccess();
                Assert.Equal(page3ProjectIds.Length, page3Result.Value!.Count);
                Assert.All(page3Result.Value, p => page3ProjectIds.Contains(p.Id));

                var page4Result = await _pager.NextPageAsync(_cancel);

                page4Result.AssertSuccess();
                Assert.Empty(page4Result.Value!);
            }

            [Fact]
            public async Task RawProjectListFailsAsync()
            {
                var failedResult = PagedResult<RestProject>.Failed(new Exception());

                var mockFailedPager = new Mock<IPager<RestProject>>
                {
                    CallBase = true
                };
                mockFailedPager.Setup(x => x.NextPageAsync(_cancel)).ReturnsAsync(failedResult);

                _mockApiClient.Setup(x => x.GetPager(PAGE_SIZE))
                    .Returns(mockFailedPager.Object);

                var pageResult = await _pager.NextPageAsync(_cancel);

                pageResult.AssertFailure();
                Assert.Equal(failedResult.Errors, pageResult.Errors);
            }
        }
    }
}
