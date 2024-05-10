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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Rest;
using Xunit;
using Xunit.Abstractions;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class JobsApiClientTests
    {
        public abstract class JobsApiClientTest : ApiClientTestBase<IJobsApiClient>
        {

            internal JobsApiClient JobsApiClient => GetApiClient<JobsApiClient>();

            public JobsApiClientTest()
                : base()
            {
                MockTaskDelayer.Setup(d => d.DelayAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                MockConfigReader.Setup(x => x.Get()).Returns(new MigrationSdkOptions
                {
                    Jobs = new JobOptions
                    {
                        JobPollRate = TimeSpan.FromMilliseconds(50),
                    }
                });
            }
        }

        public class GetJobStatusAsync : JobsApiClientTest
        {
            [Fact]
            public async Task Returns_success()
            {
                var jobResponse = AutoFixture.CreateResponse<JobResponse>();

                var mockResponse = new MockHttpResponseMessage<JobResponse>(jobResponse);

                MockHttpClient.SetupResponse(mockResponse);

                var jobId = Guid.NewGuid();

                var result = await JobsApiClient.GetJobStatusAsync(jobId, Cancel);

                Assert.True(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/jobs/{jobId.ToUrlSegment()}");
            }

            [Fact]
            public async Task Returns_failure()
            {
                var exception = new Exception();

                var mockResponse = new MockHttpResponseMessage<JobResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var jobId = Guid.NewGuid();

                var result = await JobsApiClient.GetJobStatusAsync(jobId, Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/jobs/{jobId.ToUrlSegment()}");
            }
        }

        public class WaitForJobAsync : JobsApiClientTest
        { 
            [Fact]
            public async Task Returns_success()
            {
                var progress = 0;

                var jobResponse = AutoFixture.CreateResponse<JobResponse>();

                MockHttpClient.SetupSuccessResponse(
                    jobResponse,
                    onRequestSent: r =>
                    {
                        jobResponse.Item!.Progress = progress;
                        jobResponse.Item.FinishCode = progress >= 100 ? 0 : 1;

                        progress += 50;
                    });

                var jobId = Guid.NewGuid();

                var result = await JobsApiClient.WaitForJobAsync(jobId, Cancel);

                Assert.True(result.Success);

                var requests = MockHttpClient.AssertRequestCount(3);

                foreach (var request in requests)
                    request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/jobs/{jobId.ToUrlSegment()}");

                MockTaskDelayer.Verify(d => d.DelayAsync(TimeSpan.FromMilliseconds(50), Cancel), Times.Exactly(2));
            }

            [Fact]
            public async Task Returns_timeout()
            {
                var mockJobPullRate = TimeSpan.FromMilliseconds(50);
                var mockJobTimeout = TimeSpan.FromMilliseconds(100);

                // Reset the delayer so it actually waits
                MockTaskDelayer.Setup(d => d.DelayAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
                    .Returns(async () =>
                    {
                        await Task.Delay(mockJobPullRate);
                    });

                MockConfigReader.Setup(x => x.Get()).Returns(new MigrationSdkOptions
                {
                    Jobs = new JobOptions
                    {
                        JobPollRate = mockJobPullRate,
                        JobTimeout = mockJobTimeout
                    }
                });

                var progress = 0;
                var startTime = DateTime.Now;

                var jobResponse = AutoFixture.CreateResponse<JobResponse>();

                MockHttpClient.SetupSuccessResponse(
                    jobResponse,
                    onRequestSent: r =>
                    {
                        jobResponse.Item!.Progress = progress;
                        jobResponse.Item.FinishCode = progress >= 100 ? 0 : 1;

                        progress += 1; // Very slow job, so it will time out given the 
                    });

                var jobId = Guid.NewGuid();

                startTime = DateTime.UtcNow;
                var result = await JobsApiClient.WaitForJobAsync(jobId, Cancel);

                result.AssertFailure();

                var resultException = Assert.Single(result.Errors);
                var restException = Assert.IsType<TimeoutJobException>(resultException);

                // Check that delay was called at most 2 times. mockJobTimeout / mockPollRate = 2ish
                // It might be less if the load on the system is a lot, then millisecond timeouts are too fast
                MockTaskDelayer.Verify(d => d.DelayAsync(mockJobPullRate, Cancel), Times.AtMost(2)); 
            }

            [Fact]
            public async Task Returns_failure_api_error()
            {
                var exception = new InvalidOperationException();

                var mockResponse = new MockHttpResponseMessage<JobResponse>(HttpStatusCode.InternalServerError, null);
                mockResponse.Setup(r => r.EnsureSuccessStatusCode()).Throws(exception);

                MockHttpClient.SetupResponse(mockResponse);

                var jobId = Guid.NewGuid();

                var result = await JobsApiClient.WaitForJobAsync(jobId, Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/jobs/{jobId.ToUrlSegment()}");

                MockTaskDelayer.Verify(d => d.DelayAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

                var resultException = Assert.Single(result.Errors);
                var restException = Assert.IsType<InvalidOperationException>(resultException);
            }

            [Fact]
            public async Task Returns_failure_job_finish_code()
            {
                var jobResponse = AutoFixture.CreateResponse<JobResponse>();
                jobResponse.Item!.Progress = 100;
                jobResponse.Item.FinishCode = 1;

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<JobResponse>(jobResponse));

                var jobId = jobResponse.Item.Id;

                var result = await JobsApiClient.WaitForJobAsync(jobId, Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/jobs/{jobId.ToUrlSegment()}");

                MockTaskDelayer.Verify(d => d.DelayAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

                var resultException = Assert.Single(result.Errors);
                var jobException = Assert.IsType<FailedJobException>(resultException);
            }

            [Fact]
            public async Task Returns_failure_job_status_note()
            {
                var jobResponse = AutoFixture.CreateResponse<JobResponse>();
                jobResponse.Item!.Progress = 100;
                jobResponse.Item.FinishCode = 0;

                jobResponse.Item.StatusNotes = CreateMany<JobResponse.JobType.StatusNoteType>(3).ToArray();
                jobResponse.Item.StatusNotes[1].Type = "errorCode";

                MockHttpClient.SetupResponse(new MockHttpResponseMessage<JobResponse>(jobResponse));

                var jobId = jobResponse.Item.Id;

                var result = await JobsApiClient.WaitForJobAsync(jobId, Cancel);

                Assert.False(result.Success);

                var request = MockHttpClient.AssertSingleRequest();

                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/jobs/{jobId.ToUrlSegment()}");

                MockTaskDelayer.Verify(d => d.DelayAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);

                var resultException = Assert.Single(result.Errors);
                var jobException = Assert.IsType<FailedJobException>(resultException);
            }

            [Fact]
            public async Task Throws_when_canceled()
            {
                using var cancellationTokenSource = new CancellationTokenSource();

                cancellationTokenSource.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => JobsApiClient.WaitForJobAsync(Create<Guid>(), cancellationTokenSource.Token));
            }
        }
    }
}
