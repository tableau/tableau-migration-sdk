using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.TestComponents.Engine.Manifest;
using Tableau.Migration.TestComponents.Hooks.Mappings;

namespace Tableau.Migration.TestComponents
{
    /// <summary>
    /// Static class containing extension methods for <see cref="IServiceCollection"/> objects.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services required for using the Tableau Migration SDK Test Components.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>        
        /// <returns>The same service collection as the <paramref name="services"/> parameter.</returns>
        public static IServiceCollection AddTestComponents(this IServiceCollection services)
        {
            services.AddScoped<TestTableauCloudUsernameMapping>();
            services.AddSingleton<MigrationManifestSerializer>();
            return services;
        }
    }
}
