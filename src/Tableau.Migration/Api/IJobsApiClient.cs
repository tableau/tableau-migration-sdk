using System;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Models;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Interface for API client job operations.
    /// </summary>
    public interface IJobsApiClient : IContentApiClient
    {
        /// <summary>
        /// Gets the status for a given job ID
        /// </summary>
        /// <param name="jobId">The job's ID</param>
        /// <param name="cancel">The cancellation token</param>
        /// <returns>The status for the given job ID</returns>
        Task<IResult<IJob>> GetJobStatusAsync(Guid jobId, CancellationToken cancel);

        /// <summary>
        /// Waits for the job with the given ID to complete.
        /// </summary>
        /// <param name="jobId">The job's ID</param>
        /// <param name="cancel">The cancellation token</param>
        /// <returns>The status for the given job ID</returns>
        Task<IResult> WaitForJobAsync(Guid jobId, CancellationToken cancel);
    }
}
