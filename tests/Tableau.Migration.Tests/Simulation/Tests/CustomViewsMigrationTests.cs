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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class CustomViewsMigrationTests
    {
        public class UsersBatch : ServerToCloud
        { }

        public class UsersIndividual : ServerToCloud
        {
            protected override bool UsersBatchImportEnabled => false;
        }

        public abstract class ServerToCloud : ServerToCloudSimulationTestBase
        {
            [Fact]
            public async Task MigratesAllCustomViewsToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();
                var sourceCustomViews = PrepareSourceCustomViewsData();

                //Migrate
                var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .FromSource(SourceEndpointConfig)
                    .ToDestination(CloudDestinationEndpointConfig)
                    .ForServerToCloud()
                    .WithTableauIdAuthenticationType()
                    .WithTableauCloudUsernames("test.com")
                    .Build();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();
                var result = await migrator.ExecuteAsync(plan, Cancel);

                //Assert - all custom views should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                var customViewManifestEntries = result.Manifest.Entries.ForContentType<ICustomView>().ToList();
                Assert.Equal(
                    CloudDestinationApi.Data.CustomViews.Count,
                    customViewManifestEntries.Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());

                Assert.All(sourceCustomViews, AssertCustomViewMigrated);

                void AssertCustomViewMigrated(CustomViewResponse.CustomViewType sourceCustomView)
                {
                    // Get destination custom view
                    var destinationCustomView = Assert.Single(
                        CloudDestinationApi.Data.CustomViews.Where(cv =>
                             cv.Name == sourceCustomView.Name
                         ));

                    Assert.NotEqual(sourceCustomView.Id, destinationCustomView.Id);
                    Assert.Equal(sourceCustomView.Name, destinationCustomView.Name);

                    Assert.NotEqual(sourceCustomView.CreatedAt, destinationCustomView.CreatedAt);

                    Assert.Equal(sourceCustomView.Shared, destinationCustomView.Shared);

                    Assert.NotEqual(sourceCustomView.View?.Id, destinationCustomView.View?.Id);

                    Assert.NotEqual(sourceCustomView.Workbook?.Id, destinationCustomView.Workbook?.Id);

                    Assert.NotEqual(sourceCustomView.Owner?.Id, destinationCustomView.Owner?.Id);
                    Assert.Equal(sourceCustomView.Owner?.Name, destinationCustomView.Owner?.Name);

                    CloudDestinationApi.Data.CustomViewDefaultUsers.TryGetValue(
                        destinationCustomView.Id,
                        out List<UsersWithCustomViewAsDefaultViewResponse.UserType>? destinationDefaultUsers);

                    SourceApi.Data.CustomViewDefaultUsers.TryGetValue(
                        sourceCustomView.Id,
                        out List<UsersWithCustomViewAsDefaultViewResponse.UserType>? sourceDefaultUsers);

                    AssertCustomViewDefaultUsers(sourceDefaultUsers, destinationDefaultUsers);
                }

                void AssertCustomViewDefaultUsers(
                    List<UsersWithCustomViewAsDefaultViewResponse.UserType>? sourceDefaultUsers,
                    List<UsersWithCustomViewAsDefaultViewResponse.UserType>? destinationDefaultUsers)
                {
                    if (sourceDefaultUsers == null)
                    {
                        Assert.Null(destinationDefaultUsers);
                        return;
                    }

                    Assert.NotNull(destinationDefaultUsers);
                    Assert.Equal(sourceDefaultUsers.Count, destinationDefaultUsers.Count);

                    // Assert that the destination user references have been updated
                    foreach (var sourceDefaultUser in sourceDefaultUsers)
                    {
                        Assert.DoesNotContain(destinationDefaultUsers, d => d.Id == sourceDefaultUser.Id);
                    }
                }
            }
        }
    }
}