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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Net;
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

        #region - GetAllWorkbooksAsync -

        public class GetAllWorkbooksAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                Api.Data.CreateWorkbooks(AutoFixture, 11);

                // Act
                var result = await sitesClient.Workbooks.GetAllWorkbooksAsync(1, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.Workbooks.GetAllWorkbooksAsync(2, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.Workbooks.GetAllWorkbooksAsync(3, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Single(result.Value);
            }
        }

        #endregion

        #region - GetWorkbookAsync -

        public class GetWorkbookAsync : WorkbooksApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var workbook = Api.Data.CreateWorkbook(AutoFixture);

                foreach (var view in workbook.Views)
                {
                    Api.Data.AddView(view);
                    Api.Data.CreateViewPermissions(AutoFixture, view, view.Id, view.Name);
                }

                var result = await sitesClient.Workbooks.GetWorkbookAsync(workbook.Id, Cancel);

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
            public async Task Returns_success_on_success()
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
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var workbookResponse = Create<WorkbookResponse.WorkbookType>();

                var project = Api.Data.CreateProject(AutoFixture);

                var mockPublishable = Create<Mock<IPublishableWorkbook>>()
                    .WithProject(project);

                var options = new PublishWorkbookOptions(
                    mockPublishable.Object,
                    new MemoryStream(Constants.DefaultEncoding.GetBytes(new SimulatedWorkbookData().ToXml())),
                    WorkbookFileTypes.Twbx);

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
            public async Task Returns_success_on_success()
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
            public async Task No_changes_returns_success()
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
            public async Task Set_name_returns_success()
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
            public async Task Set_show_tabs_returns_success()
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
            public async Task Clear_show_tabs_returns_success()
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
            public async Task Set_excrypt_extracts_returns_success()
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
            public async Task Set_project_returns_success()
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
            public async Task Set_owner_returns_success()
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
            public async Task Returns_success_on_success()
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

                var getResult = await sitesClient.Workbooks.GetWorkbookAsync(workbook.Id, Cancel);
                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                Assert.Equal(tagsCountBefore + testTags.Length, getResult.Value.Tags is null ? 0 : getResult.Value.Tags.Count);
            }

            [Fact]
            public async Task No_Duplicates_Inserted()
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

                var getResult = await sitesClient.Workbooks.GetWorkbookAsync(workbook.Id, Cancel);
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
            public async Task Returns_success_on_success()
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

                var getResult = await sitesClient.Workbooks.GetWorkbookAsync(workbook.Id, Cancel);
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
            public async Task Returns_success_on_success()
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
            public async Task Returns_success_on_success()
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
    }
}

