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
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineFactoryTests
    {
        public class Create : AutoFixtureTestBase
        {
            private readonly MockServiceProvider _mockServices;

            private readonly IConfigReader _mockConfigReader;

            private readonly MigrationPipelineFactory _factory;

            public Create()
            {
                _mockServices = Create<MockServiceProvider>();

                _mockConfigReader = Create<IConfigReader>();

                _factory = new(_mockServices.Object);
            }

            [Fact]
            public void CreatesServerToCloudMigration()
            {
                var pipeline = new ServerToCloudMigrationPipeline(_mockServices.Object, _mockConfigReader);
                _mockServices.Setup(x => x.GetService(typeof(ServerToCloudMigrationPipeline))).Returns(pipeline);

                var mockPlan = Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.PipelineProfile).Returns(PipelineProfile.ServerToCloud);

                var result = _factory.Create(mockPlan.Object);

                Assert.Same(pipeline, result);
            }

            [Fact]
            public void UnsupportedProfile()
            {
                var mockPlan = Create<Mock<IMigrationPlan>>();
                mockPlan.SetupGet(x => x.PipelineProfile).Returns((PipelineProfile)int.MaxValue);

                Assert.Throws<ArgumentException>(() => _factory.Create(mockPlan.Object));
            }
        }
    }
}
