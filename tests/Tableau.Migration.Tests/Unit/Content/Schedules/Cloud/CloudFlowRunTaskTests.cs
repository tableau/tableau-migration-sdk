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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Schedules.Cloud
{
    public sealed class CloudFlowRunTaskTests
    {
        public abstract class CloudFlowRunTaskTest : ScheduleTestBase
        {
            protected readonly Mock<IContentReferenceFinderFactory> MockFinderFactory = new() { CallBase = true };
            protected readonly Mock<IContentReferenceFinder<IFlow>> MockFlowFinder = new();
            protected ILogger Logger { get; }
            protected ISharedResourcesLocalizer Localizer { get; }

            protected CloudFlowRunTaskTest()
            {
                Logger = Create<ILogger>();
                Localizer = Create<ISharedResourcesLocalizer>();
                MockFinderFactory.Setup(x => x.ForContentType<IFlow>()).Returns(MockFlowFinder.Object);
                MockFlowFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync((IContentReference?)null);
            }

            protected FlowRunTasksResponse.TaskType.FlowRunType CreateFlowRunResponse(
                Guid? id = null,
                string? type = null,
                int? priority = null,
                int? consecutiveFailedCount = null,
                Guid? flowId = null,
                string? scheduleFrequency = null)
            {
                return new FlowRunTasksResponse.TaskType.FlowRunType
                {
                    Id = id ?? Guid.NewGuid(),
                    Type = type ?? "RunFlowTask",
                    Priority = priority ?? 50,
                    ConsecutiveFailedCount = consecutiveFailedCount ?? 0,
                    Flow = flowId.HasValue ? new FlowRunTasksResponse.TaskType.FlowRunType.FlowType
                    {
                        Id = flowId.Value
                    } : null,
                    Schedule = scheduleFrequency != null ? new FlowRunTasksResponse.TaskType.FlowRunType.ScheduleType
                    {
                        Frequency = scheduleFrequency
                    } : null
                };
            }

            protected IContentReference CreateFlowReference(Guid? id = null)
            {
                var mock = new Mock<IContentReference>();
                mock.SetupGet(r => r.Id).Returns(id ?? Guid.NewGuid());
                return mock.Object;
            }
        }

        public sealed class Ctor : CloudFlowRunTaskTest
        {
            [Fact]
            public void Initializes()
            {
                var flowTaskId = Guid.NewGuid();
                var type = "RunFlowTask";
                var priority = 50;
                var consecutiveFailedCount = 0;
                var flow = CreateFlowReference();
                var schedule = CreateCloudSchedule();

                var task = new CloudFlowRunTask(flowTaskId, type, priority, consecutiveFailedCount, flow, schedule);

                Assert.Equal(flowTaskId, task.Id);
                Assert.Equal(type, task.Type);
                Assert.Equal(priority, task.Priority);
                Assert.Equal(consecutiveFailedCount, task.ConsecutiveFailedCount);
                Assert.Same(flow, task.Flow);
                Assert.Same(schedule, task.Schedule);
            }
        }

        public sealed class CreateManyAsync : CloudFlowRunTaskTest
        {
            [Fact]
            public async Task IgnoresTasksWithMissingFlowReferencesAsync()
            {
                var flowId1 = Guid.NewGuid();
                var flowId2 = Guid.NewGuid();
                var flowId3 = Guid.NewGuid();

                var response = new FlowRunTasksResponse
                {
                    Items = new[]
                    {
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(flowId: flowId1)
                        },
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(flowId: flowId2)
                        },
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(flowId: flowId3)
                        }
                    }
                };

                // Setup flow finder to return reference only for flowId2
                var flowReference2 = CreateFlowReference(flowId2);
                MockFlowFinder.Setup(f => f.FindByIdAsync(flowId2, It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(flowReference2);

                var tasks = await CloudFlowRunTask.CreateManyAsync(response, MockFinderFactory.Object, Logger, Cancel);

                // Should only have one task (for flowId2)
                Assert.Single(tasks);
                Assert.Equal(flowId2, tasks[0].Flow.Id);
            }

            [Fact]
            public async Task IgnoresTasksWithEmptyFlowIdAsync()
            {
                var flowId = Guid.NewGuid();

                var response = new FlowRunTasksResponse
                {
                    Items = new[]
                    {
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(flowId: Guid.Empty) // Empty flow ID
                        },
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(flowId: flowId)
                        }
                    }
                };

                var flowReference = CreateFlowReference(flowId);
                MockFlowFinder.Setup(f => f.FindByIdAsync(flowId, It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(flowReference);

                var tasks = await CloudFlowRunTask.CreateManyAsync(response, MockFinderFactory.Object, Logger, Cancel);

                // Should only have one task (for the valid flowId)
                Assert.Single(tasks);
                Assert.Equal(flowId, tasks[0].Flow.Id);
            }

            [Fact]
            public async Task CreatesTasksWithSchedulesAsync()
            {
                var flowId = Guid.NewGuid();
                var flowReference = CreateFlowReference(flowId);

                var response = new FlowRunTasksResponse
                {
                    Items = new[]
                    {
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(
                                flowId: flowId,
                                scheduleFrequency: "Hourly")
                        }
                    }
                };

                MockFlowFinder.Setup(f => f.FindByIdAsync(flowId, It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(flowReference);

                var tasks = await CloudFlowRunTask.CreateManyAsync(response, MockFinderFactory.Object, Logger, Cancel);

                Assert.Single(tasks);
                Assert.Equal("Hourly", tasks[0].Schedule.Frequency);
            }

            [Fact]
            public async Task CreatesTasksWithoutSchedulesAsync()
            {
                var flowId = Guid.NewGuid();
                var flowReference = CreateFlowReference(flowId);

                var response = new FlowRunTasksResponse
                {
                    Items = new[]
                    {
                        new FlowRunTasksResponse.TaskType
                        {
                            FlowRun = CreateFlowRunResponse(flowId: flowId, scheduleFrequency: null)
                        }
                    }
                };

                MockFlowFinder.Setup(f => f.FindByIdAsync(flowId, It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(flowReference);

                var tasks = await CloudFlowRunTask.CreateManyAsync(response, MockFinderFactory.Object, Logger, Cancel);

                Assert.Single(tasks);
                Assert.NotNull(tasks[0].Schedule);
            }
        }
    }
}
