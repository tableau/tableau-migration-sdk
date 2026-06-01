//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public class FlowRunTaskFlowIdTransformerTests
    {
        public abstract class FlowRunTaskFlowIdTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockDestinationContentReferenceFinderFactory = new();
            protected readonly Mock<ILogger<FlowRunTaskFlowIdTransformer>> MockLogger;
            protected readonly MockSharedResourcesLocalizer MockSharedResourcesLocalizer = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IFlow>> MockFlowContentFinder = new();

            protected readonly FlowRunTaskFlowIdTransformer Transformer;

            public FlowRunTaskFlowIdTransformerTest()
            {
                MockLogger = Create<Mock<ILogger<FlowRunTaskFlowIdTransformer>>>();

                MockDestinationContentReferenceFinderFactory.Setup(p => p.ForDestinationContentType<IFlow>()).Returns(MockFlowContentFinder.Object);

                Transformer = new(MockDestinationContentReferenceFinderFactory.Object, MockSharedResourcesLocalizer.Object, MockLogger.Object);
            }
        }

        public class TransformAsync : FlowRunTaskFlowIdTransformerTest
        {
            [Fact]
            public async Task ReturnsSameObjectAsync()
            {
                var flowRunTask = Create<ICloudFlowRunTask>();

                var sourceFlow = Create<IContentReference>();
                flowRunTask.Flow = sourceFlow;

                var destinationFlow = Create<IContentReference>();
                MockFlowContentFinder.Setup(f => f.FindBySourceIdAsync(sourceFlow.Id, Cancel)).ReturnsAsync(destinationFlow);

                var result = await Transformer.TransformAsync(flowRunTask, Cancel);

                Assert.NotNull(result);
                Assert.Same(flowRunTask, result);
            }

            [Fact]
            public async Task ReturnsDestinationFlowWhenFoundAsync()
            {
                var flowRunTask = Create<ICloudFlowRunTask>();
                var sourceFlow = Create<IContentReference>();
                var destinationFlow = Create<IContentReference>();
                flowRunTask.Flow = sourceFlow;

                MockFlowContentFinder.Setup(f => f.FindBySourceIdAsync(sourceFlow.Id, Cancel)).ReturnsAsync(destinationFlow);

                var result = await Transformer.TransformAsync(flowRunTask, Cancel);

                Assert.NotNull(result);
                Assert.Same(flowRunTask, result);
                MockLogger.VerifyWarnings(Times.Never);
                Assert.Same(destinationFlow, result.Flow);
            }

            [Fact]
            public async Task ReturnsSameObjectWhenFlowIsNullAsync()
            {
                var flowRunTask = Create<ICloudFlowRunTask>();
                flowRunTask.Flow = null!;

                var result = await Transformer.TransformAsync(flowRunTask, Cancel);

                Assert.NotNull(result);
                Assert.Same(flowRunTask, result);
                MockFlowContentFinder.Verify(f => f.FindBySourceIdAsync(It.IsAny<System.Guid>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task ThrowsOnDestinationFlowNotFoundAsync()
            {
                var flowRunTask = Create<ICloudFlowRunTask>();
                var sourceFlow = Create<IContentReference>();
                flowRunTask.Flow = sourceFlow;

                // Return null to simulate flow not found
                MockFlowContentFinder.Setup(f => f.FindBySourceIdAsync(sourceFlow.Id, Cancel))
                    .ReturnsAsync((IContentReference?)null);

                await Assert.ThrowsAsync<Exception>(() => 
                    Transformer.TransformAsync(flowRunTask, Cancel));
            }
        }
    }
}
