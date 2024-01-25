using Microsoft.Extensions.DependencyInjection;

namespace Tableau.Migration.Tests.Unit
{
    public class IServiceCollectionExtensionsTests : IServiceCollectionExtensionsTestBase
    {
        protected override void ConfigureServices(IServiceCollection services) => services.AddTableauMigrationSdk();
    }
}
