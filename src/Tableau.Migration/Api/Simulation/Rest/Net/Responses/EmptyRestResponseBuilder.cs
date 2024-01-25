using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class EmptyRestResponseBuilder : IRestApiResponseBuilder
    {
        private static readonly UnauthorizedRestErrorBuilder _unauthorizedErrorBuilder = new();

        private readonly IHttpContentSerializer _serializer;

        protected TableauData Data { get; }

        public bool RequiresAuthentication { get; }

        public IRestErrorBuilder? ErrorOverride { get; set; }

        public EmptyRestResponseBuilder(TableauData data, IHttpContentSerializer serializer, bool requiresAuthentication)
        {
            Data = data;
            _serializer = serializer;
            RequiresAuthentication = requiresAuthentication;
        }

        protected virtual Task<HttpResponseMessage> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

        protected HttpResponseMessage BuildErrorResponse(HttpRequestMessage request, IRestErrorBuilder errorBuilder)
        {
            var contentType = request.Headers.Accept.FirstOrDefault() ?? MediaTypes.Xml;

            var tsResponse = new EmptyTableauServerResponse
            {
                Error = errorBuilder.BuildError(out var statusCode)
            };

            return new HttpResponseMessage(statusCode)
            {
                Content = _serializer.Serialize(tsResponse, contentType)
            };
        }

        public async Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (ErrorOverride is not null)
            {
                return BuildErrorResponse(request, ErrorOverride);
            }
            else
            {
                if (this.IsUnauthorizedRequest(request, Data))
                {
                    return BuildErrorResponse(request, _unauthorizedErrorBuilder);
                }
            }

            return await BuildResponseAsync(request, cancel).ConfigureAwait(false);
        }
    }
}
