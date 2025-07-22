//
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
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class ExtractRefreshTaskMigrationTests
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
            public async Task MigratesAllExtractRefreshTasksToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceDataSources = PrepareSourceDataSourceData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();
                var sourceExtractRefreshTasks = PrepareSourceExtractRefreshTasksData();

                //Migrate
                var result = await RunMigrationWithTableauIdAuthAsync();

                //Assert - all extract refresh tasks should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(CloudDestinationApi.Data.CloudExtractRefreshTasks.Count,
                    result.Manifest.Entries.ForContentType<IServerExtractRefreshTask>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());

                Assert.All(sourceExtractRefreshTasks, AssertExtractRefreshTasksMigrated);

                void AssertExtractRefreshTasksMigrated(ExtractRefreshTasksResponse.TaskType sourceExtractRefreshTask)
                {
                    // Get source references
                    var sourceExtractRefresh = sourceExtractRefreshTask.ExtractRefresh!;
                    var sourceSchedule = SourceApi.Data.Schedules.First(sch => sch.Id == sourceExtractRefresh.Schedule!.Id);
                    var extractRefreshType = SourceApi.Data.ScheduleExtractRefreshTasks.First(t => t.Id == sourceExtractRefresh.Id);
                    var sourceDataSource = SourceApi.Data.DataSources.FirstOrDefault(ds =>
                        sourceExtractRefresh.DataSource is not null &&
                        ds.Id == sourceExtractRefresh.DataSource.Id);
                    var sourceWorkbook = SourceApi.Data.Workbooks.FirstOrDefault(wb =>
                        sourceExtractRefresh.Workbook is not null &&
                        wb.Id == sourceExtractRefresh.Workbook.Id);
                    // Get destination references
                    var destinationDataSource = CloudDestinationApi.Data.DataSources.FirstOrDefault(ds =>
                        sourceDataSource is not null &&
                        ds.Name == sourceDataSource.Name);
                    var destinationWorkbook = CloudDestinationApi.Data.Workbooks.FirstOrDefault(wb =>
                        sourceWorkbook is not null &&
                        wb.Name == sourceWorkbook.Name);

                    // Get destination extract refresh task
                    var destinationExtractRefreshTask = Assert.Single(CloudDestinationApi.Data.CloudExtractRefreshTasks, cert =>
                            (
                                cert.ExtractRefresh!.DataSource is not null &&
                                destinationDataSource is not null &&
                                cert.ExtractRefresh.DataSource.Id == destinationDataSource.Id
                            ) ||
                            (
                                cert.ExtractRefresh!.Workbook is not null &&
                                destinationWorkbook is not null &&
                                cert.ExtractRefresh.Workbook.Id == destinationWorkbook.Id
                            ));
                    var destinationExtractRefresh = destinationExtractRefreshTask.ExtractRefresh!;

                    Assert.NotEqual(sourceExtractRefresh.Id, destinationExtractRefresh.Id);
                    if (extractRefreshType.Type == ExtractRefreshType.ServerIncrementalRefresh)
                    {
                        Assert.Equal(ExtractRefreshType.CloudIncrementalRefresh, destinationExtractRefresh.Type);
                    }
                    else
                    {
                        Assert.Equal(extractRefreshType.Type, destinationExtractRefresh.Type);
                    }

                    AssertScheduleMigrated(sourceSchedule, destinationExtractRefresh.Schedule);
                }
            }
        }
    }
}
