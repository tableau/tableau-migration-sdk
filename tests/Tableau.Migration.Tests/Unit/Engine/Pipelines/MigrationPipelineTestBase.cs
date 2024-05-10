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
using AutoFixture;
using Moq;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Hooks;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineTestBase<TPipeline> : AutoFixtureTestBase
    {
        protected readonly Mock<IServiceProvider> MockServices;
        protected readonly Mock<IMigrationHookRunner> MockHookRunner;
        protected readonly Mock<IDestinationApiEndpoint> MockDestinationEndpoint;
        protected readonly Mock<ISourceApiEndpoint> MockSourceEndpoint;

        protected readonly TPipeline Pipeline;

        protected virtual TPipeline CreatePipeline()
            => Create<TPipeline>();

        public MigrationPipelineTestBase()
        {
            MockServices = Freeze<MockServiceProvider>();
            MockServices.Setup(x => x.GetService(typeof(TestAction))).Returns(() => new TestAction());

            MockHookRunner = Freeze<Mock<IMigrationHookRunner>>();
            MockHookRunner.Setup(x => x.ExecuteAsync<IMigrationActionCompletedHook, IMigrationActionResult>(It.IsAny<IMigrationActionResult>(), Cancel))
                .ReturnsAsync((IMigrationActionResult r, CancellationToken c) => r);

            MockDestinationEndpoint = Freeze<Mock<IDestinationApiEndpoint>>();
            MockSourceEndpoint = Freeze<Mock<ISourceApiEndpoint>>();

            AutoFixture.Register<IDestinationEndpoint>(() => MockDestinationEndpoint.Object);
            AutoFixture.Register<ISourceEndpoint>(() => MockSourceEndpoint.Object);

            Pipeline = CreatePipeline();
        }
    }
}
