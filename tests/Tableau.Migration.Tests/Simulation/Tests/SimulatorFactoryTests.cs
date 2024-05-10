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

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests
{
    public class SimulatorFactoryTests
    {
        public class Authentication : SimulationTestBase
        {
            protected override IServiceCollection ConfigureServices(IServiceCollection services)
            {
                return services.AddTableauMigrationSdk();
            }

            /// <summary>
            /// GH Issue #9
            /// Ensure the simulator built by a plan builder can authenticate.
            /// </summary>
            [Fact]
            public async Task AuthenticatesAsync()
            {
                var plan = ServiceProvider.GetRequiredService<IMigrationPlanBuilder>()
                    .FromSourceTableauServer(new("http://source"), "s", "sTokenName", "sToken", createApiSimulator: true)
                    .ToDestinationTableauCloud(new("http://destination"), "d", "dTokenName", "dToken", createApiSimulator: true)
                    .ForServerToCloud()
                    .Build();

                var migrator = ServiceProvider.GetRequiredService<IMigrator>();
                var result = await migrator.ExecuteAsync(plan, Cancel);

                //Assert - empty migration should have succeded.

                Assert.Empty(result.Manifest.Errors);
                Assert.Equal(MigrationCompletionStatus.Completed, result.Status);
            }
        }
    }
}
