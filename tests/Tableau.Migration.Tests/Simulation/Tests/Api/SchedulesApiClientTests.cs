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
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class SchedulesApiClientTests
    {
        public abstract class SchedulesApiClientTest : ApiClientTestBase
        { }

        public class GetByIdAsync : SchedulesApiClientTest
        {
            [Fact]
            public async Task Returns_Schedule_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var schedule = Create<ScheduleResponse.ScheduleType>();

                Api.Data.Schedules.Add(schedule);

                var result = await sitesClient.Schedules.GetByIdAsync(schedule.Id, Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_error_when_not_found()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var result = await sitesClient.Schedules.GetByIdAsync(Create<Guid>(), Cancel);

                Assert.False(result.Success);
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);
            }
        }

        public class GetScheduleExtractRefreshTasksAsync : SchedulesApiClientTest
        {
            [Fact]
            public async Task Returns_success_on_success()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var schedule = Create<ScheduleResponse.ScheduleType>();

                const int ADDITIONAL_EXTRACTS_COUNT = 7;

                // Add a few more extract refresh tasks for the schedules
                for (int i = 0; i < ADDITIONAL_EXTRACTS_COUNT; i++)
                {
                    Api.Data.CreateScheduleExtractRefreshTask(
                        AutoFixture);
                }

                const int EXTRACTS_COUNT = 10;

                for (var i = 0; i != EXTRACTS_COUNT; i++)
                {
                    Api.Data.CreateScheduleExtractRefreshTask(
                        AutoFixture,
                        schedule);
                }

                // Act
                var result = await sitesClient.Schedules.GetScheduleExtractRefreshTasksAsync(schedule.Id, 1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.NotNull(result.Value);
                Assert.Equal(EXTRACTS_COUNT, result.Value.Count);
                Assert.Equal(EXTRACTS_COUNT, result.TotalCount);
            }

            [Fact]
            public async Task Returns_success_when_no_schedules_exist()
            {
                // Arrange 
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var schedule = Create<ScheduleResponse.ScheduleType>();

                const int ADDITIONAL_EXTRACTS_COUNT = 7;

                // Add a few more extract refresh tasks for the schedules
                for (int i = 0; i < ADDITIONAL_EXTRACTS_COUNT; i++)
                {
                    Api.Data.CreateScheduleExtractRefreshTask(
                        AutoFixture);
                }

                // Act
                var result = await sitesClient.Schedules.GetScheduleExtractRefreshTasksAsync(schedule.Id, 1, 100, Cancel);

                // Assert
                Assert.True(result.Success);
                Assert.Empty(result.Value);
            }
        }
    }
}
