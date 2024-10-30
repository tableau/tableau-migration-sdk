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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
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

        private readonly Assembly _tableauMigrationAssembly;

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

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Find the assembly by name
            _tableauMigrationAssembly = assemblies.FirstOrDefault(a => a.GetName().Name == "Tableau.Migration") ?? throw new Exception("Could not find Tableau.Migration assembly");

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

            if (!_options.SkipTypes.Any())
            {
                _logger.LogInformation("No SkipFilter types provided. Skipping no content types.");
            }

            // Add SkipFilter for each type in the configuration
            foreach (string skipTypeStr in _options.SkipTypes)
            {
                var contentType = _tableauMigrationAssembly.GetTypes().FirstOrDefault(t => t.Name == skipTypeStr);

                if (contentType is null)
                {
                    _logger.LogCritical($"Could not find type Tableau.Migration.Content.{skipTypeStr} to skip.");
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                    _appLifetime.StopApplication();
                    return;
                }

                _planBuilder.Filters.Add(typeof(SkipFilter<>), new[] { new[] { contentType! } });
                _logger.LogInformation("Created SkipFilter for type {ContentType}", contentType.Name);
            }

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
            if (_options.SpecialUsers.Emails.Any())
            {
                _planBuilder.Filters.Add<SpecialUserFilter, IUser>();
                _planBuilder.Mappings.Add<SpecialUserMapping, IUser>();
            }

            // Map unlicensed users to single admin
            _planBuilder.Mappings.Add<UnlicensedUserMapping, IUser>();

            // Save manifest every every batch of every content type.
            var contentTypeArrays = ServerToCloudMigrationPipeline.ContentTypes.Select(t => new[] { t.ContentType });
            _planBuilder.Hooks.Add(typeof(LogMigrationBatchSummaryHook<>), contentTypeArrays);
            if (!string.IsNullOrEmpty(_options.Log.ManifestFolderPath))
            {
                _planBuilder.Hooks.Add(typeof(SaveManifestAfterBatchMigrationCompletedHook<>), contentTypeArrays);
            }

            // ViewOwnerTransformer
            _planBuilder.Transformers.Add<ViewerOwnerTransformer<IProject>, IProject>();
            _planBuilder.Transformers.Add<ViewerOwnerTransformer<IDataSource>, IDataSource>();
            _planBuilder.Transformers.Add<ViewerOwnerTransformer<IWorkbook>, IWorkbook>();

            // Log when a content type is done
            _planBuilder.Hooks.Add<TimeLoggerAfterActionHook>();

            _planBuilder.Filters.Add<SkipByParentLocationFilter<IProject>, IProject>();
            _planBuilder.Filters.Add<SkipByParentLocationFilter<IDataSource>, IDataSource>();
            _planBuilder.Filters.Add<SkipByParentLocationFilter<IWorkbook>, IWorkbook>();
            _planBuilder.Transformers.Add<RemoveMissingDestinationUsersFromGroupsTransformer, IPublishableGroup>();
            _planBuilder.Mappings.Add<ContentWithinSkippedLocationMapping<IProject>, IProject>();
            _planBuilder.Mappings.Add<ContentWithinSkippedLocationMapping<IDataSource>, IDataSource>();
            _planBuilder.Mappings.Add<ContentWithinSkippedLocationMapping<IWorkbook>, IWorkbook>();

            var prevManifest = await LoadManifest(_options.PreviousManifestPath, cancel);

            // Start timer 
            var startTime = DateTime.UtcNow;
            _timer.Start();

            // Build plan
            var plan = _planBuilder.Build();

            // Execute plan
            var result = await _migrator.ExecuteAsync(plan, prevManifest, cancel);

            _timer.Stop();

            var endTime = DateTime.UtcNow;

            await _manifestSerializer.SaveAsync(result.Manifest, manifestFilePath);

            _logger.LogInformation(MigrationSummaryBuilder.Build(result, startTime, endTime, _timer.Elapsed));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            _appLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancel) => Task.CompletedTask;


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
