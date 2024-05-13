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
                var input = Create<IMigrationInput>();

                var mockPipelineFactory = Create<Mock<IMigrationPipelineFactory>>();

                var sourceEndpoint = Freeze<ISourceEndpoint>();
                var destinationEndpoint = Freeze<IDestinationEndpoint>();

                var mockEndpointFactory = Create<Mock<IMigrationEndpointFactory>>();

                var manifest = Create<IMigrationManifestEditor>();

                var mockManifestFactory = Freeze<Mock<IMigrationManifestFactory>>();
                mockManifestFactory.Setup(x => x.Create(input, It.IsAny<Guid>())).Returns(manifest);

                var m = new Migration.Engine.Migration(input, mockPipelineFactory.Object, mockEndpointFactory.Object, mockManifestFactory.Object);

                Assert.Equal(input.MigrationId, m.Id);
                Assert.Same(input.Plan, m.Plan);

                Assert.Same(sourceEndpoint, m.Source);
                Assert.Equal(destinationEndpoint, m.Destination);

                Assert.Same(manifest, m.Manifest);
                Assert.NotSame(input.PreviousManifest, m.Manifest);
            }
        }
    }
}
