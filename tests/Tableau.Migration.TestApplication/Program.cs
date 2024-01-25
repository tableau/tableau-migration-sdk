using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Tableau.Migration.TestApplication.Config;
using Tableau.Migration.TestComponents;
using Tableau.Migration.TestComponents.Hooks.Mappings;

namespace Tableau.Migration.TestApplication
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
                        .Configure<TestApplicationOptions>(ctx.Configuration)
                        .Configure<TestTableauCloudUsernameOptions>(ctx.Configuration.GetSection("tableau:migrationOptions"))
                        .AddTableauMigrationSdk(ctx.Configuration.GetSection("tableau:migrationSdk"))
                        .AddTestComponents()
                        .AddHostedService<TestApplication>()
                        .AddLogging(config =>
                        {
                            config.ClearProviders();
                            config.AddSerilog(
                                new LoggerConfiguration()
                                        .ReadFrom.Configuration(ctx.Configuration)
                                        .CreateLogger());
                        });
                })
                .Build();

            await host.RunAsync();
        }
    }
}