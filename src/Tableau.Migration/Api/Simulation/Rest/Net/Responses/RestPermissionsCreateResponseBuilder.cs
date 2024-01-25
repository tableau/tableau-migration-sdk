using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestPermissionsCreateResponseBuilder<TContent> : RestApiResponseBuilderBase<PermissionsResponse>
        where TContent : IRestIdentifiable, INamedContent
    {
        private readonly string _contentTypeUrlPrefix;
        private readonly Func<TableauData, ICollection<TContent>> _getContent;

        public RestPermissionsCreateResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            string contentTypeUrlPrefix,
            Func<TableauData, ICollection<TContent>> getContent)
            : base(data, serializer, requiresAuthentication: true)
        {
            _contentTypeUrlPrefix = contentTypeUrlPrefix.ToLower();
            _getContent = getContent;
        }

        private static bool IsProjectLeaderDenyCapability(CapabilityType capabilityType)
            => PermissionsCapabilityNames.IsAMatch(PermissionsCapabilityNames.ProjectLeader, capabilityType.Name)
            && PermissionsCapabilityModes.IsAMatch(PermissionsCapabilityModes.Deny, capabilityType.Mode);

        private static bool HasProjectLeaderDenyCapability(PermissionsType permissionRequest)
        {
            if (permissionRequest.GranteeCapabilities is not null)
            {
                foreach (var grantee in permissionRequest.GranteeCapabilities)
                {
                    if (grantee.Capabilities.Any(IsProjectLeaderDenyCapability))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

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

            var contentId = request.GetIdAfterSegment(_contentTypeUrlPrefix);
            if (contentId is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "URL content item's ID cannot be null.", "");

            var content = _getContent(Data).SingleOrDefault(d => d.Id == contentId);
            if (content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 0, $"The content with ID {contentId} could not be found.", "");

            if (HasProjectLeaderDenyCapability(createPermissionRequest))
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 9, "Invalid capability", "The capability 'ProjectLeader Deny' is invalid for a project");

            Data.AddContentTypePermissions(_contentTypeUrlPrefix, content.Id, createPermissionRequest);

            return ValueTask.FromResult((new PermissionsResponse
            {
                Parent = new ParentContentType
                {
                    Id = content.Id
                },
                Item = createPermissionRequest
            },
            HttpStatusCode.OK));
        }
    }
}
