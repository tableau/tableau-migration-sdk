using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyMigrationApplication.Config;
using MyMigrationApplication.Hooks.Filters;
using MyMigrationApplication.Hooks.Mappings;
using Tableau.Migration;

#region namespace
namespace MyMigrationApplication
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Set the DOTNET_ENVIRONMENT environment variable to the name of the environment.
            // This loads the appsettings.<DOTNET_ENVIRONMENT>.json config file.
            // If no DOTNET_ENVIRONMENT is set, appsettings.json will be used
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    services
                        .Configure<MyMigrationApplicationOptions>(ctx.Configuration)
                        .Configure<EmailDomainMappingOptions>(ctx.Configuration.GetSection("tableau:migrationOptions"))
                        .AddTableauMigrationSdk(ctx.Configuration.GetSection("tableau:migrationSdk"))
                        .AddCustomizations()
                        .AddHostedService<MyMigrationApplication>();
                })
                .Build();

            await host.RunAsync();
        }

        /// <summary>
        /// Registers services required for using the Tableau Migration SDK customizations.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>        
        /// <returns>The same service collection as the <paramref name="services"/> parameter.</returns>
        public static IServiceCollection AddCustomizations(this IServiceCollection services)
        {
            #region EmailDomainMapping-DI
            services.AddScoped<EmailDomainMapping>();
            #endregion

            #region DefaultProjectsFilter-DI
            services.AddScoped<DefaultProjectsFilter>();
            #endregion

            #region UnlicensedUsersFilter-DI
            services.AddScoped<UnlicensedUsersFilter>();
            #endregion

            return services;
        }
    }
}
#endregion