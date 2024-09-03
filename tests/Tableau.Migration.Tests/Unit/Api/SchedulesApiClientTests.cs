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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Tableau.Migration.Config;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class SchedulesApiClientTests
    {
        public abstract class SchedulesApiClientTest : ApiClientTestBase<ISchedulesApiClient>
        {
            internal SchedulesApiClient SchedulesApiClient => GetApiClient<SchedulesApiClient>();
            private readonly string _baseApiUri;
            protected SchedulesApiClientTest()
            {
                _baseApiUri = $"/api/{TableauServerVersion.RestApiVersion}";
            }

            protected internal void AssertScheduleRelativeUri(HttpRequestMessage request, Guid scheduleId)
            {
                request.AssertRelativeUri($"{_baseApiUri}/schedules/{scheduleId.ToUrlSegment()}");
            }
            protected internal void AssertScheduleExtractsRelativeUri(HttpRequestMessage request, Guid scheduleId)
            {
                request.AssertRelativeUri($"{_baseApiUri}/sites/{SiteId}/schedules/{scheduleId.ToUrlSegment()}/extracts");
            }
        }

        public class GetByIdAsync : SchedulesApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var scheduleResponse = AutoFixture.CreateResponse<ScheduleResponse>();

                var mockResponse = new MockHttpResponseMessage<ScheduleResponse>(scheduleResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var scheduleExtractRefreshTasksResponse = AutoFixture.CreateResponse<ScheduleExtractRefreshTasksResponse>();
                var mockScheduleExtractRefreshTasksResponse = new MockHttpResponseMessage<ScheduleExtractRefreshTasksResponse>(
                    scheduleExtractRefreshTasksResponse);

                MockHttpClient.SetupResponse(mockScheduleExtractRefreshTasksResponse);

                var scheduleId = Guid.NewGuid();

                MockConfigReader
                    .Setup(x => x.Get<IServerExtractRefreshTask>())
                    .Returns(new ContentTypesOptions()
                    {
                        Type = "ExtractRefresh_ServerSchedule",
                        BatchSize = ContentTypesOptions.Defaults.BATCH_SIZE
                    });

                var result = await SchedulesApiClient.GetByIdAsync(scheduleId, Cancel);

                Assert.True(result.Success);

                var requests = MockHttpClient.AssertRequestCount(2);

                Assert.Collection(requests,
                    getScheduleRequest => AssertScheduleRelativeUri(getScheduleRequest, scheduleId),
                    getScheduleExtractRefTasks => AssertScheduleExtractsRelativeUri(getScheduleExtractRefTasks, scheduleId));

                MockScheduleCache.Verify(c => c.AddOrUpdate(result.Value), Times.Once);

                var serverSchedule = result.Value;
                var expectedTasks = scheduleExtractRefreshTasksResponse.Items;
                Assert.NotNull(expectedTasks);
                Assert.Equal(expectedTasks.Length, serverSchedule.ExtractRefreshTasks.Count);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var exception = new Exception();

                var mockResponse =
                    new MockHttpResponseMessage<ScheduleResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var scheduleId = Guid.NewGuid();

                var result = await SchedulesApiClient.GetByIdAsync(scheduleId, Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                AssertScheduleRelativeUri(request, scheduleId);

                MockScheduleCache.Verify(c => c.AddOrUpdate(It.IsAny<IServerSchedule>()), Times.Never);
            }
        }

        public class GetScheduleExtractRefreshTasksAsync : SchedulesApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var response = AutoFixture.CreateResponse<ScheduleExtractRefreshTasksResponse>();

                var mockResponse = new MockHttpResponseMessage<ScheduleExtractRefreshTasksResponse>(response);

                MockHttpClient.SetupResponse(mockResponse);

                var scheduleId = Guid.NewGuid();

                var result = await SchedulesApiClient.GetScheduleExtractRefreshTasksAsync(scheduleId, 1, 1, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();
                AssertScheduleExtractsRelativeUri(request, scheduleId);
            }

            [Fact]
            public async Task Returns_failure()
            {
                var exception = new Exception();

                var mockResponse =
                    new MockHttpResponseMessage<ScheduleExtractRefreshTasksResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var scheduleId = Guid.NewGuid();

                var result = await SchedulesApiClient.GetScheduleExtractRefreshTasksAsync(scheduleId, 1, 1, Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                AssertScheduleExtractsRelativeUri(request, scheduleId);
            }
        }
    }
}