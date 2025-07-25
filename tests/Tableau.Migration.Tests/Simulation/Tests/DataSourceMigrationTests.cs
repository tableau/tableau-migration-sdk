﻿//
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
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class DataSourceMigrationTests
    {
        public class UsersBatch : ServerToCloud
        {
        }

        public class UsersIndividual : ServerToCloud
        {
            protected override bool UsersBatchImportEnabled => false;
        }

        public abstract class ServerToCloud : ServerToCloudSimulationTestBase
        {
            [Fact]
            public async Task MigratesAllDataSourcesToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceDataSources = PrepareSourceDataSourceData();

                //Migrate
                var result = await RunMigrationWithTableauIdAuthAsync();

                //Assert - all projects should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(CloudDestinationApi.Data.DataSources.Count,
                    result.Manifest.Entries.ForContentType<IDataSource>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());

                Assert.All(sourceDataSources, AssertDataSourceMigrated);

                void AssertDataSourceMigrated(DataSourceResponse.DataSourceType sourceDataSource)
                {
                    var destinationDataSource = Assert.Single(CloudDestinationApi.Data.DataSources, ds =>
                            ds.Name == sourceDataSource.Name &&
                            ds.Description == sourceDataSource.Description);

                    Assert.NotEqual(sourceDataSource.Id, destinationDataSource.Id);
                    Assert.Equal(sourceDataSource.Name, destinationDataSource.Name);

                    AssertPermissionsMigrated(result.Manifest,
                            SourceApi.Data.DataSourcePermissions[sourceDataSource.Id],
                            CloudDestinationApi.Data.DataSourcePermissions[destinationDataSource.Id]);

                    AssertEmbeddedCredentialsMigrated(result.Manifest,
                        SourceApi.Data.DataSourceKeychains[sourceDataSource.Id],
                        CloudDestinationApi.Data.DataSourceKeychains[destinationDataSource.Id],
                        SourceApi.Data.UserSavedCredentials,
                        CloudDestinationApi.Data.UserSavedCredentials);

                    //Ownership was mapped so should equal the transformed ID.
                    Assert.NotNull(destinationDataSource.Owner);
                    Assert.NotEqual(destinationDataSource.Owner.Id, Guid.Empty);
                    Assert.NotEqual(destinationDataSource.Owner.Id, sourceDataSource.Owner?.Id);

                    sourceDataSource.Tags.AssertEqual(destinationDataSource.Tags);
                }
            }

            [Fact]
            public async Task MappedToNonMigratedProjectAsync()
            {
                //Arrange - create source content to migrate.
                var sourceDataSources = PrepareSourceDataSourceData();

                var destinationOnlyOwner = CloudDestinationApi.Data.AddUser(new()
                {
                    Id = Guid.NewGuid(),
                    Name = "owner",
                    FullName = "Destination Owner",
                    SiteRole = SiteRoles.Creator,
                    Domain = new() { Name = Constants.LocalDomain }
                });
                var destinationOnlyProject = CloudDestinationApi.Data.AddProject(new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Destination Project",
                    ParentProjectId = null,
                    ContentPermissions = ContentPermissions.ManagedByOwner,
                    Owner = new() { Id = destinationOnlyOwner.Id }
                });

                //Migrate
                var planBuilder = CreateMigrationPlanBuilderWithTableauIdAuth();

                planBuilder.Mappings.Add<IDataSource>(ds =>
                {
                    return ds.MapTo(new(destinationOnlyProject.Name!, ds.MappedLocation.Name));
                });

                var result = await RunMigrationAsync(planBuilder);

                //Assert - all data sources should be migrated.
                Assert.All(result.Manifest.Entries.ForContentType<IDataSource>(), e => Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status));
            }
        }
    }
}
