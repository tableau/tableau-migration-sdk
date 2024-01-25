using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal sealed class RestUpdateFileUploadResponseBuilder : RestApiResponseBuilderBase<FileUploadResponse>
    {
        public RestUpdateFileUploadResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(data, serializer, true)
        { }

        protected async override ValueTask<(FileUploadResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, CancellationToken cancel)
        {
            var sessionId = request.GetLastSegment();
            if (sessionId is null || !Data.Files.ContainsKey(sessionId))
            {
                return BuildEmptyErrorResponse(
                    HttpStatusCode.NotFound,
                    15,
                    $"File upload not found",
                    "The file upload ID in the URI doesn't correspond to an existing file upload.");
            }

            if (request.Content is not null && (request.Content is MultipartContent multipartContent))
            {
                foreach (var contentPart in multipartContent)
                {
                    //Look for the multipart section that represents the file data.
                    if (contentPart.Headers.ContentType?.MediaType == MediaTypes.OctetStream.MediaType)
                    {
                        var fileContent = await contentPart.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);
                        var fileSize = fileContent.Length;

                        Data.UpdateFile(sessionId, fileContent);

                        return (new FileUploadResponse
                        {
                            Item = new FileUploadResponse.FileUploadType
                            {
                                UploadSessionId = sessionId,
                                FileSize = fileSize,
                            }
                        },
                        HttpStatusCode.Created);
                    }
                }
            }

            return BuildEmptyErrorResponse(
                    HttpStatusCode.BadRequest,
                    20,
                    $"Missing file data",
                    "There is no attachment in the request with the file data to be appended to the upload.");
        }
    }
}
