using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API job methods.
    /// </summary>
    public sealed class JobsRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated job query API method.
        /// </summary>
        public MethodSimulator QueryJob { get; }

        /// <summary>
        /// Creates a new <see cref="JobsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public JobsRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            QueryJob = simulator.SetupRestGetById<JobResponse, JobResponse.JobType>(SiteEntityUrl("jobs"), d => d.Jobs);
        }
    }
}
