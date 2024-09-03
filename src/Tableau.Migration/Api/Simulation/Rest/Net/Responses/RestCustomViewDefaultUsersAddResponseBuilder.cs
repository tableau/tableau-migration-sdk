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

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;


namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestCustomViewDefaultUsersAddResponseBuilder : RestApiResponseBuilderBase<CustomViewAsUsersDefaultViewResponse>
    {
        public RestCustomViewDefaultUsersAddResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(CustomViewAsUsersDefaultViewResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request,
            CancellationToken cancel)
        {
            if (request?.Content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "");

            var customViewId = request.GetIdAfterSegment(RestUrlPrefixes.CustomViews);
            var usersToAdd = request.GetTableauServerRequest<SetCustomViewDefaultUsersRequest>()?.Users;

            if (usersToAdd is null || customViewId is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Request must be of the type {nameof(SetCustomViewDefaultUsersRequest.UserType)} and not null. CustomView ID should not be null",
                    "");
            }

            var customViewUsers = new List<UsersWithCustomViewAsDefaultViewResponse.UserType>();
            foreach (var user in usersToAdd)
            {
                customViewUsers.Add(new()
                {
                    Id = user.Id
                });
            }

            if (Data.CustomViewDefaultUsers.TryGetValue(
                customViewId.Value,
                out List<UsersWithCustomViewAsDefaultViewResponse.UserType>? value))
            {
                value.AddRange(customViewUsers);
            }
            else
            {
                Data.CustomViewDefaultUsers.TryAdd(customViewId.Value, customViewUsers);
            }


            var resultUsers = new List<CustomViewAsUsersDefaultViewResponse.CustomViewAsUserDefaultViewType>();

            foreach (var user in customViewUsers)
            {
                resultUsers.Add(new()
                {
                    Success = true,
                    User = new()
                    {
                        Id = user.Id
                    }
                });
            }
            return ValueTask.FromResult((new CustomViewAsUsersDefaultViewResponse
            {
                Items = [.. resultUsers]
            },
            HttpStatusCode.Created));
        }
    }
}
