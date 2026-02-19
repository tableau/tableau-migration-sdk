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
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;
using Tableau.Migration.Net;
using Tableau.Migration.Tests.Unit.Content.Files;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class WorkbooksApiClientTests
    {
        public class WorkbooksApiClientTest : ApiClientTestBase<IWorkbooksApiClient, WorkbookResponse.WorkbookType>
        { }

        public class Permissions : PermissionsContentApiClientTestBase<IWorkbooksApiClient, WorkbookResponse.WorkbookType>
        {
            protected override ICollection<WorkbookResponse.WorkbookType> GetContentData() => Api.Data.Workbooks;
        }

        #region - GetPageAsync -

        public class GetPageAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                Api.Data.CreateWorkbooks(AutoFixture, 11);

                // Act
                var result = await sitesClient.Workbooks.GetPageAsync(1, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.Workbooks.GetPageAsync(2, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.Workbooks.GetPageAsync(3, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Single(result.Value);
            }
        }

        #endregion

        #region - GetByIdAsync -

        public class GetByIdAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbook = Api.Data.CreateWorkbook(AutoFixture);

                foreach (var view in workbook.Views)
                {
                    Api.Data.AddView(new ViewResponse.ViewType()
                    {
                        Id = view.Id,
                        ContentUrl = view.ContentUrl,
                        Name = view.Name,
                        Tags = view.Tags.Select(t => new ViewResponse.ViewType.TagType() { Label = t.Label }).ToArray(),
                    });
                    Api.Data.CreateViewPermissions(AutoFixture, view, view.Id, view.Name);
                }

                var result = await ((IReadApiClient<IWorkbookDetails>)sitesClient.Workbooks).GetByIdAsync(workbook.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        #endregion

        #region - DownloadWorkbookAsync -

        public class DownloadWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbook = Api.Data.CreateWorkbook(AutoFixture);

                Api.Data.AddWorkbook(workbook, fileData: Constants.DefaultEncoding.GetBytes(workbook.ToXml()));

                var result = await sitesClient.Workbooks.DownloadWorkbookAsync(workbook.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        #endregion

        #region - PublishWorkbookAsync -

        public class PublishWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbookResponse = Create<WorkbookResponse.WorkbookType>();

                var project = Api.Data.CreateProject(AutoFixture);

                var mockPublishable = Create<Mock<IPublishableWorkbook>>()
                    .WithProject(project);

                mockPublishable.SetupGet(x => x.File).Returns(new MockXmlFileHandle(new SimulatedWorkbookData().ToXml()).Object);

                var options = new PublishWorkbookOptions(mockPublishable.Object, WorkbookFileTypes.Twbx);

                var result = await sitesClient.Workbooks.PublishWorkbookAsync(options, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        #endregion

        #region - PublishAsync -

        public class PublishAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var workbook = Api.Data.CreateWorkbook(AutoFixture, project, owner);

                var publishableWbResult = await sitesClient.Workbooks.PullAsync(
                    new Workbook(
                        workbook,
                        new Project(project, null, Create<IContentReference>()),
                        new User(owner)),
                    Cancel);

                Assert.Empty(publishableWbResult.Errors);
                Assert.True(publishableWbResult.Success);
                Assert.NotNull(publishableWbResult.Value);

                var result = await sitesClient.Workbooks.PublishAsync(publishableWbResult.Value, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        #endregion

        #region - UpdateWorkbookAsync -

        public class UpdateWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task NoChangesSucceedsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(workbookResponse.Item.Name, result.Value.Name);
                Assert.Equal(workbookResponse.Item.Description, result.Value.Description);
                Assert.Equal(workbookResponse.Item.ShowTabs, result.Value.ShowTabs);
                Assert.Equal(workbookResponse.Item.EncryptExtracts, result.Value.EncryptExtracts);
            }

            [Fact]
            public async Task SetNameSucceedsAsync()
            {
                // Arrange
                var newName = "Test1234";
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel,
                    newName: newName);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(newName, result.Value.Name);
                Assert.Equal(workbookResponse.Item.ShowTabs, result.Value.ShowTabs);
                Assert.Equal(workbookResponse.Item.EncryptExtracts, result.Value.EncryptExtracts);
            }

            [Fact]
            public async Task SetShowTabsSucceedsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                workbookResponse.Item!.ShowTabs = false;

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel,
                    newShowTabs: true);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(workbookResponse.Item.Name, result.Value.Name);
                Assert.True(result.Value.ShowTabs);
                Assert.Equal(workbookResponse.Item.EncryptExtracts, result.Value.EncryptExtracts);
            }

            [Fact]
            public async Task ClearShowTabsSucceedsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                workbookResponse.Item!.ShowTabs = true;

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel,
                    newShowTabs: false);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(workbookResponse.Item.Name, result.Value.Name);
                Assert.False(result.Value.ShowTabs);
                Assert.Equal(workbookResponse.Item.EncryptExtracts, result.Value.EncryptExtracts);
            }

            [Fact]
            public async Task SetEncryptedExtractsSucceedsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                workbookResponse.Item!.EncryptExtracts = false;

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel,
                    newEncryptExtracts: true);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(workbookResponse.Item.Name, result.Value.Name);
                Assert.Equal(workbookResponse.Item.ShowTabs, result.Value.ShowTabs);
                Assert.True(result.Value.EncryptExtracts);
            }

            [Fact]
            public async Task SetProjectSucceedsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                workbookResponse.Item!.EncryptExtracts = false;

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                var movingProject = projects.Items.LastOrDefault();
                Assert.NotNull(project);
                Assert.NotNull(movingProject);

                Api.Data.AddProject(project, null);
                Api.Data.AddProject(movingProject, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel,
                    newProjectId: movingProject.Id);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(workbookResponse.Item.Name, result.Value.Name);
                Assert.Equal(workbookResponse.Item.ShowTabs, result.Value.ShowTabs);
                Assert.Equal(workbookResponse.Item.EncryptExtracts, result.Value.EncryptExtracts);
            }

            [Fact]
            public async Task SetOwnerSucceedsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbookResponse = Create<WorkbookResponse>();
                Assert.NotNull(workbookResponse.Item);

                workbookResponse.Item!.EncryptExtracts = false;

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null);
                workbookResponse.Item.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                var owners = Create<UsersResponse>();
                var owner = owners.Items.FirstOrDefault();
                var newOwner = owners.Items.LastOrDefault();
                Assert.NotNull(owner);
                Assert.NotNull(newOwner);

                Api.Data.AddUser(owner);
                Api.Data.AddUser(newOwner);
                workbookResponse.Item.Owner = new WorkbookResponse.WorkbookType.OwnerType
                {
                    Id = owner.Id
                };

                Api.Data.AddWorkbook(workbookResponse.Item, fileData: Constants.DefaultEncoding.GetBytes(workbookResponse.Item.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.UpdateWorkbookAsync(
                    workbookResponse.Item.Id,
                    Cancel,
                    newOwnerId: newOwner.Id);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(workbookResponse.Item.Name, result.Value.Name);
                Assert.Equal(workbookResponse.Item.Description, result.Value.Description);
                Assert.Equal(workbookResponse.Item.ShowTabs, result.Value.ShowTabs);
                Assert.Equal(workbookResponse.Item.EncryptExtracts, result.Value.EncryptExtracts);
            }
        }

        #endregion

        #region - AddTagsAsync -

        public class AddTagsAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbookResponse = Create<WorkbookResponse>();

                var workbook = workbookResponse.Item;

                Assert.NotNull(workbook);

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null, useSignInOwner: true);
                workbook.Project = new()
                {
                    Id = project.Id,
                    Name = project.Name
                };

                var owner = Create<UsersResponse.UserType>();
                Api.Data.AddUser(owner);
                workbook.Owner = new()
                {
                    Id = owner.Id
                };

                var tagsCountBefore = workbook.Tags.Length;
                Api.Data.AddWorkbook(workbook, null);

                var testTags = CreateMany<ITag>().ToArray();

                var result = await sitesClient.Workbooks.Tags.AddTagsAsync(workbook.Id, testTags, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                var getResult = await ((IReadApiClient<IWorkbookDetails>)sitesClient.Workbooks).GetByIdAsync(workbook.Id, Cancel);
                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                Assert.Equal(tagsCountBefore + testTags.Length, getResult.Value.Tags is null ? 0 : getResult.Value.Tags.Count);
            }

            [Fact]
            public async Task NoDuplicatesInsertedAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbookResponse = Create<WorkbookResponse>();

                var workbook = workbookResponse.Item;

                Assert.NotNull(workbook);

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null, useSignInOwner: true);
                workbook.Project = new()
                {
                    Id = project.Id,
                    Name = project.Name
                };

                var owner = Create<UsersResponse.UserType>();
                Api.Data.AddUser(owner);
                workbook.Owner = new()
                {
                    Id = owner.Id
                };

                var tagsCountBefore = workbook.Tags.Length;
                Api.Data.AddWorkbook(workbook, null);

                var testTags = workbook.Tags.Select(tag => new Tag(tag)).ToArray();

                Assert.NotNull(testTags);

                var result = await sitesClient.Workbooks.Tags.AddTagsAsync(workbook.Id, testTags, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                var getResult = await ((IReadApiClient<IWorkbookDetails>)sitesClient.Workbooks).GetByIdAsync(workbook.Id, Cancel);
                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                Assert.Equal(tagsCountBefore, getResult.Value.Tags.Count);
            }
        }

        #endregion

        #region - RemoveTagsAsync -

        public class RemoveTagsAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbookResponse = Create<WorkbookResponse>();

                var workbook = workbookResponse.Item;

                Assert.NotNull(workbook);

                var projects = Create<ProjectsResponse>();
                var project = projects.Items.FirstOrDefault();
                Assert.NotNull(project);

                Api.Data.AddProject(project, null, useSignInOwner: true);
                workbook.Project = new WorkbookResponse.WorkbookType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                var owner = Create<UsersResponse.UserType>();
                Api.Data.AddUser(owner);
                workbook.Owner = new()
                {
                    Id = owner.Id
                };

                var tagsCountBefore = workbook.Tags.Length;
                Api.Data.AddWorkbook(workbook, null);

                var tagsToRemove = workbook.Tags.Select(t => new Tag(t)).ToArray();
                Assert.NotNull(tagsToRemove);

                var result = await sitesClient.Workbooks.Tags.RemoveTagsAsync(workbook.Id, tagsToRemove, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                var getResult = await ((IReadApiClient<IWorkbookDetails>)sitesClient.Workbooks).GetByIdAsync(workbook.Id, Cancel);
                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                Assert.Equal(tagsCountBefore - tagsToRemove.Length, getResult.Value.Tags.Count);
            }
        }

        #endregion

        #region - Connections -

        public class ListConnectionsAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var project = Create<ProjectsResponse.ProjectType>();
                var user = Create<UsersResponse.UserType>();
                var fileData = Create<SimulatedWorkbookData>();

                var workbook = Api.Data.CreateWorkbook(AutoFixture, project, user, Constants.DefaultEncoding.GetBytes(fileData.ToXml()));

                var result = await sitesClient.Workbooks.GetConnectionsAsync(workbook.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        public class UpdateConnectionAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var project = Create<ProjectsResponse.ProjectType>();
                var user = Create<UsersResponse.UserType>();
                var fileData = Create<SimulatedWorkbookData>();
                var connectionToUpdate = fileData.Connections.First();
                Assert.NotNull(connectionToUpdate);

                var workbook = Api.Data.CreateWorkbook(AutoFixture, project, user, Constants.DefaultEncoding.GetBytes(fileData.ToXml()));

                var updateOptions = Create<UpdateConnectionOptions>();

                var result = await sitesClient.Workbooks.UpdateConnectionAsync(
                    workbook.Id,
                    connectionToUpdate.Id,
                    updateOptions,
                    Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                var updatedConnection = result.Value;
                Assert.NotNull(updatedConnection);

                Assert.Equal(updateOptions.ServerAddress, updatedConnection.ServerAddress);
                Assert.Equal(updateOptions.ServerPort, updatedConnection.ServerPort);
                Assert.Equal(updateOptions.ConnectionUsername, updatedConnection.ConnectionUsername);
                Assert.Equal(updateOptions.QueryTaggingEnabled, updatedConnection.QueryTaggingEnabled);
            }
        }

        #endregion

        #region - GetWorkbookViewsAsync -

        public class GetWorkbookViewsAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var workbook = Api.Data.CreateWorkbook(AutoFixture, project, owner);

                // Create simulated workbook data with views
                var simulatedWorkbook = new SimulatedWorkbookData();

                simulatedWorkbook.Views.Add(new SimulatedWorkbookData.SimulatedViewType
                {
                    View = AutoFixture.Build<ViewResponse.ViewType>()
                    .With(v => v.Name, "Test View 1")
                    .With(v => v.ContentUrl, "test-view-1")
                    .With(v => v.ViewUrlName, "test-view-1")
                    .Create()

                });
                simulatedWorkbook.Views.Add(new SimulatedWorkbookData.SimulatedViewType
                {
                    View = AutoFixture.Build<ViewResponse.ViewType>()
                    .With(v => v.Name, "Test View 2")
                    .With(v => v.ContentUrl, "test-view-2")
                    .With(v => v.ViewUrlName, "test-view-2")
                    .Create()
                });

                // Add workbook with simulated data
                Api.Data.AddWorkbook(workbook, Constants.DefaultEncoding.GetBytes(simulatedWorkbook.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.GetWorkbookViewsAsync(workbook.Id, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                Assert.NotNull(result.Value);
                Assert.Equal(2, result.Value.Count);

                var views = result.Value.ToList();
                Assert.Equal("Test View 1", views[0].Name);
                Assert.Equal("test-view-1", views[0].ContentUrl);
                Assert.Equal("Test View 2", views[1].Name);
                Assert.Equal("test-view-2", views[1].ContentUrl);
            }

            [Fact]
            public async Task ReturnsEmptyWhenNoViewsAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var workbook = Api.Data.CreateWorkbook(AutoFixture, project, owner);

                // Create simulated workbook data with no views
                var simulatedWorkbook = new SimulatedWorkbookData();
                Api.Data.AddWorkbook(workbook, Constants.DefaultEncoding.GetBytes(simulatedWorkbook.ToXml()));

                // Act
                var result = await sitesClient.Workbooks.GetWorkbookViewsAsync(workbook.Id, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                Assert.NotNull(result.Value);
                Assert.Empty(result.Value);
            }

            [Fact]
            public async Task ReturnsEmptyWhenWorkbookNotFoundAsync()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                // Act
                var result = await sitesClient.Workbooks.GetWorkbookViewsAsync(Create<Guid>(), Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Errors);
                Assert.NotNull(result.Value);
                Assert.Empty(result.Value);
            }
        }

        #endregion
    }
}

