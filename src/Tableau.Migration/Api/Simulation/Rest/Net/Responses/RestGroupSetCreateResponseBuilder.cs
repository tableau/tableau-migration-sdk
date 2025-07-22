//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestGroupSetCreateResponseBuilder : RestResponseBuilderBase<CreateGroupSetResponse>
    {
        public RestGroupSetCreateResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override async ValueTask<(CreateGroupSetResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(
            HttpRequestMessage request,
            CancellationToken cancel)
        {
            if (request?.Content is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "").Result;
            }

            var createRequest = await Serializer.DeserializeAsync<CreateGroupSetRequest>(request.Content, cancel).ConfigureAwait(false);
            if (createRequest?.GroupSet?.Name is null)
            {
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Group set name is required.", "").Result;
            }

            // Check for duplicate name
            var existingGroupSet = Data.GroupSets.FirstOrDefault(gs =>
                string.Equals(gs.Name, createRequest.GroupSet.Name, StringComparison.OrdinalIgnoreCase));

            if (existingGroupSet is not null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.Conflict, 120, "Group set already exists.", "").Result;

            var newGroupSet = new GroupSetsResponse.GroupSetType
            {
                Id = Guid.NewGuid(),
                Name = createRequest.GroupSet.Name
            };

            Data.AddGroupSet(newGroupSet);

            var response = new CreateGroupSetResponse
            {
                Item = new CreateGroupSetResponse.GroupSetType
                {
                    Id = newGroupSet.Id,
                    Name = newGroupSet.Name
                }
            };

            return (response, HttpStatusCode.Created);
        }
    }
}