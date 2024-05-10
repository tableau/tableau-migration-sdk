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
using System.Collections.Generic;
using System.Linq;
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
    internal class RestUserUpdateResponseBuilder : RestEntityListResponseBuilderBase<UpdateUserResponse, UsersResponse.UserType>
    {
        public RestUserUpdateResponseBuilder(
            TableauData data,
            IHttpContentSerializer serializer,
            Func<TableauData, HttpRequestMessage, ICollection<UsersResponse.UserType>> getEntities)
            : base(data, serializer, getEntities, requiresAuthentication: true)
        { }

        protected static UsersResponse.UserType? UpdateUser(HttpRequestMessage request, ICollection<UsersResponse.UserType> allUsers)
        {
            var oldUser = allUsers.FirstOrDefault(u => u.Id == request.GetRequestIdFromUri());

            if (oldUser is null)
                return null;

            var newUser = request.GetTableauServerRequest<UpdateUserRequest>()?.User;

            if (newUser is null)
                return null;

            var siteRole = SiteRoleMapping.GetSiteRole(
                SiteRoleMapping.GetAdministratorLevel(newUser.SiteRole),
                SiteRoleMapping.GetLicenseLevel(newUser.SiteRole),
                SiteRoleMapping.GetPublishingCapability(newUser.SiteRole));

            if (!string.IsNullOrEmpty(newUser.FullName))
                oldUser.FullName = newUser.FullName;

            if (!string.IsNullOrEmpty(newUser.Email))
                oldUser.Email = newUser.Email;

            if (!string.IsNullOrEmpty(siteRole))
                oldUser.SiteRole = siteRole;

            if (!string.IsNullOrEmpty(newUser.AuthSetting))
                oldUser.AuthSetting = newUser.AuthSetting;

            return oldUser;
        }

        protected override ValueTask<(UpdateUserResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var allUsers = GetEntities(Data, request);
            var updatedUser = UpdateUser(request, allUsers);

            if (updatedUser is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "No user was found to update.", "");

            return ValueTask.FromResult((new UpdateUserResponse
            {
                Item = new UpdateUserResponse.UserType
                {
                    Name = updatedUser.Name,
                    Email = updatedUser.Email,
                    SiteRole = updatedUser.SiteRole,
                    AuthSetting = updatedUser.AuthSetting,
                    FullName = updatedUser.FullName,
                }

            }, HttpStatusCode.OK));
        }
    }
}
