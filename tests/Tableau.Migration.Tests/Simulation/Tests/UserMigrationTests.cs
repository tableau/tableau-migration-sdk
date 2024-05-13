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

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class UserMigrationTests
    {
        public class Batch : ServerToCloud
        {
        }

        public class Individual : ServerToCloud
        {
            protected override bool UsersBatchImportEnabled => false;
        }

        public abstract class ServerToCloud : ServerToCloudSimulationTestBase
        {
            [Fact]
            public async Task MigratesAllUsersToCloudAsync()
            {
                //Arrange - create source users to migrate.
                var (nonSupportUsers, supportUsers) = PrepareSourceUsersData();

                //Migrate
                var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .FromSource(SourceEndpointConfig)
                    .ToDestination(CloudDestinationEndpointConfig)
                    .ForServerToCloud()
                    .Build();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();
                var result = await migrator.ExecuteAsync(plan, Cancel);

                //Assert - all users should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(nonSupportUsers.Count,
                    result.Manifest.Entries.ForContentType<IUser>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count() - 1); // The source has a default user, hence the -1

                Assert.Equal(supportUsers.Count,
                    result.Manifest.Entries.ForContentType<IUser>().Where(e => e.Status == MigrationManifestEntryStatus.Skipped).Count());


                void AssertUserMigrated(UsersResponse.UserType sourceUser)
                {
                    var destinationUser = Assert.Single(
                        CloudDestinationApi.Data.Users.Where(
                            u => u.Domain?.Name == sourceUser.Domain?.Name
                            && u.Name == sourceUser.Name));

                    Assert.NotEqual(sourceUser.Id, destinationUser.Id);
                    Assert.Equal(sourceUser.Domain?.Name, destinationUser.Domain?.Name);
                    Assert.Equal(sourceUser.Name, destinationUser.Name);
                    Assert.Null(destinationUser.Email);

                    if (sourceUser.SiteRole == SiteRoles.Viewer ||
                        sourceUser.SiteRole == SiteRoles.Guest ||
                        sourceUser.SiteRole == SiteRoles.SupportUser)
                    {
                        Assert.Equal(SiteRoles.Viewer, destinationUser.SiteRole);
                    }
                    else if (sourceUser.SiteRole == SiteRoles.ServerAdministrator)
                    {
                        Assert.Equal(SiteRoles.SiteAdministratorCreator, destinationUser.SiteRole);
                    }
                    else
                    {
                        Assert.Equal(sourceUser.SiteRole, destinationUser.SiteRole);
                    }
                    Assert.Null(destinationUser.FullName);
                }

                Assert.All(SourceApi.Data.Users.Where(u => u.SiteRole != SiteRoles.SupportUser), AssertUserMigrated);
            }
        }
    }
}
