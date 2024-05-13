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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Tableau.Migration.Paging;
using Tableau.Migration.Tests.Unit.Api.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ProjectsApiClientTests
    {
        public class ProjectsApiClientTest : PermissionsApiClientTestBase<IProjectsApiClient>
        {
            internal readonly Mock<IDefaultPermissionsApiClient> MockDefaultPermissionsClient = new();

            internal ProjectsApiClient ProjectsApiClient => GetApiClient<ProjectsApiClient>();

            public ProjectsApiClientTest()
            {
                MockPermissionsClientFactory
                    .Setup(f => f.CreateDefaultPermissionsClient())
                    .Returns(MockDefaultPermissionsClient.Object);
            }
        }

        #region - CreateProjectAsync -

        public class CreateProjectAsync : ProjectsApiClientTest
        {
            private ICreateProjectOptions CreateOptions(
                Action<Mock<ICreateProjectOptions>>? configure = null)
            {
                var mockOptions = Create<Mock<ICreateProjectOptions>>();

                mockOptions.SetupGet(o => o.PublishSamples).Returns(false);

                configure?.Invoke(mockOptions);

                return mockOptions.Object;
            }

            [Fact]
            public async Task Returns_success()
            {
                var options = CreateOptions();

                var response = AutoFixture.CreateResponse<CreateProjectResponse>();
                response.Item!.ParentProjectId = options.ParentProject?.Id.ToString();

                var mockResponse = new MockHttpResponseMessage<CreateProjectResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ProjectsApiClient.CreateProjectAsync(options, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects");

                var requestContent = Assert.IsType<StringContent>(request.Content);

                var requestModel = await HttpContentSerializer.Instance.DeserializeAsync<CreateProjectRequest>(requestContent, Cancel);

                Assert.NotNull(requestModel);
                Assert.NotNull(requestModel.Project);
                Assert.Equal(options.ParentProject?.Id.ToString(), requestModel.Project.ParentProjectId);
                Assert.Equal(options.Name, requestModel.Project.Name);
                Assert.Equal(options.Description, requestModel.Project.Description);
                Assert.Equal(options.ContentPermissions, requestModel.Project.ContentPermissions);

                MockUserFinder.Verify(x => x.FindByIdAsync(response.Item.Owner!.Id, Cancel), Times.Once);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var options = CreateOptions();

                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<CreateProjectResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ProjectsApiClient.CreateProjectAsync(options, Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);

                Assert.Same(exception, error);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects");
            }

            [Fact]
            public async Task Publishes_samples()
            {
                var options = CreateOptions(m => m.SetupGet(o => o.PublishSamples).Returns(true));

                var response = AutoFixture.CreateResponse<CreateProjectResponse>();
                response.Item!.ParentProjectId = options.ParentProject?.Id.ToString();

                var mockResponse = new MockHttpResponseMessage<CreateProjectResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ProjectsApiClient.CreateProjectAsync(options, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects");

                request.AssertQuery("publishSamples", "true");
            }
        }

        #endregion

        #region - PublishAsync -

        public class PublishAsync : ProjectsApiClientTest
        {
            private IProject CreateProject(Action<Mock<IProject>>? configure = null)
            {
                var mockProject = Create<Mock<IProject>>();

                configure?.Invoke(mockProject);

                return mockProject.Object;
            }

            [Fact]
            public async Task Returns_success()
            {
                var project = CreateProject();

                var createProjectResponse = AutoFixture.CreateResponse<CreateProjectResponse>();
                createProjectResponse.Item!.Id = project.Id;
                createProjectResponse.Item!.ParentProjectId = project.ParentProject?.Id.ToString();

                var mockCreateProjectResponse = new MockHttpResponseMessage<CreateProjectResponse>(createProjectResponse);

                MockHttpClient.SetupResponse(mockCreateProjectResponse);

                var result = await ProjectsApiClient.PublishAsync(project, Cancel);

                Assert.True(result.Success);

                await AssertCreateProjectRequestAsync(MockHttpClient.SentRequests[0], project);

                MockUserFinder.Verify(x => x.FindByIdAsync(createProjectResponse.Item.Owner!.Id, Cancel), Times.Once);
            }

            [Fact]
            public async Task Succeeds_when_project_exists()
            {
                var existingProject = CreateProject();

                var createProjectResponse = new CreateProjectResponse
                {
                    Error = new Error
                    {
                        Code = ProjectsApiClient.PROJECT_NAME_CONFLICT_ERROR_CODE
                    }
                };

                var mockCreateProjectResponse = new MockHttpResponseMessage<CreateProjectResponse>(HttpStatusCode.Conflict, createProjectResponse);

                MockHttpClient.SetupResponse(mockCreateProjectResponse);

                var owner = Create<IContentReference>();
                MockUserFinder.Setup(x => x.FindByIdAsync(owner.Id, Cancel))
                    .ReturnsAsync(owner);

                var getProjectResponse = AutoFixture.CreateResponse<ProjectsResponse>();
                getProjectResponse.Items = new[]
                {
                    new ProjectsResponse.ProjectType
                    {
                        Id = existingProject.Id,
                        ParentProjectId = existingProject.ParentProject?.Id.ToString(),
                        Name = existingProject.Name,
                        Description = existingProject.Description,
                        ContentPermissions = existingProject.ContentPermissions,
                        Owner = new() { Id = owner.Id }
                    }
                };

                var mockGetProjectResponse = new MockHttpResponseMessage<ProjectsResponse>(getProjectResponse);

                MockHttpClient.SetupResponse(mockGetProjectResponse);

                var result = await ProjectsApiClient.PublishAsync(existingProject, Cancel);

                Assert.True(result.Success);

                await AssertCreateProjectRequestAsync(MockHttpClient.SentRequests[0], existingProject);
                AssertGetProjectRequest(MockHttpClient.SentRequests[1], existingProject.Name, existingProject.ParentProject?.Id);

                MockUserFinder.Verify(x => x.FindByIdAsync(getProjectResponse.Items[0].Owner!.Id, Cancel), Times.Once);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var project = CreateProject();

                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<CreateProjectResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var result = await ProjectsApiClient.PublishAsync(project, Cancel);

                Assert.False(result.Success);

                var error = Assert.Single(result.Errors);

                Assert.Same(exception, error);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects");
            }

            private async Task AssertCreateProjectRequestAsync(HttpRequestMessage r, IProject project)
            {
                r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects");

                r.AssertHttpMethod(HttpMethod.Post);

                var content = Assert.IsType<StringContent>(r.Content);

                var model = await HttpContentSerializer.Instance.DeserializeAsync<CreateProjectRequest>(content, Cancel);

                Assert.NotNull(project);

                Assert.NotNull(model);
                Assert.NotNull(model.Project);

                Assert.Equal(project.ParentProject?.Id.ToString(), model.Project.ParentProjectId);
                Assert.Equal(project.Name, model.Project.Name);
                Assert.Equal(project.Description, model.Project.Description);
                Assert.Equal(project.ContentPermissions, model.Project.ContentPermissions);
            }

            private void AssertGetProjectRequest(HttpRequestMessage r, string name, Guid? parentProjectId)
            {
                r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects");

                var filter = $"name:eq:{name}";

                if (parentProjectId is not null)
                    filter = $"{filter},parentProjectId:eq:{parentProjectId}";
                else
                    filter = $"{filter},topLevelProject:eq:true";

                r.AssertQuery("filter", filter);

                r.AssertHttpMethod(HttpMethod.Get);
            }
        }

        #endregion

        #region - GetPager -

        public class GetPager : ProjectsApiClientTest
        {
            [Fact]
            public void UsesBreadthFirstPager()
            {
                // Act
                var pager = ProjectsApiClient.GetPager(123);

                // Assert
                Assert.IsType<BreadthFirstPathHierarchyPager<IProject>>(pager);
            }
        }

        #endregion

        #region - UpdateProjectAsync -

        public class UpdateProjectAsync : ProjectsApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<UpdateProjectResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);
                MockHttpClient.SetupResponse(mockResponse);

                var projectId = Guid.NewGuid();

                var result = await ApiClient.UpdateProjectAsync(projectId, Cancel);

                result.AssertFailure();

                var resultError = Assert.Single(result.Errors);
                Assert.Same(exception, resultError);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects/{projectId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                var mockResponse = new MockHttpResponseMessage<UpdateProjectResponse>(HttpStatusCode.NotFound, null);
                MockHttpClient.SetupResponse(mockResponse);

                var projectId = Guid.NewGuid();

                var result = await ApiClient.UpdateProjectAsync(projectId, Cancel);

                result.AssertFailure();

                Assert.Null(result.Value);
                Assert.Single(result.Errors);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects/{projectId}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                var projResponse = AutoFixture.CreateResponse<UpdateProjectResponse>();

                var mockResponse = new MockHttpResponseMessage<UpdateProjectResponse>(projResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var projectId = Guid.NewGuid();

                var result = await ApiClient.UpdateProjectAsync(projectId, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects/{projectId}");
            }
        }

        #endregion

        #region - ChangeOwnerAsync -

        public class ChangeOwnerAsync : ProjectsApiClientTest
        {
            [Fact]
            public async Task UpdatesProjectAsync()
            {
                var projResponse = AutoFixture.CreateResponse<UpdateProjectResponse>();

                var mockResponse = new MockHttpResponseMessage<UpdateProjectResponse>(projResponse);
                MockHttpClient.SetupResponse(mockResponse);

                var projectId = Guid.NewGuid();
                var ownerId = Guid.NewGuid();

                var result = await ApiClient.ChangeOwnerAsync(projectId, ownerId, Cancel);

                result.AssertSuccess();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects/{projectId}");
            }
        }

        #endregion

        #region - DeleteProjectAsync -

        public class DeleteProjectAsync : ProjectsApiClientTest
        {

            [Fact]
            public async Task Returns_success()
            {
                var projectId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.NoContent));

                var result = await ProjectsApiClient.DeleteProjectAsync(projectId, Cancel);

                result.AssertSuccess();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects/{projectId}");
                });
            }

            [Fact]
            public async Task Returns_failure()
            {
                var projectId = Guid.NewGuid();

                MockHttpClient.SetupResponse(new MockHttpResponseMessage(HttpStatusCode.InternalServerError));

                var result = await ProjectsApiClient.DeleteProjectAsync(projectId, Cancel);

                result.AssertFailure();

                MockHttpClient.AssertSingleRequest(r =>
                {
                    r.AssertHttpMethod(HttpMethod.Delete);
                    r.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/projects/{projectId}");
                });
            }
        }

        #endregion
    }
}
