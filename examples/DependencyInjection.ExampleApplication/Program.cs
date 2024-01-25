using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DependencyInjection.ExampleApplication.Hooks.Filters;
using DependencyInjection.ExampleApplication.Hooks.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration;

namespace DependencyInjection.ExampleApplication
{
    public static class Program
    {
        // Update the values here to filter the service output.
        private static readonly IEnumerable<Type>? DisplayFilter = null; // new[] { typeof(ProjectsFilter), typeof(ProjectMapping) };

        public static async Task Main()
        {
            // Initialize a new service collection
            var serviceCollection = new ServiceCollection()

                // Register the default migration-related services.
                .AddTableauMigrationSdk()

                // Set up logging to redirect to the console.
                .AddConsoleLogging()

                // Register a singleton service.
                .AddSingleton<SingletonService>()

                // Register a scoped filter.
                .AddScoped<ProjectsFilter>()

                /// Register a scoped mapping.
                .AddScoped<ProjectMapping>()

                // Display the services in the collection.
                .DisplayServices(DisplayFilter); 

            // Build the service provider (container) instance to manage services.
            // Dependencies will be retrieved from this instance.
            await using var serviceProvider = serviceCollection.BuildServiceProvider();

            // Verify the services are configured correctly.
            await VerifyServicesAsync(serviceProvider);
        }

        private static async Task VerifyServicesAsync(IServiceProvider serviceProvider)
        {
            // Create a scope to access our scoped services.
            await using var scope1 = serviceProvider.CreateAsyncScope();

            // Retrieve some services from our scoped provider...
            var scope1Singleton = scope1.ServiceProvider.GetRequiredService<SingletonService>();

            var scope1Filter1 = scope1.ServiceProvider.GetRequiredService<ProjectsFilter>();
            var scope1Mapping1 = scope1.ServiceProvider.GetRequiredService<ProjectMapping>();

            var scope1Filter2 = scope1.ServiceProvider.GetRequiredService<ProjectsFilter>();
            var scope1Mapping2 = scope1.ServiceProvider.GetRequiredService<ProjectMapping>();

            // Create another scope to access our scoped services.
            await using var scope2 = serviceProvider.CreateAsyncScope();

            // Retrieve some services from our scoped provider...
            var scope2Singleton = scope1.ServiceProvider.GetRequiredService<SingletonService>();

            var scope2Filter = scope2.ServiceProvider.GetRequiredService<ProjectsFilter>();
            var scope2Mapping = scope2.ServiceProvider.GetRequiredService<ProjectMapping>();

            // Because the SingletonService was registered as a singleton,
            // requests across scopes will return the same instance.
            Assert.SameReferences(scope1Singleton, scope2Singleton);

            // Because we registered these services as scoped, other requests for the service
            // within the same scope will return the initial instance.
            Assert.SameReferences(scope1Filter1, scope1Filter2);
            Assert.SameReferences(scope1Mapping1, scope1Mapping2);

            // Because we are accessing services from a different scope, the instances
            // returned here will be different from the first scope.
            Assert.DifferentReferences(scope1Filter1, scope2Filter);
            Assert.DifferentReferences(scope1Mapping1, scope2Mapping);
        }
    }
}
