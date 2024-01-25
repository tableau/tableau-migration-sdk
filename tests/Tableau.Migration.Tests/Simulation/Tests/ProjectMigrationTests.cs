using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class ProjectMigrationTests
    {
        public class ServerToCloud : ServerToCloudSimulationTestBase
        {
            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                return services.AddTableauMigrationSdk();
            }

            [Fact]
            public async Task MigratesAllProjectsToCloudAsync()
            {
                //Arrange - create source content to migrate.
                var (NonSupportUsers, SupportUsers) = PrepareSourceUsersData(5);
                var groups = PrepareSourceGroupsData(5);
                var sourceProjects = PrepareSourceProjectsData();

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

                //Assert - all projects should be migrated.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                Assert.Equal(CloudDestinationApi.Data.Projects.Count,
                    result.Manifest.Entries.ForContentType<IProject>().Where(e => e.Status == MigrationManifestEntryStatus.Migrated).Count());

                Assert.All(sourceProjects, AssertProjectMigrated);

                void AssertProjectMigrated(ProjectsResponse.ProjectType sourceProject)
                {
                    var destinationProject = Assert.Single(
                        CloudDestinationApi.Data.Projects.Where(p =>
                            p.Name == sourceProject.Name &&
                            p.ParentProjectId is null == sourceProject.ParentProjectId is null));

                    Assert.NotEqual(sourceProject.Id, destinationProject.Id);
                    Assert.Equal(sourceProject.Name, destinationProject.Name);

                    if (sourceProject.ParentProjectId is not null)
                    {
                        Assert.NotNull(destinationProject.ParentProjectId);
                        Assert.NotEqual(sourceProject.ParentProjectId, destinationProject.ParentProjectId);
                    }
                    else
                    {
                        Assert.Null(destinationProject.ParentProjectId);
                    }

                    foreach (var contentType in DefaultPermissionsContentTypeUrlSegments.GetAll())
                    {
                        AssertPermissionsMigrated(result.Manifest,
                            SourceApi.Data.DefaultProjectPermissions[sourceProject.Id][contentType],
                            CloudDestinationApi.Data.DefaultProjectPermissions[destinationProject.Id][contentType]);
                    }

                    AssertPermissionsMigrated(result.Manifest,
                        SourceApi.Data.ProjectPermissions[sourceProject.Id],
                        CloudDestinationApi.Data.ProjectPermissions[destinationProject.Id]);
                }
            }
        }
    }
}
