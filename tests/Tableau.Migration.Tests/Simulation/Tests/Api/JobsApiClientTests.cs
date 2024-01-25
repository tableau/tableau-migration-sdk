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
