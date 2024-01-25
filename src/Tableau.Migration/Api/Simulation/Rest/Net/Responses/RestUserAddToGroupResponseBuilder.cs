using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestUserAddToGroupResponseBuilder : RestApiResponseBuilderBase<AddUserResponse>
    {
        public RestUserAddToGroupResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(AddUserResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (request?.Content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "");

            var addUserRequest = request.GetTableauServerRequest<AddUserToGroupRequest>()?.User;
            var segments = request.RequestUri!.GetNonSlashSegments();

            var groupId = new Guid(segments[5]);

            if (addUserRequest is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Request must be of the type {nameof(AddUserToGroupRequest.UserType)} and not null",
                    "");
            }

            var user = Data.Users.First(u => u.Id == addUserRequest.Id);

            Data.AddUserToGroup(user.Id, groupId);

            return ValueTask.FromResult((new AddUserResponse
            {
                Item = new AddUserResponse.UserType
                {
                    Id = user.Id,
                    Name = user.Name,
                    SiteRole = user.SiteRole
                }
            },
            HttpStatusCode.Created));
        }
    }
}
