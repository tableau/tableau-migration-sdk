using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class GroupMigrationTests
    {
        public class ServerToCloud : ServerToCloudSimulationTestBase
        {
            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                return services.AddTableauMigrationSdk();
            }

            [Fact]
            public async Task MigratesAllGroupsToCloudAsync()
            {
                //Arrange - create source users to migrate.
                var (nonSupportUsers, supportUsers) = PrepareSourceUsersData();
                var groups = PrepareSourceGroupsData();

                //Migrate
                var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .FromSource(SourceEndpointConfig)
                    .ToDestination(CloudDestinationEndpointConfig)
                    .ForServerToCloud()
                    .Build();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();
                var result = await migrator.ExecuteAsync(plan, Cancel);
                // Wait import all groups
                await Task.Delay(500);

                //Assert - all users should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(groups.Count,
                    result.Manifest.Entries.ForContentType<IGroup>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());
                Assert.Single(result.Manifest.Entries.ForContentType<IGroup>().Where(e => e.Status == MigrationManifestEntryStatus.Skipped));

                void AssertGroupMigrated(GroupsResponse.GroupType sourceGroup)
                {
                    var destinationGroup = Assert.Single(
                        CloudDestinationApi.Data.Groups.Where(
                            g => g.Name == sourceGroup.Name));

                    Assert.NotEqual(sourceGroup.Id, destinationGroup.Id);
                    Assert.Equal(sourceGroup.Name, destinationGroup.Name);
                    Assert.Equal(sourceGroup.Domain?.Name, destinationGroup.Domain?.Name);
                    Assert.Equal(sourceGroup.Import?.SiteRole, destinationGroup.Import?.SiteRole);
                }

                Assert.All(SourceApi.Data.Groups.Where(g => g.Name != "All Users"), AssertGroupMigrated);
            }
        }
    }
}
