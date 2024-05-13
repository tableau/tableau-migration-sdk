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
    internal class RestGroupAddResponseBuilder : RestApiResponseBuilderBase<CreateGroupResponse>
    {
        public RestGroupAddResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        private static (CreateGroupResponse Response, HttpStatusCode ResponseCode) GroupNameConflictResult()
         => BuildEmptyErrorResponse(HttpStatusCode.Conflict, 9, "Group name conflict.", "");

        protected override async ValueTask<(CreateGroupResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (request?.Content is null)
                return BuildEmptyErrorResponse(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "");

            var grouRequestString = await request.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);

            var activeDirectoryGroupRequest = grouRequestString?.FromXml<ImportGroupRequest>()?.Group;

            if (activeDirectoryGroupRequest is null)
            {
                return BuildEmptyErrorResponse(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Request must be of the type {nameof(CreateLocalGroupRequest.GroupType)} and not null",
                    "");
            }

            var group = new GroupsResponse.GroupType();

            if (activeDirectoryGroupRequest?.Import is not null)
            {
                if (Data.Groups.Any(g => g.Name == activeDirectoryGroupRequest.Name && g.Domain?.Name == activeDirectoryGroupRequest.Import.DomainName))
                {
                    return GroupNameConflictResult();
                }

                group = new GroupsResponse.GroupType
                {
                    Id = Guid.NewGuid(),
                    Name = activeDirectoryGroupRequest.Name,
                    Domain = new GroupsResponse.GroupType.DomainType
                    {
                        Name = activeDirectoryGroupRequest.Import.DomainName
                    },
                    Import = new GroupsResponse.GroupType.ImportType
                    {
                        DomainName = activeDirectoryGroupRequest.Import.DomainName,
                        SiteRole = activeDirectoryGroupRequest.Import.SiteRole,
                        GrantLicenseMode = activeDirectoryGroupRequest.Import.GrantLicenseMode
                    }
                };
            }
            else
            {
                var localGroupRequest = grouRequestString?.FromXml<CreateLocalGroupRequest>()?.Group;

                if (Data.Groups.Any(g => g.Name == localGroupRequest?.Name && g.Domain?.Name == Constants.LocalDomain))
                {
                    return GroupNameConflictResult();
                }

                group = new GroupsResponse.GroupType
                {
                    Id = Guid.NewGuid(),
                    Name = localGroupRequest?.Name,
                    Domain = new GroupsResponse.GroupType.DomainType
                    {
                        Name = "local"
                    },
                    Import = new GroupsResponse.GroupType.ImportType
                    {
                        DomainName = "local",
                        SiteRole = localGroupRequest?.MinimumSiteRole,
                    }
                };
            }

            Data.AddGroup(group);

            return (new CreateGroupResponse
            {
                Item = new CreateGroupResponse.GroupType
                {
                    Id = group.Id,
                    Name = group.Name,
                    Import = new CreateGroupResponse.GroupType.ImportType
                    {
                        DomainName = group.Import.DomainName,
                        SiteRole = group.Import.SiteRole,
                        GrantLicenseMode = group.Import.GrantLicenseMode
                    }
                }
            },
            HttpStatusCode.Created);
        }
    }
}
