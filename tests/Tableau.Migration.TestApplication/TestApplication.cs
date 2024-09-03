//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.TestApplication.Config;
using Tableau.Migration.TestApplication.Hooks;

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
            var manifestFilePath = LogFileHelper.GetManifestFilePath(_options.Log);

            Console.WriteLine("Starting app");
            _logger.LogInformation("Starting app log");

            _planBuilder = _planBuilder
                .FromSourceTableauServer(_options.Source.ServerUrl, _options.Source.SiteContentUrl, _options.Source.AccessTokenName, Environment.GetEnvironmentVariable("TABLEAU_MIGRATION_SOURCE_TOKEN") ?? _options.Source.AccessToken)
                .ToDestinationTableauCloud(_options.Destination.ServerUrl, _options.Destination.SiteContentUrl, _options.Destination.AccessTokenName, Environment.GetEnvironmentVariable("TABLEAU_MIGRATION_DESTINATION_TOKEN") ?? _options.Destination.AccessToken)
                .ForServerToCloud();


            if (_options.Destination.SiteContentUrl != "")
            { // Most likely means it's a Cloud site not a Server
                _planBuilder = ((IServerToCloudMigrationPlanBuilder)_planBuilder)
                    .WithTableauIdAuthenticationType()
                    .WithTableauCloudUsernames<TestTableauCloudUsernameMapping>();

                _planBuilder.Filters.Add<UnlicensedUserFilter, IUser>();
            }
            else
            { // Most likely means it's a Server
                _planBuilder.Filters.Add<NonDomainUserFilter, IUser>();
            }

            // A user has non-ASCII names in their username, which causes issues for now. 
            // Filtering to make it past the issue.
            _planBuilder.Filters.Add<SpecialUserFilter, IUser>();
            _planBuilder.Mappings.Add<SpecialUserMapping, IUser>();

            // Map unlicensed users to single admin
            _planBuilder.Mappings.Add<UnlicensedUserMapping, IUser>();

            // Save manifest every every batch of every content type
            _planBuilder.Hooks.Add<SaveManifestAfterBatchMigrationCompletedHook<IUser>>();
            _planBuilder.Hooks.Add<SaveManifestAfterBatchMigrationCompletedHook<IGroup>>();
            _planBuilder.Hooks.Add<SaveManifestAfterBatchMigrationCompletedHook<IProject>>();
            _planBuilder.Hooks.Add<SaveManifestAfterBatchMigrationCompletedHook<IDataSource>>();
            _planBuilder.Hooks.Add<SaveManifestAfterBatchMigrationCompletedHook<IWorkbook>>();
            _planBuilder.Hooks.Add<SaveManifestAfterBatchMigrationCompletedHook<ICloudExtractRefreshTask>>();

            // Log when a content type is done
            _planBuilder.Hooks.Add<TimeLoggerAfterActionHook>();

            _planBuilder.Filters.Add<SkipByParentLocationFilter<IProject>, IProject>();
            _planBuilder.Filters.Add<SkipByParentLocationFilter<IDataSource>, IDataSource>();
            _planBuilder.Filters.Add<SkipByParentLocationFilter<IWorkbook>, IWorkbook>();
            _planBuilder.Transformers.Add<RemoveMissingDestinationUsersFromGroupsTransformer, IPublishableGroup>();
            _planBuilder.Mappings.Add<ContentWithinSkippedLocationMapping<IProject>, IProject>();
            _planBuilder.Mappings.Add<ContentWithinSkippedLocationMapping<IDataSource>, IDataSource>();
            _planBuilder.Mappings.Add<ContentWithinSkippedLocationMapping<IWorkbook>, IWorkbook>();
            // Skip content types we've already done. 
            // Uncomment as needed
            //_planBuilder.Filters.Add(new SkipFilter<IUser>());
            //_planBuilder.Filters.Add(new SkipFilter<IGroup>());
            //_planBuilder.Filters.Add(new SkipFilter<IProject>());
            //_planBuilder.Filters.Add(new SkipFilter<IDataSource>());
            //_planBuilder.Filters.Add(new SkipFilter<IWorkbook>());
            //_planBuilder.Filters.Add(new SkipFilter<IServerExtractRefreshTask>());

            var prevManifest = await LoadManifest(_options.PreviousManifestPath, cancel);

            // Start timer 
            var startTime = DateTime.UtcNow;
            _timer.Start();

            // Build plan
            var plan = _planBuilder.Build();

            // Execute plan
            var result = await _migrator.ExecuteAsync(plan, prevManifest, cancel);

            _timer.Stop();

            await _manifestSerializer.SaveAsync(result.Manifest, manifestFilePath);
            PrintResult(result);

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

            // Print out total results
            foreach (var type in ServerToCloudMigrationPipeline.ContentTypes)
            {
                var contentType = type.ContentType;

                var typeResult = result.Manifest.Entries.ForContentType(contentType);

                var countTotal = typeResult.Count;
                var countMigrated = typeResult.Where(x => x.Status == MigrationManifestEntryStatus.Migrated).Count();
                var countSkipped = typeResult.Where(x => x.Status == MigrationManifestEntryStatus.Skipped).Count();
                var countErrored = typeResult.Where(x => x.Status == MigrationManifestEntryStatus.Error).Count();
                var countCancelled = typeResult.Where(x => x.Status == MigrationManifestEntryStatus.Canceled).Count();
                var countPending = typeResult.Where(x => x.Status == MigrationManifestEntryStatus.Pending).Count();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"{contentType.Name}");
                sb.AppendLine($"\t{countMigrated}/{countTotal} succeeded");
                sb.AppendLine($"\t{countSkipped}/{countTotal} skipped");
                sb.AppendLine($"\t{countErrored}/{countTotal} errored");
                sb.AppendLine($"\t{countCancelled}/{countTotal} cancelled");
                sb.AppendLine($"\t{countPending}/{countTotal} pending");

                _logger.LogInformation(sb.ToString());
            }
        }

        private async Task<MigrationManifest?> LoadManifest(string manifestFilepath, CancellationToken cancel)
        {
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
                {
                    return null;
                }

                _logger.LogInformation($"Using previous manifest from {manifestFilepath}");
                return manifest;
            }

            return null;
        }
    }
}
