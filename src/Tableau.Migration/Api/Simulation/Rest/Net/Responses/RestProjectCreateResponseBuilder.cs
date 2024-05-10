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
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Net;

namespace Tableau.Migration.Api.Simulation.Rest.Net.Responses
{
    internal class RestProjectCreateResponseBuilder : RestApiResponseBuilderBase<CreateProjectResponse>
    {
        public RestProjectCreateResponseBuilder(TableauData data, IHttpContentSerializer serializer)
            : base(data, serializer, requiresAuthentication: true)
        { }

        protected override ValueTask<(CreateProjectResponse Response, HttpStatusCode ResponseCode)> BuildResponseAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            if (request?.Content is null)
                return BuildEmptyErrorResponseAsync(HttpStatusCode.BadRequest, 0, "Request or content cannot be null.", "");

            var createProjectRequest = request.GetTableauServerRequest<CreateProjectRequest>()?.Project;

            if (createProjectRequest is null)
            {
                return BuildEmptyErrorResponseAsync(
                    HttpStatusCode.BadRequest,
                    0,
                    $"Request must be of the type {nameof(CreateProjectRequest.ProjectType)} and not null",
                    "");
            }

            if (Data.Projects.Any(p => p.Name == createProjectRequest.Name && p.ParentProjectId == createProjectRequest.ParentProjectId))
                return BuildEmptyErrorResponseAsync(HttpStatusCode.Conflict, 6, "Project name conflict.", "");

            var parentProjectId = createProjectRequest.GetParentProjectId();

            if (parentProjectId is not null && !Data.Projects.Any(p => p.Id == parentProjectId))
                return BuildEmptyErrorResponseAsync(HttpStatusCode.NotFound, 0, $"Parent project with ID {parentProjectId} does not exist.", "");

            var currentUser = EnsureSignedInUser();

            var project = new ProjectsResponse.ProjectType
            {
                Id = Guid.NewGuid(),
                Name = createProjectRequest.Name,
                Description = createProjectRequest.Description,
                ContentPermissions = createProjectRequest.ContentPermissions,
                ParentProjectId = createProjectRequest.ParentProjectId,
                Owner = new()
                {
                    Id = currentUser.Id
                },
            };

            Data.AddProject(project);

            return ValueTask.FromResult((new CreateProjectResponse
            {
                Item = new CreateProjectResponse.ProjectType
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    ContentPermissions = project.ContentPermissions,
                    ParentProjectId = project.ParentProjectId,
                    Owner = new()
                    {
                        Id = project.Owner.Id
                    }
                }
            },
            HttpStatusCode.Created));
        }
    }
}
