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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Engine.Migrators
{
    /// <summary>
    /// Default <see cref="IMigrator"/> implementation.
    /// </summary>
    public class Migrator : IMigrator
    {
        private readonly IMigrationManifestFactory _manifestFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<Migrator> _log;
        private readonly ISharedResourcesLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="Migrator"/> object.
        /// </summary>
        /// <param name="manifestFactory">A manifest factory to create empty manifests for on fatal errors.</param>
        /// <param name="serviceScopeFactory">The DI scope factory to create migration scopes and dependencies from.</param>
        /// <param name="log">A logger to write output to.</param>
        /// <param name="localizer">A string localizer.</param>
        public Migrator(IMigrationManifestFactory manifestFactory, IServiceScopeFactory serviceScopeFactory,
            ILogger<Migrator> log, ISharedResourcesLocalizer localizer)
        {
            _manifestFactory = manifestFactory;
            _serviceScopeFactory = serviceScopeFactory;
            _log = log;
            _localizer = localizer;
        }

        /// <inheritdoc />
        public async Task<MigrationResult> ExecuteAsync(IMigrationPlan plan, CancellationToken cancel) => await ExecuteAsync(plan, null, cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<MigrationResult> ExecuteAsync(IMigrationPlan plan, IMigrationManifest? previousManifest, CancellationToken cancel)
        {
            var serviceScope = _serviceScopeFactory.CreateAsyncScope();
            await using (serviceScope.ConfigureAwait(false))
            {
                var services = serviceScope.ServiceProvider;

                IMigration? migration = null;
                try
                {
                    var inputInitializer = services.GetRequiredService<IMigrationInputInitializer>();
                    inputInitializer.Initialize(plan, previousManifest);

                    cancel.ThrowIfCancellationRequested();

                    //Initialize endpoints - any failure to connect is a fatal error before the pipeline is executed.
                    migration = services.GetRequiredService<IMigration>();
                    var endpointInitTasks = new[]
                    {
                        migration.Source.InitializeAsync(cancel),
                        migration.Destination.InitializeAsync(cancel)
                    };

                    var endpointInitResults = await Task.WhenAll(endpointInitTasks).ConfigureAwait(false);

                    cancel.ThrowIfCancellationRequested();

                    var anyEndpointFailures = endpointInitResults.Any(r => !r.Success);
                    if (anyEndpointFailures)
                    {
                        var completionStatus = endpointInitResults.All(r => r.Errors.All(e => e.IsCancellationException())) ? MigrationCompletionStatus.Canceled : MigrationCompletionStatus.FatalError;

                        if (completionStatus != MigrationCompletionStatus.Canceled) //Don't dirty the end result with op/task canceled exceptions.
                        {
                            migration.Manifest.AddErrors(endpointInitResults);

                            //Extra error log entry to clarify why we didn't start any migration.
                            _log.LogError(_localizer[SharedResourceKeys.EndpointInitializationError]);
                        }

                        return new(completionStatus, migration.Manifest);
                    }

                    cancel.ThrowIfCancellationRequested();

                    //Execute the pipeline of migration actions.
                    var pipelineRunner = services.GetRequiredService<IMigrationPipelineRunner>();

                    var pipelineResult = await pipelineRunner.ExecuteAsync(migration.Pipeline, cancel).ConfigureAwait(false);

                    //Any errors bubbled up through the pipeline/actions are put at the top level of the manifest.
                    migration.Manifest.AddErrors(pipelineResult);

                    return new(MigrationCompletionStatus.Completed, migration.Manifest);
                }
                catch (Exception ex)
                {
                    var completionStatus = ex.IsCancellationException() ? MigrationCompletionStatus.Canceled : MigrationCompletionStatus.FatalError;

                    var manifest = migration?.Manifest ?? _manifestFactory.Create(plan.PlanId, Guid.NewGuid());
                    if (completionStatus != MigrationCompletionStatus.Canceled) //Don't dirty the end result with op/task canceled exceptions.
                    {
                        manifest.AddErrors(ex);
                    }

                    return new(completionStatus, manifest);
                }
            }
        }
    }
}
