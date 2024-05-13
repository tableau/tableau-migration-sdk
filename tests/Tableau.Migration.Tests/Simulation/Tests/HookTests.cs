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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Hooks;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    /// <summary>
    /// Test class containing end-to-end tests for hooks.
    /// </summary>
    public class HookTests
    {
        public class CallCounter
        {
            public int Count { get; set; }
        }

        public abstract class MigrationActionCompletedHookTest : ServerToCloudSimulationTestBase
        {
            protected readonly CallCounter CallCounter = new();

            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                return services
                    .AddTableauMigrationSdk()
                    .AddSingleton(CallCounter);
            }

            protected async Task TestActionCompletedHook(Action<IMigrationPlanBuilder> registerHook)
            {
                var planBuilder = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .ForServerToCloud()
                    .FromSource(SourceEndpointConfig)
                    // This likely won't be needed once ServerToCloudMigrationPipeline is implemented.
                    .ToDestination(CloudDestinationEndpointConfig);

                registerHook(planBuilder);

                var plan = planBuilder
                    .Build();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();
                var result = await migrator.ExecuteAsync(plan, Cancel);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);

                //Hook call count should equal total pipeline length, which we have to do some internal work to find dynamically.
                await using var countScope = ServiceProvider.CreateAsyncScope();

                var init = countScope.ServiceProvider.GetRequiredService<IMigrationInputInitializer>();
                init.Initialize(plan, null);

                var pipelineFactory = countScope.ServiceProvider.GetRequiredService<IMigrationPipelineFactory>();
                var pipeline = pipelineFactory.Create(plan);

                Assert.Equal(pipeline.BuildActions().Length, CallCounter.Count);
            }
        }

        public class MigrationActionCompletedHookTests : MigrationActionCompletedHookTest
        {
            #region - User Defined Type (Non-DI) -

            private class MyActionCompletedHook : IMigrationActionCompletedHook
            {
                private readonly CallCounter _hookCallCounter;

                public MyActionCompletedHook(CallCounter hookCallCounter)
                {
                    _hookCallCounter = hookCallCounter;
                }

                public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
                {
                    _hookCallCounter.Count++;
                    return Task.FromResult<IMigrationActionResult?>(ctx);
                }
            }

            [Fact]
            public async Task UserDefinedHookTypeAsync()
            {
                await TestActionCompletedHook(planBuilder =>
                {
                    var hook = new MyActionCompletedHook(CallCounter);
                    planBuilder.Hooks.Add(hook);
                });
            }

            #endregion
        }

        public class MigrationActionCompletedUserInjectedHookTests : MigrationActionCompletedHookTest
        {
            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                return base.ConfigureServices(services)
                    .AddTransient<MyInjectedActionCompletedHook>();
            }

            #region - User Defined Type (DI) -

            private class MyInjectedActionCompletedHook : IMigrationActionCompletedHook
            {
                private readonly IMigrationPlan _plan;
                private readonly CallCounter _hookCallCounter;

                public MyInjectedActionCompletedHook(IMigrationPlan plan, CallCounter hookCallCounter)
                {
                    _plan = plan;
                    _hookCallCounter = hookCallCounter;
                }

                public Task<IMigrationActionResult?> ExecuteAsync(IMigrationActionResult ctx, CancellationToken cancel)
                {
                    _hookCallCounter.Count++;
                    return Task.FromResult<IMigrationActionResult?>(ctx);
                }
            }

            [Fact]
            public async Task UserInjectedHookTypeDefaultAsync()
            {
                await TestActionCompletedHook(planBuilder =>
                {
                    planBuilder.Hooks.Add<MyInjectedActionCompletedHook>();
                });
            }

            [Fact]
            public async Task UserInjectedHookTypeInitializerAsync()
            {
                await TestActionCompletedHook(planBuilder =>
                {
                    planBuilder.Hooks.Add(services => services.GetRequiredService<MyInjectedActionCompletedHook>());
                });
            }

            #endregion

            [Fact]
            public async Task CallbackAsync()
            {
                await TestActionCompletedHook(planBuilder =>
                {
                    planBuilder.Hooks.Add<IMigrationActionCompletedHook, IMigrationActionResult>((ctx, cancel) =>
                    {
                        CallCounter.Count++;
                        return Task.FromResult<IMigrationActionResult?>(ctx);
                    });
                });
            }
        }
    }
}
