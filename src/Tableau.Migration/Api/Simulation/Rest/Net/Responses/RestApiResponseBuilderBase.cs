using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    /// <summary>
    /// Abstract base class for REST API style response builders.
    /// </summary>
    internal abstract class RestApiResponseBuilderBase<TResponse> : IRestApiResponseBuilder
        where TResponse : TableauServerResponse, new()
    {
        protected TableauData Data { get; }

        protected IHttpContentSerializer Serializer { get; }

        public bool RequiresAuthentication { get; }

        public IRestErrorBuilder? ErrorOverride { get; set; }

        public RestApiResponseBuilderBase(TableauData data, IHttpContentSerializer serializer, bool requiresAuthentication)
        {
            Data = data;
            Serializer = serializer;
            RequiresAuthentication = requiresAuthentication;
        }

        protected static (TResponse Response, HttpStatusCode ResponseCode) BuildEmptyErrorResponse(HttpStatusCode statusCode, int subCode, string summary, string detail)
        {
            var errorBuilder = new StaticRestErrorBuilder(statusCode, subCode, summary, detail);
            var response = new TResponse()
            {
                Error = errorBuilder.BuildError(out _)
            };

            return (response, statusCode);
        }

        protected UsersResponse.UserType EnsureSignedInUser()
        {
            if (Data.SignIn is null)
            {
                throw new Exception("No signed in user available");
            }

            var currentUser = Data.Users.Where(u => u.Id == Data.SignIn.User!.Id).FirstOrDefault();

            return currentUser is null ? throw new Exception("No signed in user available") : currentUser;
        }

        protected static ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildEmptyErrorResponseAsync(HttpStatusCode statusCode, int subCode, string summary, string detail)
            => ValueTask.FromResult(BuildEmptyErrorResponse(statusCode, subCode, summary, detail));

        protected abstract ValueTask<(TResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel);

        public virtual async Task<HttpResponseMessage> RespondAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            TResponse responseToSerialize;
            HttpStatusCode responseCode;

            if (ErrorOverride is not null)
            {
                responseToSerialize = new()
                {
                    Error = ErrorOverride.BuildError(out responseCode)
                };

                return BuildResponseMessage(responseToSerialize, responseCode);
            }
            else
            {
                if (this.IsUnauthorizedRequest(request, Data))
                {
                    var errorBuilder = new UnauthorizedRestErrorBuilder();
                    var tsResponse = new TResponse()
                    {
                        Error = errorBuilder.BuildError(out _)
                    };

                    return BuildResponseMessage(tsResponse, errorBuilder.StatusCode);
                }

                (responseToSerialize, responseCode) = await BuildResponseAsync(request, cancel).ConfigureAwait(false);

                return BuildResponseMessage(responseToSerialize, responseCode);
            }

            HttpResponseMessage BuildResponseMessage(TResponse responseContent, HttpStatusCode statusCode)
            {
                var response = new HttpResponseMessage(statusCode)
                {
                    Content = Serializer.Serialize(responseContent, request.Headers.Accept.FirstOrDefault() ?? MediaTypes.Xml) ?? new StringContent(string.Empty)
                };

                return response;
            }
        }
    }
}
