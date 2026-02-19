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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Paging;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Rest.Filtering;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Paging
{
    public sealed class ProjectFilteredListPagerTests
    {
        public sealed class GetPageAsync : ApiClientTestBase<IProjectsApiClient>
        {
            internal ProjectsApiClient ProjectsApiClient => GetApiClient<ProjectsApiClient>();

            [Fact]
            public async Task RequestFailsAsync()
            {
                var filters = CreateMany<Filter>();
                var (response, ex) = SetupExceptionResponse<ProjectsResponse>();

                var pager = new ProjectFilteredListPager(ProjectsApiClient, MockContentFinderFactory.Object, filters, 123);

                var result = await pager.NextPageAsync(Cancel);

                result.AssertFailure([ex]);
            }

            [Fact]
            public async Task LazyFindsParentProjectAsync()
            {
                var filters = CreateMany<Filter>();
                var parentProject = Create<IContentReference>();

                SetupSuccessResponse<ProjectsResponse>(response =>
                {
                    response.Pagination.TotalAvailable = 2;
                    response.Items = CreateMany<ProjectsResponse.ProjectType>(2).ToArray();
                    response.Items[0].ParentProjectId = null;
                    response.Items[1].ParentProjectId = parentProject.Id.ToString();
                });

                MockProjectFinder.Setup(x => x.FindByIdAsync(parentProject.Id, Cancel)).ReturnsAsync(parentProject);
                MockUserFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken c) => new ContentReferenceStub(id, Create<string>(), Create<ContentLocation>()));

                var pager = new ProjectFilteredListPager(ProjectsApiClient, MockContentFinderFactory.Object, filters, 123);

                var result = await pager.NextPageAsync(Cancel);

                result.AssertSuccess();

                Assert.Equal(1, result.PageNumber);
                Assert.Equal(123, result.PageSize);
                Assert.Equal(2, result.TotalCount);

                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.Count);

                Assert.Null(result.Value[0].ParentProject);
                Assert.Same(parentProject, result.Value[1].ParentProject);

                MockProjectFinder.Verify(x => x.FindByIdAsync(parentProject.Id, Cancel), Times.Once);
                MockProjectFinder.VerifyNoOtherCalls();

                MockUserFinder.Verify(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel), Times.Exactly(2));
                MockProjectFinder.VerifyNoOtherCalls();
            }
        }
    }
}
