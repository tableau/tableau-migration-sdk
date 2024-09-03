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
using System.Collections.Generic;
using Moq;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public sealed class CustomMigrationPipelineFactoryTests
    {
        public sealed class TestCustomPipeline : MigrationPipelineBase
        {
            public TestCustomPipeline(IServiceProvider services) 
                : base(services)
            { }

            protected override IEnumerable<IMigrationAction> BuildPipeline()
            {
                throw new NotImplementedException();
            }
        }

        public sealed class Create : AutoFixtureTestBase
        {
            [Fact]
            public void CreatesCustomPipeline()
            {
                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.PipelineProfile).Returns(PipelineProfile.Custom);

                var pipeline = Freeze<TestCustomPipeline>();

                var factory = Create<CustomMigrationPipelineFactory<TestCustomPipeline>>();

                var result = factory.Create(mockPlan.Object);

                Assert.Same(pipeline, result);
            }
        }
    }
}
