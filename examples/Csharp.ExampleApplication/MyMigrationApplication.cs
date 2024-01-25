using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyMigrationApplication.Config;
using MyMigrationApplication.Hooks.Filters;
using MyMigrationApplication.Hooks.Mappings;
using Tableau.Migration;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Pipelines;

#region namespace

namespace MyMigrationApplication
{
    internal sealed class MyMigrationApplication : IHostedService
    {
        private readonly Stopwatch _timer;
        private readonly IHostApplicationLifetime _appLifetime;
        private IMigrationPlanBuilder _planBuilder;
        private readonly IMigrator _migrator;
        private readonly MyMigrationApplicationOptions _options;
        private readonly ILogger<MyMigrationApplication> _logger;

        public MyMigrationApplication(
            IHostApplicationLifetime appLifetime,
            IMigrationPlanBuilder planBuilder,
            IMigrator migrator,
            IOptions<MyMigrationApplicationOptions> options,
            ILogger<MyMigrationApplication> logger)
        {
            _timer = new Stopwatch();

            _appLifetime = appLifetime;

            // You can choose to assign an instance of the ServerToCloudMigrationPlanBuilder to help you 
            // add your own filters, mappings, transformers or hooks.
            // Refer to the Articles section of this documentation for more details.
            _planBuilder = planBuilder;
            _migrator = migrator;
            _options = options.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancel)
        {
            var startTime = DateTime.UtcNow;
            _timer.Start();

            #region EmailDomainMapping-Registration
            // Use the methods on your plan builder to add configuration and make customizations.
            _planBuilder = _planBuilder
                .FromSourceTableauServer(_options.Source.ServerUrl, _options.Source.SiteContentUrl, _options.Source.AccessTokenName, Environment.GetEnvironmentVariable("TABLEAU_MIGRATION_SOURCE_TOKEN") ?? string.Empty)
                .ToDestinationTableauCloud(_options.Destination.ServerUrl, _options.Destination.SiteContentUrl, _options.Destination.AccessTokenName, Environment.GetEnvironmentVariable("TABLEAU_MIGRATION_DESTINATION_TOKEN") ?? string.Empty)
                .ForServerToCloud()
                .WithTableauIdAuthenticationType()
                // You can add authentication type mappings here            
                .WithTableauCloudUsernames<EmailDomainMapping>();
            #endregion

            var validationResult = _planBuilder.Validate();

            if (!validationResult.Success)
            {
                _logger.LogError("Migration plan validation failed. {Errors}", validationResult.Errors);
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                _appLifetime.StopApplication();
            }

            // Add filters
            #region DefaultProjectsFilter-Registration
            _planBuilder.Filters.Add<DefaultProjectsFilter, IProject>();
            #endregion

            #region UnlicensedUsersFilter-Registration
            _planBuilder.Filters.Add<UnlicensedUsersFilter, IUser>();
            #endregion


            // Build the plan
            var plan = _planBuilder.Build();

            // Execute the migration
            var result = await _migrator.ExecuteAsync(plan, cancel);

            _timer.Stop();

            PrintResult(result);

            _logger.LogInformation($"Migration Started: {startTime}");
            _logger.LogInformation($"Migration Finished: {DateTime.UtcNow}");
            _logger.LogInformation($"Elapsed: {_timer.Elapsed}");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            _appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancel) => Task.CompletedTask;

        /// <summary>
        /// Prints the result to console. 
        /// You can replace this with a logging based method of your choice.
        /// </summary>
        /// <param name="result">The migration result.</param>
        private void PrintResult(MigrationResult result)
        {
            _logger.LogInformation($"Result: {result.Status}");

            // Logging any errors from the manifest.
            if (result.Manifest.Errors.Any())
            {
                _logger.LogError("## Errors detected! ##");
                foreach (var error in result.Manifest.Errors)
                {
                    _logger.LogError(error, "Processing Error.");
                }
            }

            foreach (var type in ServerToCloudMigrationPipeline.ContentTypes)
            {
                var contentType = type.ContentType;

                _logger.LogInformation($"## {contentType.Name} ##");

                // Manifest entries can be grouped based on content type.
                foreach (var entry in result.Manifest.Entries.ForContentType(contentType))
                {
                    _logger.LogInformation($"{contentType.Name} {entry.Source.Location} Migration Status: {entry.Status}");

                    if (entry.Errors.Any())
                    {
                        _logger.LogError($"## {contentType.Name} Errors detected! ##");
                        foreach (var error in entry.Errors)
                        {
                            _logger.LogError(error, "Processing Error.");
                        }
                    }

                    if (entry.Destination is not null)
                    {
                        _logger.LogInformation($"{contentType.Name} {entry.Source.Location} migrated to {entry.Destination.Location}");
                    }
                }
            }
        }
    }
}

#endregion