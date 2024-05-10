//
//  Copyright (c) 2024, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the "License") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models;
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
            var siteRole = SiteRoleMapping.GetSiteRole(
                SiteRoleMapping.GetAdministratorLevel(addUserRequest?.SiteRole), 
                SiteRoleMapping.GetLicenseLevel(addUserRequest?.SiteRole), 
                SiteRoleMapping.GetPublishingCapability(addUserRequest?.SiteRole));
            var user = new UsersResponse.UserType()
            {
                Id = Guid.NewGuid(),
                Name = addUserRequest?.Name,
                AuthSetting = addUserRequest?.AuthSetting,
                SiteRole = siteRole,
                Domain = TableauData.GetUserDomain(addUserRequest?.Name) ?? new() { Name = Data.DefaultDomain }
            };

            Data.AddUser(user);

            return ValueTask.FromResult((new AddUserResponse
            {
                Item = new AddUserResponse.UserType
                {
                    Id = user.Id,
                    AuthSetting = user.AuthSetting,
                    Name = user.Name,
                    SiteRole = siteRole
                }
            },
            HttpStatusCode.Created));
        }
    }
}
