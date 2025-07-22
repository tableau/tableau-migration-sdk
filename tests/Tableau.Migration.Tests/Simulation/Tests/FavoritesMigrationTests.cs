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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class FavoritesMigrationTests
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
            public async Task MigratesAllFavoritesToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();
                var sourceWorkbooks = PrepareSourceWorkbooksData();
                var sourceDataSources = PrepareSourceDataSourceData();
                var sourceCustomViews = PrepareSourceCustomViewsData();
                var userFavorites = PrepareSourceFavoritesData();


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

                //Assert - all favorites should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);
                var manifestEntries = result.Manifest.Entries.ForContentType<IFavorite>().ToList();

                var migratedEntries = manifestEntries.Where(e => e.Status == MigrationManifestEntryStatus.Migrated).ToList();
                var nonMigratedEntries = manifestEntries.Where(e => e.Status != MigrationManifestEntryStatus.Migrated).ToList();

                Assert.Empty(nonMigratedEntries);

                Assert.Equal(
                    CloudDestinationApi.Data.UserFavorites
                    .Select(uf => uf.Value)
                    .SelectMany(uf => uf.Keys.Select(k => k.Id)).Count(),
                    migratedEntries.Count);

                AssertFavoritesMigrated(userFavorites, result);
            }

            private void AssertFavoritesMigrated(
                ImmutableDictionary<Guid, ConcurrentDictionary<(FavoriteContentType, Guid), string>> sourceFavorites,
                MigrationResult result)
            {
                foreach (var (sourceUserId, sourceUserFavorites) in sourceFavorites)
                {
                    var destinationUserId = MapReference<IUser>(result.Manifest, sourceUserId)?.Id;

                    Assert.NotNull(destinationUserId);

                    CloudDestinationApi.Data.UserFavorites.TryGetValue(destinationUserId.Value, out var destinationUserFavorites);

                    if (sourceUserFavorites.IsNullOrEmpty())
                    {
                        return;
                    }

                    Assert.NotNull(destinationUserFavorites);
                    Assert.False(destinationUserFavorites.IsEmpty);

                    foreach (var ((contentType, sourceContentId), label) in sourceUserFavorites)
                    {
                        var destinationContentId = contentType switch
                        {
                            FavoriteContentType.Project => MapReference<IProject>(result.Manifest, sourceContentId)?.Id,
                            FavoriteContentType.DataSource => MapReference<IDataSource>(result.Manifest, sourceContentId)?.Id,
                            FavoriteContentType.Workbook => MapReference<IWorkbook>(result.Manifest, sourceContentId)?.Id,
                            FavoriteContentType.View => MapReference<IView>(result.Manifest, sourceContentId)?.Id,
                            FavoriteContentType.Flow => MapReference<IFlow>(result.Manifest, sourceContentId)?.Id,
                            _ => throw new ArgumentException($"{contentType} is not supported for Favorites.")
                        };

                        Assert.NotNull(destinationContentId);

                        Assert.True(destinationUserFavorites.ContainsKey((contentType, destinationContentId.Value)));
                        Assert.Equal(label, destinationUserFavorites[(contentType, destinationContentId.Value)]);
                    }
                }
            }
        }
    }
}