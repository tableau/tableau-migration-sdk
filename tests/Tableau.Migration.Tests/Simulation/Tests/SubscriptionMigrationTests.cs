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
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public sealed class SubscriptionMigrationTests
    {
        public sealed class ServerToCloud : ServerToCloudSimulationTestBase
        {
            [Fact]
            public async Task MigratesAllSubscriptionsToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();
                var sourceSubscriptions = PrepareSourceSubscriptionsData();

                //Migrate
                var result = await RunMigrationWithTableauIdAuthAsync();

                //Assert - all subscriptions should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(CloudDestinationApi.Data.CloudSubscriptions.Count,
                    result.Manifest.Entries.ForContentType<IServerSubscription>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());

                Assert.All(sourceSubscriptions, AssertSubscriptionMigrated);

                void AssertSubscriptionMigrated(GetSubscriptionsResponse.SubscriptionType sourceSubscription)
                {
                    // Get source references
                    var sourceSchedule = SourceApi.Data.Schedules.Single(s => s.Id == sourceSubscription.Schedule!.Id);
                    Assert.NotNull(sourceSubscription.Content);

                    var sourceUser = SourceApi.Data.Users.Single(u => u.Id == sourceSubscription.User!.Id);
                    var mappedUserId = result.Manifest.Entries.ForContentType<IUser>().Single(e => e.Source.Id == sourceUser.Id).Destination?.Id;

                    var contentType = sourceSubscription.Content.Type ?? string.Empty;

                    var sourceWorkbook = contentType != "workbook" ? null : SourceApi.Data.Workbooks.Single(w => w.Id == sourceSubscription.Content.Id);
                    var destinationWorkbook = sourceWorkbook is null ? null : CloudDestinationApi.Data.Workbooks.Single(w => string.Equals(w.Name, sourceWorkbook.Name, StringComparison.OrdinalIgnoreCase));

                    var sourceView = contentType != "view" ? null : SourceApi.Data.Views.Single(w => w.Id == sourceSubscription.Content.Id);
                    var destinationView = sourceView is null ? null : CloudDestinationApi.Data.Views.Single(w => string.Equals(w.Name, sourceView.Name, StringComparison.OrdinalIgnoreCase));

                    // Get destination subscription
                    var destinationSubscription = Assert.Single(CloudDestinationApi.Data.CloudSubscriptions, sub =>
                        sub.Content?.Type == contentType &&
                        (
                            (contentType == "workbook" && sub.Content?.Id == destinationWorkbook?.Id) ||
                            (contentType == "view" && sub.Content?.Id == destinationView?.Id)
                        ));

                    Assert.NotEqual(sourceSubscription.Id, destinationSubscription.Id);

                    Assert.Equal(sourceSubscription.AttachImage, destinationSubscription.AttachImage);
                    Assert.Equal(sourceSubscription.AttachPdf, destinationSubscription.AttachPdf);
                    Assert.Equal(sourceSubscription.Message, destinationSubscription.Message);
                    Assert.Equal(sourceSubscription.PageOrientation, destinationSubscription.PageOrientation);
                    Assert.Equal(sourceSubscription.PageSizeOption, destinationSubscription.PageSizeOption);
                    Assert.Equal(sourceSubscription.Subject, destinationSubscription.Subject);

                    Assert.Equal(mappedUserId, destinationSubscription.User?.Id);

                    AssertScheduleMigrated(sourceSchedule, destinationSubscription.Schedule);
                }
            }
        }
    }
}
