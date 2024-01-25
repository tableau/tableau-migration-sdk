using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Engine.Endpoints;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class TableauApiEndpointTestBase<TEndpoint> : AutoFixtureTestBase, IDisposable
        where TEndpoint : TableauApiEndpointBase
    {
        protected readonly Mock<IApiClient> MockServerApi;
        protected readonly Mock<ISitesApiClient> MockSiteApi;

        protected readonly ServiceProvider MigrationServices;

        public TableauApiEndpointTestBase()
        {
            MockSiteApi = Create<Mock<ISitesApiClient>>();
            MockServerApi = Freeze<Mock<IApiClient>>();
            MockServerApi.Setup(x => x.SignInAsync(Cancel))
                .ReturnsAsync(() => AsyncDisposableResult<ISitesApiClient>.Succeeded(MockSiteApi.Object));

            var migrationServiceCollection = new ServiceCollection()
                .AddMigrationApiClient();
            migrationServiceCollection.AddTransient(p => MockServerApi.Object);

            MigrationServices = migrationServiceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            MigrationServices.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
