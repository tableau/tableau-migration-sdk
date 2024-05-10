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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content;
using Tableau.Migration.Tests.Content.Permissions;
using Tableau.Migration.Tests.Unit.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class ProjectsApiClientTests
    {
        public class ProjectsApiClientTest : ApiClientTestBase<IProjectsApiClient, ProjectsResponse.ProjectType>
        { }

        public class Permissions : PermissionsContentApiClientTestBase<IProjectsApiClient, ProjectsResponse.ProjectType>
        {
            protected override ICollection<ProjectsResponse.ProjectType> GetContentData() => Api.Data.Projects;
        }

        public class GetAllProjectsAsync : ProjectsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int PROJECT_COUNT = 10;

                for (var i = 0; i != PROJECT_COUNT; i++)
                {
                    Api.Data.AddProject(Create<ProjectsResponse.ProjectType>(), useSignInOwner: true);
                }

                // Act
                var result = await sitesClient.Projects.GetAllAsync(100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_success_with_no_item_when_no_users_exist()
            {
                // Arrange 
                Api.Data.Projects.Clear();

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.Projects.GetAllAsync(100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Value);
            }
        }

        public class GetPermissionsAsync : ProjectsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var testProject = Create<ProjectsResponse.ProjectType>();
                Api.Data.Projects.Add(testProject);

                var testGranteeCapabilities = new GranteeCapabilityType[]
                                         {
                                            new GranteeCapabilityType()
                                            {
                                                User= new GranteeCapabilityType.UserType()
                                                {
                                                    Id=Guid.NewGuid()
                                                },
                                                Capabilities = new CapabilityType[]
                                                {
                                                    new CapabilityType()
                                                    {
                                                        Name= PermissionsCapabilityNames.Read,
                                                        Mode= PermissionsCapabilityModes.Allow
                                                    }
                                                }
                                            }
                                         };

                var testPermissions = new PermissionsType()
                {
                    GranteeCapabilities = testGranteeCapabilities
                };

                Api.Data.AddProjectPermissions(testProject, testPermissions);

                // Act
                var result = await sitesClient.Projects.Permissions.GetPermissionsAsync(testProject.Id, Cancel);

                // Assert
                Assert.True(result.Success);
                var permissions = result.Value;
                Assert.NotNull(permissions);

                Assert.Equal(testGranteeCapabilities.ToIGranteeCapabilities(), permissions.GranteeCapabilities, IGranteeCapabilityComparer.Instance);
            }
        }

        public class CreateProjectAsync : ProjectsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var mockOptions = Create<Mock<ICreateProjectOptions>>();
                mockOptions.SetupGet(o => o.ParentProject).Returns((IContentReference?)null);

                // Act
                var result = await sitesClient.Projects.CreateProjectAsync(mockOptions.Object, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_success_for_child_project()
            {
                // Arrange 
                var parentProject = Create<ProjectsResponse.ProjectType>();
                Api.Data.AddProject(parentProject, Create<ProjectsResponse.ProjectType>());

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var mockOptions = Create<Mock<ICreateProjectOptions>>();
                mockOptions.SetupGet(o => o.ParentProject).Returns(new Project(parentProject, null, Create<IContentReference>()));

                // Act
                var result = await sitesClient.Projects.CreateProjectAsync(mockOptions.Object, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        public class PublishAsync : ProjectsApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = CreateProject(null);

                // Act
                var result = await sitesClient.Projects.PublishAsync(project, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_success_for_child_project()
            {
                // Arrange 
                var parentProject = Create<ProjectsResponse.ProjectType>();
                Api.Data.AddProject(parentProject, Create<ProjectsResponse.ProjectType>());

                var project = CreateProject(parentProject);

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.Projects.PublishAsync(project, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            private IProject CreateProject(ProjectsResponse.ProjectType? parentProject)
            {
                var parentContentReference = parentProject is not null
                    ? new Project(parentProject, null, Create<IContentReference>())
                    : null;

                var mockProject = Create<Mock<IProject>>();
                mockProject.SetupGet(p => p.ParentProject).Returns(parentContentReference);

                return mockProject.Object;
            }
        }
    }
}
