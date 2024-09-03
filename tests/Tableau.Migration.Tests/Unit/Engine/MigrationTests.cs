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
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine
{
    public class MigrationTests
    {
        public class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.PipelineFactoryOverride).Returns((Func<IServiceProvider, IMigrationPipelineFactory>?)null);

                var input = Freeze<IMigrationInput>();

                var pipeline = Freeze<IMigrationPipeline>();
                var mockPipelineFactory = Freeze<Mock<IMigrationPipelineFactory>>();

                var sourceEndpoint = Freeze<ISourceEndpoint>();
                var destinationEndpoint = Freeze<IDestinationEndpoint>();

                var mockEndpointFactory = Freeze<Mock<IMigrationEndpointFactory>>();

                var manifest = Freeze<IMigrationManifestEditor>();

                var mockManifestFactory = Freeze<Mock<IMigrationManifestFactory>>();
                mockManifestFactory.Setup(x => x.Create(input, It.IsAny<Guid>())).Returns(manifest);

                var m = Create<Migration.Engine.Migration>();

                Assert.Equal(input.MigrationId, m.Id);
                Assert.Same(input.Plan, m.Plan);

                Assert.Same(pipeline, m.Pipeline);
                mockPipelineFactory.Verify(x => x.Create(input.Plan), Times.Once);

                Assert.Same(sourceEndpoint, m.Source);
                Assert.Equal(destinationEndpoint, m.Destination);

                Assert.Same(manifest, m.Manifest);
                Assert.NotSame(input.PreviousManifest, m.Manifest);
            }

            [Fact]
            public void InitializesWithCustomPipeline()
            {
                var customPipeline = Create<IMigrationPipeline>();
                var mockCustomPipelineFactory = new Mock<IMigrationPipelineFactory>();
                mockCustomPipelineFactory.Setup(x => x.Create(It.IsAny<IMigrationPlan>())).Returns(customPipeline);
                IMigrationPipelineFactory CustomPipelineInitializer(IServiceProvider s) => mockCustomPipelineFactory.Object;

                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.PipelineFactoryOverride).Returns(CustomPipelineInitializer);

                var input = Freeze<IMigrationInput>();

                var mockDefaultPipelineFactory = Freeze<Mock<IMigrationPipelineFactory>>();

                var sourceEndpoint = Freeze<ISourceEndpoint>();
                var destinationEndpoint = Freeze<IDestinationEndpoint>();

                var mockEndpointFactory = Freeze<Mock<IMigrationEndpointFactory>>();

                var manifest = Freeze<IMigrationManifestEditor>();

                var mockManifestFactory = Freeze<Mock<IMigrationManifestFactory>>();
                mockManifestFactory.Setup(x => x.Create(input, It.IsAny<Guid>())).Returns(manifest);

                var m = Create<Migration.Engine.Migration>();

                Assert.Equal(input.MigrationId, m.Id);
                Assert.Same(input.Plan, m.Plan);

                Assert.Same(customPipeline, m.Pipeline);
                mockDefaultPipelineFactory.Verify(x => x.Create(input.Plan), Times.Never);
                mockCustomPipelineFactory.Verify(x => x.Create(input.Plan), Times.Once);

                Assert.Same(sourceEndpoint, m.Source);
                Assert.Equal(destinationEndpoint, m.Destination);

                Assert.Same(manifest, m.Manifest);
                Assert.NotSame(input.PreviousManifest, m.Manifest);
            }
        }
    }
}
