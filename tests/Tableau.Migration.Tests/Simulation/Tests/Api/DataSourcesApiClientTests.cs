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
    public class DataSourcesApiClientTests
    {
        public class DataSourcesApiClientTest : ApiClientTestBase<IDataSourcesApiClient, DataSourceResponse.DataSourceType>
        { }

        public class GetAllPublishedDataSourcesAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                const int PROJECT_COUNT = 3;

                var projects = Api.Data.CreateProjects(AutoFixture, PROJECT_COUNT);

                const int OWNER_COUNT = 4;

                var owners = Api.Data.CreateUsers(AutoFixture, OWNER_COUNT);

                const int DATA_SOURCE_COUNT = 11;

                for (var i = 0; i != DATA_SOURCE_COUNT; i++)
                {
                    Api.Data.CreateDataSource(AutoFixture, projects[i % projects.Count], owners[i % owners.Count]);
                }

                // Act
                var result = await sitesClient.DataSources.GetAllPublishedDataSourcesAsync(1, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.DataSources.GetAllPublishedDataSourcesAsync(2, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(5, result.Value.Count);

                // Act
                result = await sitesClient.DataSources.GetAllPublishedDataSourcesAsync(3, 5, Cancel);

                // Assert                
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Single(result.Value);
            }
        }

        public class PublishDataSourceAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var dataSourceResponse = Create<DataSourceResponse.DataSourceType>();

                var project = Api.Data.CreateProject(AutoFixture);

                var mockPublishable = Create<Mock<IPublishableDataSource>>()
                    .WithProject(project);

                var options = new PublishDataSourceOptions(
                    mockPublishable.Object,
                    new MemoryStream(Constants.DefaultEncoding.GetBytes(dataSourceResponse.ToXml())),
                    DataSourceFileTypes.Tdsx);

                var result = await sitesClient.DataSources.PublishDataSourceAsync(options, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        public class PublishAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture, project, owner);

                var publishableDsResult = await sitesClient.DataSources.PullAsync(
                    new DataSource(
                        dataSource,
                        new Project(project, null, Create<IContentReference>()),
                        new User(owner)),
                    Cancel);

                Assert.Empty(publishableDsResult.Errors);
                Assert.True(publishableDsResult.Success);
                Assert.NotNull(publishableDsResult.Value);

                var result = await sitesClient.DataSources.PublishAsync(publishableDsResult.Value, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        public class GetDataSourceAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                var result = await sitesClient.DataSources.GetDataSourceAsync(
                    dataSource.Id,
                    Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.NotNull(result.Value.CertificationNote);
                Assert.Equal(dataSource.CertificationNote, result.Value.CertificationNote);
            }
        }

        public class UpdateDataSourceAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task No_changes_returns_success()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(dataSource.Name, result.Value.Name);
                Assert.Equal(dataSource.IsCertified, result.Value.IsCertified);
                Assert.Equal(dataSource.CertificationNote, result.Value.CertificationNote);
                Assert.Equal(dataSource.EncryptExtracts, result.Value.EncryptExtracts);
                Assert.Equal(dataSource.Project!.Id, result.Value.ProjectId);
                Assert.Equal(dataSource.Owner!.Id, result.Value.OwnerId);
            }

            [Fact]
            public async Task Set_name_returns_success()
            {
                // Arrange
                var newName = "Test1234";
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel,
                    newName: newName);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(newName, result.Value.Name);
                Assert.Equal(dataSource.IsCertified, result.Value.IsCertified);
                Assert.Equal(dataSource.CertificationNote, result.Value.CertificationNote);
                Assert.Equal(dataSource.EncryptExtracts, result.Value.EncryptExtracts);
                Assert.Equal(dataSource.Project!.Id, result.Value.ProjectId);
                Assert.Equal(dataSource.Owner!.Id, result.Value.OwnerId);
            }

            [Fact]
            public async Task Set_certification_returns_success()
            {
                // Arrange
                var newCertification = "New Certification Note";
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel,
                    newIsCertified: true,
                    newCertificationNote: newCertification);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(dataSource.Name, result.Value.Name);
                Assert.True(result.Value.IsCertified);
                Assert.Equal(newCertification, result.Value.CertificationNote);
                Assert.Equal(dataSource.EncryptExtracts, result.Value.EncryptExtracts);
                Assert.Equal(dataSource.Project!.Id, result.Value.ProjectId);
                Assert.Equal(dataSource.Owner!.Id, result.Value.OwnerId);
            }

            [Fact]
            public async Task Clear_certification_returns_success()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                Assert.NotNull(dataSource);
                dataSource.IsCertified = true;

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel,
                    newIsCertified: false);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(dataSource.Name, result.Value.Name);
                Assert.False(result.Value.IsCertified);
                Assert.Empty(result.Value.CertificationNote!);
                Assert.Equal(dataSource.EncryptExtracts, result.Value.EncryptExtracts);
                Assert.Equal(dataSource.Project!.Id, result.Value.ProjectId);
                Assert.Equal(dataSource.Owner!.Id, result.Value.OwnerId);
            }

            [Fact]
            public async Task Set_encrypt_extracts_returns_success()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                dataSource.EncryptExtracts = false;

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel,
                    newEncryptExtracts: true);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(dataSource.Name, result.Value.Name);
                Assert.Equal(dataSource.IsCertified, result.Value.IsCertified);
                Assert.Equal(dataSource.CertificationNote, result.Value.CertificationNote);
                Assert.True(result.Value.EncryptExtracts);
                Assert.Equal(dataSource.Project!.Id, result.Value.ProjectId);
                Assert.Equal(dataSource.Owner!.Id, result.Value.OwnerId);
            }

            [Fact]
            public async Task Set_project_returns_success()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture, project, owner);

                var movingProject = Api.Data.CreateProject(AutoFixture);

                dataSource.Project = new DataSourceResponse.DataSourceType.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name
                };

                Api.Data.AddDataSource(dataSource, fileData: Constants.DefaultEncoding.GetBytes(dataSource.ToXml()));

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel,
                    newProjectId: movingProject.Id);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(dataSource.Name, result.Value.Name);
                Assert.Equal(dataSource.IsCertified, result.Value.IsCertified);
                Assert.Equal(dataSource.CertificationNote, result.Value.CertificationNote);
                Assert.Equal(dataSource.EncryptExtracts, result.Value.EncryptExtracts);
                Assert.Equal(movingProject.Id, result.Value.ProjectId);
                Assert.Equal(dataSource.Owner!.Id, result.Value.OwnerId);
            }

            [Fact]
            public async Task Set_owner_returns_success()
            {
                // Arrange
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture, project, owner);

                var newOwner = Api.Data.CreateUser(AutoFixture);

                dataSource.Owner = new DataSourceResponse.DataSourceType.OwnerType
                {
                    Id = owner.Id
                };

                Api.Data.AddDataSource(dataSource, fileData: Constants.DefaultEncoding.GetBytes(dataSource.ToXml()));

                // Act
                var result = await sitesClient.DataSources.UpdateDataSourceAsync(
                    dataSource.Id,
                    Cancel,
                    newOwnerId: newOwner.Id);

                // Assert
                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(dataSource.Name, result.Value.Name);
                Assert.Equal(dataSource.IsCertified, result.Value.IsCertified);
                Assert.Equal(dataSource.CertificationNote, result.Value.CertificationNote);
                Assert.Equal(dataSource.EncryptExtracts, result.Value.EncryptExtracts);
                Assert.Equal(dataSource.Project?.Id, result.Value.ProjectId);
                Assert.Equal(newOwner.Id, result.Value.OwnerId);
            }
        }

        public class AddTagsAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                var tagsCountBefore = dataSource.Tags == null ? 0 : dataSource.Tags.Length;
                Api.Data.AddDataSource(dataSource, null);

                var testTags = CreateMany<ITag>().ToArray();

                var result = await sitesClient.DataSources.Tags.AddTagsAsync(
                    dataSource.Id,
                    testTags,
                    Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                var getResult = await sitesClient.DataSources.GetDataSourceAsync(
                    dataSource.Id,
                    Cancel);

                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                Assert.Equal(tagsCountBefore + testTags.Length, getResult.Value.Tags is null ? 0 : getResult.Value.Tags.Count);
            }

            [Fact]
            public async Task No_Duplicates_Inserted()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture);

                var tagsCountBefore = dataSource.Tags.Length;
                Api.Data.AddDataSource(dataSource, null);

                var testTags = dataSource.Tags.Select(tag => new Tag(tag)).ToArray();

                Assert.NotNull(testTags);

                var result = await sitesClient.DataSources.Tags.AddTagsAsync(dataSource.Id, testTags, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                var getResult = await sitesClient.DataSources.GetDataSourceAsync(
                    dataSource.Id,
                    Cancel);

                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                var resultTags = getResult.Value.Tags;
                var tagsCountAfter = resultTags is null ? 0 : resultTags.Count;

                Assert.Equal(tagsCountBefore, tagsCountAfter);
            }
        }
        public class RemoveTagsAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture, project, owner);

                var tagsCountBefore = dataSource.Tags.Length;

                var tagsToRemove = dataSource.Tags.Select(t => new Tag(t)).ToArray();
                Assert.NotNull(tagsToRemove);

                var result = await sitesClient.DataSources.Tags.RemoveTagsAsync(dataSource.Id, tagsToRemove, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);

                var getResult = await sitesClient.DataSources.GetDataSourceAsync(
                    dataSource.Id,
                    Cancel);

                Assert.Empty(getResult.Errors);
                Assert.True(getResult.Success);
                Assert.NotNull(getResult.Value);
                Assert.Equal(tagsCountBefore - tagsToRemove.Length, getResult.Value.Tags is null ? 0 : getResult.Value.Tags.Count);
            }
        }

        public class PullAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var project = Api.Data.CreateProject(AutoFixture);
                var owner = Api.Data.CreateUser(AutoFixture);
                var dataSource = Api.Data.CreateDataSource(AutoFixture, project, owner);

                var result = await sitesClient.DataSources.PullAsync(
                    new DataSource(
                        dataSource,
                        new Project(project, null, Create<IContentReference>()),
                        new User(owner)),
                    Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        #region - Connections -

        public class ListConnectionsAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var project = Create<ProjectsResponse.ProjectType>();
                var user = Create<UsersResponse.UserType>();
                var fileData = Create<SimulatedDataSourceData>();

                var workbook = Api.Data.CreateDataSource(AutoFixture, project, user, Constants.DefaultEncoding.GetBytes(fileData.ToXml()));

                var result = await sitesClient.DataSources.GetConnectionsAsync(workbook.Id, Cancel);

                Assert.Empty(result.Errors);
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }
        }

        public class UpdateConnectionAsync : DataSourcesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);
                var project = Create<ProjectsResponse.ProjectType>();
                var user = Create<UsersResponse.UserType>();
                var fileData = Create<SimulatedDataSourceData>();
                var connectionToUpdate = fileData.Connections.First();
                Assert.NotNull(connectionToUpdate);

                var dataSource = Api.Data.CreateDataSource(
                    AutoFixture,
                    project,
                    user,
                    fileData);

                var updateOptions = Create<UpdateConnectionOptions>();

                var result = await sitesClient.DataSources.UpdateConnectionAsync(
                    dataSource.Id,
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