using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.TestApplication.Config;
using Tableau.Migration.TestComponents.Engine.Manifest;
using Tableau.Migration.TestComponents.Hooks.Mappings;

namespace Tableau.Migration.TestApplication
{
    internal sealed class TestApplication : IHostedService
    {
        private readonly Stopwatch _timer;
        private readonly IHostApplicationLifetime _appLifetime;
        private IMigrationPlanBuilder _planBuilder;
        private readonly IMigrator _migrator;
        private readonly TestApplicationOptions _options;
        private readonly ILogger<TestApplication> _logger;
        private readonly MigrationManifestSerializer _manifestSerializer;

        public TestApplication(
            IHostApplicationLifetime appLifetime,
            IMigrationPlanBuilder planBuilder,
            IMigrator migrator,
            IOptions<TestApplicationOptions> options,
            ILogger<TestApplication> logger,
            MigrationManifestSerializer manifestSerializer)
        {
            _timer = new Stopwatch();

            _appLifetime = appLifetime;
            _planBuilder = planBuilder;
            _migrator = migrator;
            _options = options.Value;
            _logger = logger;
            _manifestSerializer = manifestSerializer;
        }

        public async Task StartAsync(CancellationToken cancel)
        {
            var manifestFilepath = @"C:\temp\Manifest.json";

            var startTime = DateTime.UtcNow;
            _timer.Start();

            _planBuilder = _planBuilder
                .FromSourceTableauServer(_options.Source.ServerUrl, _options.Source.SiteContentUrl, _options.Source.AccessTokenName, Environment.GetEnvironmentVariable("TABLEAU_MIGRATION_SOURCE_TOKEN") ?? string.Empty)
                .ToDestinationTableauCloud(_options.Destination.ServerUrl, _options.Destination.SiteContentUrl, _options.Destination.AccessTokenName, Environment.GetEnvironmentVariable("TABLEAU_MIGRATION_DESTINATION_TOKEN") ?? string.Empty)
                .ForServerToCloud()
                .WithTableauIdAuthenticationType()
                .WithTableauCloudUsernames<TestTableauCloudUsernameMapping>();

            var plan = _planBuilder.Build();

            var manifest = await _manifestSerializer.LoadAsync(manifestFilepath, cancel);
            if (manifest is not null)
            {
                ConsoleKey key;
                do
                {
                    Console.Write($"Existing Manifest found at {manifestFilepath}. Should it be used? [Y/n] ");
                    key = Console.ReadKey().Key;
                    Console.WriteLine(); // make Console logs prettier
                } while (key is not ConsoleKey.Enter && key is not ConsoleKey.Y && key is not ConsoleKey.N);

                if (key is ConsoleKey.N)
                    manifest = null;
            }

            var result = await _migrator.ExecuteAsync(plan, manifest, cancel);

            _timer.Stop();

            PrintResult(result);

            await _manifestSerializer.SaveAsync(result.Manifest, manifestFilepath, cancel);

            _logger.LogInformation($"Migration Started: {startTime}");
            _logger.LogInformation($"Migration Finished: {DateTime.UtcNow}");
            _logger.LogInformation($"Elapsed: {_timer.Elapsed}");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            _appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancel) => Task.CompletedTask;

        private void PrintResult(MigrationResult result)
        {
            _logger.LogInformation($"Result: {result.Status}");

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
