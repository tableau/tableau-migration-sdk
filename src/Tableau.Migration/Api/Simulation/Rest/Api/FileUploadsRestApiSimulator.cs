using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Net.Simulation;
using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API file upload methods.
    /// </summary>
    public sealed class FileUploadsRestApiSimulator
    {
        /// <summary>
        /// Gets the simulated initiate file upload API method.
        /// </summary>
        public MethodSimulator InitiateFileUpload { get; }

        /// <summary>
        /// Gets the simulated update file upload API method.
        /// </summary>
        public MethodSimulator UpdateFileUpload { get; }

        /// <summary>
        /// Creates a new <see cref="FileUploadsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public FileUploadsRestApiSimulator(TableauApiResponseSimulator simulator)
        {
            InitiateFileUpload = simulator.SetupRestPost(
               SiteUrl("fileUploads"),
               new RestInitiateFileUploadResponseBuilder(simulator.Data, simulator.Serializer));

            UpdateFileUpload = simulator.SetupRestPut(
               SiteUrl($"fileUploads/{GuidPattern}"),
               new RestUpdateFileUploadResponseBuilder(simulator.Data, simulator.Serializer));
        }
    }
}
