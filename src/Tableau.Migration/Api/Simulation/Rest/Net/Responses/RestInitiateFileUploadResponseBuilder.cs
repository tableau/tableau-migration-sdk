using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestInitiateFileUploadResponseBuilder : RestApiResponseBuilderBase<FileUploadResponse>
    {
        public RestInitiateFileUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(data, serializer, true)
        { }

        protected override ValueTask<(FileUploadResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, CancellationToken cancel)
        {
            var sessionId = Guid.NewGuid().ToString();
            var fileSize = 0;

            // Initiate Tableau Data with a 0 file size to indicate the upload request was initiated.
            Data.UpdateFile(sessionId, Array.Empty<byte>());

            return ValueTask.FromResult((new FileUploadResponse
            {
                Item = new FileUploadResponse.FileUploadType
                {
                    UploadSessionId = sessionId,
                    FileSize = fileSize,
                }
            },
            HttpStatusCode.Created));
        }
    }
}
