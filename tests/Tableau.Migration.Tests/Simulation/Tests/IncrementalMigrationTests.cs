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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class IncrementalMigrationTests
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
            private IMigrationPlanBuilder ConfigurePlanBuilder()
            {
                return ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .FromSource(SourceEndpointConfig)
                    .ToDestination(CloudDestinationEndpointConfig)
                    .ForServerToCloud()
                    .WithTableauIdAuthenticationType()
                    .WithTableauCloudUsernames("test.com");
            }

            [Fact]
            public async Task IncrementalFilterAsync()
            {
                //Scenario: We are migrating workbooks but have mistakenly
                //included a filter that prevents some of the workbooks from migrating.
                //We want to re-run the migration and only migrate the workbooks we missed the first run.

                //Arrange - create source content to migrate.
                var sourceProjects = PrepareSourceProjectsData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();

                var filteredWorkbooksIds = sourceWorkbooks
                    .Where((w, i) => i % 2 == 1)
                    .Select(w => w.Id)
                    .ToImmutableHashSet();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();

                //First migration, filters out some workbooks.
                var planBuilder = ConfigurePlanBuilder();

                planBuilder.Filters.Add<IWorkbook>(items => items.Where(i => !filteredWorkbooksIds.Contains(i.SourceItem.Id)));

                var plan = planBuilder.Build();
                var result1 = await migrator.ExecuteAsync(plan, Cancel);

                //Sanity test our 'mistake' filter worked.
                Assert.Equal(sourceWorkbooks.Count - filteredWorkbooksIds.Count, CloudDestinationApi.Data.Workbooks.Count);

                Assert.All(result1.Manifest.Entries.ForContentType<IWorkbook>(), we =>
                {
                    if (filteredWorkbooksIds.Contains(we.Source.Id))
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Skipped, we.Status);
                        Assert.False(we.HasMigrated);
                    }
                    else
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Migrated, we.Status);
                        Assert.True(we.HasMigrated);
                    }
                });

                //Perform second (incremental) migration
                planBuilder = ConfigurePlanBuilder();
                plan = planBuilder.Build();

                var result2 = await migrator.ExecuteAsync(plan, result1.Manifest, Cancel);

                //Assert everything moved eventually, but only the workbooks that were filtered out
                //last run were migrated this run.
                Assert.Equal(sourceWorkbooks.Count, CloudDestinationApi.Data.Workbooks.Count);

                Assert.All(result2.Manifest.Entries.ForContentType<IWorkbook>(), we =>
                {
                    if (filteredWorkbooksIds.Contains(we.Source.Id))
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Migrated, we.Status);
                        Assert.True(we.HasMigrated);
                    }
                    else
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Skipped, we.Status);
                        Assert.True(we.HasMigrated);
                    }
                });
            }

            [Fact]
            public async Task ContentAfterStructureAsync()
            {
                //Scenario: A user migrates the users/groups/project structure in a first run.
                //then migrates data source/workbook content in a second run.

                //Arrange - create source content to migrate.
                (var sourceUsers, var sourceSupportUsers) = PrepareSourceUsersData();
                var sourceGroups = PrepareSourceGroupsData();
                var sourceProjects = PrepareSourceProjectsData();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();

                //First migration, filters out some workbooks.
                var planBuilder = ConfigurePlanBuilder();

                var plan = planBuilder.Build();
                var result1 = await migrator.ExecuteAsync(plan, Cancel);

                //Sanity test our first migration worked.
                Assert.All(result1.Manifest.Entries.ForContentType<IUser>(), e =>
                {
                    //Don't expect support users to migrate.
                    if (sourceUsers.Any(u => u.Id == e.Source.Id))
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status);
                        Assert.True(e.HasMigrated);
                    }
                });
                Assert.All(result1.Manifest.Entries.ForContentType<IGroup>(), e =>
                {
                    //Don't expect the all users group to migrate.
                    if (sourceGroups.Any(u => u.Id == e.Source.Id))
                    {
                        Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status);
                        Assert.True(e.HasMigrated);
                    }
                });
                Assert.All(result1.Manifest.Entries.ForContentType<IProject>(), e => Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status));

                //Perform second (incremental) migration
                var sourceDataSources = PrepareSourceDataSourceData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();

                planBuilder = ConfigurePlanBuilder();
                plan = planBuilder.Build();

                var result2 = await migrator.ExecuteAsync(plan, result1.Manifest, Cancel);

                //Assert everything moved eventually, but only the workbooks that were filtered out
                //last run were migrated this run.
                Assert.All(result2.Manifest.Entries.ForContentType<IUser>(), e => Assert.Equal(MigrationManifestEntryStatus.Skipped, e.Status));
                Assert.All(result2.Manifest.Entries.ForContentType<IGroup>(), e => Assert.Equal(MigrationManifestEntryStatus.Skipped, e.Status));
                Assert.All(result2.Manifest.Entries.ForContentType<IProject>(), e => Assert.Equal(MigrationManifestEntryStatus.Skipped, e.Status));
                Assert.All(result2.Manifest.Entries.ForContentType<IDataSource>(), e => Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status));
                Assert.All(result2.Manifest.Entries.ForContentType<IWorkbook>(), e => Assert.Equal(MigrationManifestEntryStatus.Migrated, e.Status));
            }
        }
    }
}
