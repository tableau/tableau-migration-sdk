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
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public class JobsApiClientTests
    {
        public abstract class JobsApiClientTest : ApiClientTestBase
        { }

        public class GetJobStatusAsync : JobsApiClientTest
        {
            [Fact]
            public async Task Returns_job_on_success()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var job = Create<JobResponse.JobType>();

                Api.Data.Jobs.Add(job);

                var result = await sitesClient.Jobs.GetJobStatusAsync(job.Id, Cancel);

                Assert.True(result.Success);
                Assert.NotNull(result.Value);
            }

            [Fact]
            public async Task Returns_error_when_not_found()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var result = await sitesClient.Jobs.GetJobStatusAsync(Create<Guid>(), Cancel);

                Assert.False(result.Success);
                Assert.Null(result.Value);

                var error = Assert.Single(result.Errors);
                Assert.IsType<RestException>(error);
            }
        }
    }
}
