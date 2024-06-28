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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class WorkbookMigrationTests
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
            public async Task MigratesAllWorkbooksToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();

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

                //Assert - all workbooks should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(CloudDestinationApi.Data.Workbooks.Count,
                    result.Manifest.Entries.ForContentType<IWorkbook>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());

                Assert.All(sourceWorkbooks, AssertWorkbookMigrated);

                void AssertWorkbookMigrated(WorkbookResponse.WorkbookType sourceWorkbook)
                {
                    // Get destination workbook
                    var destinationWorkbook = Assert.Single(
                         CloudDestinationApi.Data.Workbooks.Where(ds =>
                             ds.Name == sourceWorkbook.Name
                         ));

                    Assert.NotEqual(sourceWorkbook.Id, destinationWorkbook.Id);
                    Assert.Equal(sourceWorkbook.Name, destinationWorkbook.Name);

                    // Assert workbook permissions
                    AssertPermissionsMigrated(result.Manifest,
                            SourceApi.Data.WorkbookPermissions[sourceWorkbook.Id],
                            CloudDestinationApi.Data.WorkbookPermissions[destinationWorkbook.Id]);

                    // Assert workbook owner
                    Assert.NotNull(destinationWorkbook.Owner);
                    Assert.NotEqual(destinationWorkbook.Owner.Id, Guid.Empty);
                    Assert.NotEqual(destinationWorkbook.Owner.Id, sourceWorkbook.Owner?.Id);

                    // Assert tags
                    sourceWorkbook.Tags.AssertEqual(destinationWorkbook.Tags);

                    // Assert views
                    Assert.All(sourceWorkbook.Views, AssertWorkbookViewMigrated);

                    // Assert connection
                    AssertWorkbookConnectionsMigrated(sourceWorkbook, destinationWorkbook);

                    void AssertWorkbookViewMigrated(WorkbookResponse.WorkbookType.ViewReferenceType sourceView)
                    {
                        // Get destination view
                        var destinationView = Assert.Single(
                                destinationWorkbook!.Views.Where(v =>
                                {
                                    return IViewReferenceTypeComparer.Instance.Equals(sourceView, v);
                                }));

                        Assert.NotEqual(sourceView.Id, destinationView.Id);

                        // Assert view permissions
                        AssertPermissionsMigrated(result.Manifest,
                            SourceApi.Data.ViewPermissions[sourceView.Id],
                            CloudDestinationApi.Data.ViewPermissions[destinationView.Id]);

                        // Assert view tags
                        sourceView.Tags.AssertEqual(destinationView.Tags);

                        // No need to verify owner as it's not migratable. View owner is the same as workbook owner.
                    }

                    void AssertWorkbookConnectionsMigrated(WorkbookResponse.WorkbookType sourceWorkbook, WorkbookResponse.WorkbookType destinationWorkbook)
                    {
                        // Get source connections
                        var sourceWorkbookFile = SourceApi.Data.WorkbookFiles[sourceWorkbook!.Id];
                        Assert.NotNull(sourceWorkbookFile);

                        var sourceSimulatedWorkbook = Constants.DefaultEncoding
                                .GetString(sourceWorkbookFile)
                                .FromXml<SimulatedWorkbookData>();

                        Assert.NotNull(sourceSimulatedWorkbook);

                        // Get destination connections
                        var destinationWorkbookFile = CloudDestinationApi.Data.WorkbookFiles[destinationWorkbook!.Id];
                        Assert.NotNull(destinationWorkbookFile);

                        var destinationSimulatedWorkbook = Constants.DefaultEncoding
                                .GetString(destinationWorkbookFile)
                                .FromXml<SimulatedWorkbookData>();

                        Assert.NotNull(destinationSimulatedWorkbook);

                        // Assert
                        AssertConnections(sourceSimulatedWorkbook.Connections, destinationSimulatedWorkbook.Connections);

                        void AssertConnections(
                            List<SimulatedConnection>? sourceConnections,
                            List<SimulatedConnection>? destinationConnections)
                        {
                            if (sourceConnections is null)
                            {
                                Assert.NotNull(destinationConnections);
                                Assert.Empty(destinationConnections);
                                return;
                            }

                            Assert.Equal<SimulatedConnection>(sourceConnections, destinationConnections, SimulatedConnectionComparer.Instance);
                        }
                    }
                }
            }
        }
    }
}
