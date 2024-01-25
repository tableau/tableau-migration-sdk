using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestDefaultPermissionsCreateResponseBuilder : RestApiResponseBuilderBase<PermissionsResponse>
    {
        private static readonly string UrlPrefix = RestUrlPrefixes.Projects;

        public RestDefaultPermissionsCreateResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(PermissionsResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request, CancellationToken cancel)
        {
            if (request?.Content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "");

            var createPermissionRequest = request.GetTableauServerRequest<PermissionsAddRequest>()?.Permissions;

            if (createPermissionRequest is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Request must be of the type {nameof(PermissionsType)} and not null",
                    "");
            }

            var contentId = request.GetIdAfterSegment(UrlPrefix);

            if (contentId is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "URL content item's ID cannot be null.", "");

            var contentType = request.ParseDefaultPermissionsContentType();
            Data.AddDefaultProjectPermissions(contentId.Value, contentType, createPermissionRequest);

            return ValueTask.FromResult((new PermissionsResponse
            {
                Item = createPermissionRequest
            },
            HttpStatusCode.OK));
        }
    }
}
