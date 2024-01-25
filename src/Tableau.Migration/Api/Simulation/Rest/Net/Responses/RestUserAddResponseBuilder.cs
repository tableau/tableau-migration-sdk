using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestUserAddResponseBuilder : RestApiResponseBuilderBase<AddUserResponse>
    {
        public RestUserAddResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(AddUserResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (request?.Content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "");

            var addUserRequest = request.GetTableauServerRequest<AddUserToSiteRequest>()?.User;

            if (addUserRequest is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Request must be of the type {nameof(AddUserToSiteRequest.UserType)} and not null",
                    "");
            }

            var user = new UsersResponse.UserType()
            {
                Id = Guid.NewGuid(),
                Name = addUserRequest?.Name,
                AuthSetting = addUserRequest?.AuthSetting,
                SiteRole = addUserRequest?.SiteRole
            };

            Data.AddUser(user);

            return ValueTask.FromResult((new AddUserResponse
            {
                Item = new AddUserResponse.UserType
                {
                    Id = user.Id,
                    AuthSetting = user.AuthSetting,
                    Name = user.Name,
                    SiteRole = user.SiteRole
                }
            },
            HttpStatusCode.Created));
        }
    }
}
