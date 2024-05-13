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

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Api;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    /// <summary>
    /// Test class containing top-level end-to-end simulation tests.
    /// </summary>
    public class MigratorTests : ServerToCloudSimulationTestBase
    {
        /// <summary>
        /// Test that a migration with as-close-to-default values succeeds.
        /// </summary>
        [Fact]
        public async Task BasicSuccessAsync()
        {
            var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                .FromSource(SourceEndpointConfig)
                // This likely won't be needed once ServerToCloudMigrationPipeline is implemented.
                .ToDestination(CloudDestinationEndpointConfig)
                .ForServerToCloud()
                .Build();

            var migrator = ServiceProvider.GetRequiredService<IMigrator>();
            var result = await migrator.ExecuteAsync(plan, Cancel);

            Assert.Empty(result.Manifest.Errors);
            Assert.Equal(MigrationCompletionStatus.Completed, result.Status);
            Assert.All(result.Manifest.Entries, entry => Assert.True(entry.Status == MigrationManifestEntryStatus.Migrated || entry.Status == MigrationManifestEntryStatus.Skipped));
        }

        /// <summary>
        /// Test a migration with failures.
        /// </summary>
        [Fact]
        public async Task FailAsync()
        {
            var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                .FromSource(SourceEndpointConfig)
                // This likely won't be needed once ServerToCloudMigrationPipeline is implemented.
                .ToDestination(new TableauApiEndpointConfiguration(new TableauSiteConnectionConfiguration()))
                .ForServerToCloud()
                .Build();

            var migrator = ServiceProvider.GetRequiredService<IMigrator>();
            var result = await migrator.ExecuteAsync(plan, Cancel);

            Assert.NotEmpty(result.Manifest.Errors);
            Assert.Equal(MigrationCompletionStatus.FatalError, result.Status);
        }

        /// <summary>
        /// Test a migration that was cancelled.
        /// </summary>
        [Fact]
        public async Task CancelAsync()
        {
            var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                .FromSource(SourceEndpointConfig)
                // This likely won't be needed once ServerToCloudMigrationPipeline is implemented.
                .ToDestination(CloudDestinationEndpointConfig)
                .ForServerToCloud()
                .Build();

            var migrator = ServiceProvider.GetRequiredService<IMigrator>();

            var result = await migrator.ExecuteAsync(plan, new CancellationToken(true));

            Assert.Empty(result.Manifest.Errors);
            Assert.Equal(MigrationCompletionStatus.Canceled, result.Status);
        }

        /// <summary>
        /// Test a migration that was cancelled by a TaskCanceledException
        /// </summary>
        [Fact]
        public async Task ThrowCancelAsync()
        {
            static Task<IMigrationActionResult?> throwCancelOnCallback(IMigrationActionResult ctx, CancellationToken cancel)
            {
                throw new TaskCanceledException();
            }
            var planBuilder = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                .FromSource(SourceEndpointConfig)
                // This likely won't be needed once ServerToCloudMigrationPipeline is implemented.
                .ToDestination(CloudDestinationEndpointConfig)
                .ForServerToCloud();

            planBuilder.Hooks.Add<IMigrationActionCompletedHook, IMigrationActionResult>(throwCancelOnCallback);

            var plan = planBuilder.Build();

            var migrator = ServiceProvider.GetRequiredService<IMigrator>();

            var result = await migrator.ExecuteAsync(plan, Cancel);

            Assert.Empty(result.Manifest.Errors);
            Assert.Equal(MigrationCompletionStatus.Canceled, result.Status);
        }
    }
}
