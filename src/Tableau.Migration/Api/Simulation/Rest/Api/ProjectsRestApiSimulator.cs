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
using System.Net.Http;
using System.Text.RegularExpressions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Rest.Models.Types;
using Tableau.Migration.Api.Simulation.Rest.Net;
using Tableau.Migration.Api.Simulation.Rest.Net.Requests;
using Tableau.Migration.Api.Simulation.Rest.Net.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Net.Simulation;

using static Tableau.Migration.Api.Simulation.Rest.Net.Requests.RestUrlPatterns;

namespace Tableau.Migration.Api.Simulation.Rest.Api
{
    /// <summary>
    /// Object that defines simulation of Tableau REST API project methods.
    /// </summary>
    public sealed class ProjectsRestApiSimulator : PermissionsRestApiSimulatorBase<ProjectsResponse.ProjectType>
    {
        private static readonly Regex _defaultProjectPermissionsRegex = SiteEntityUrl(RestUrlPrefixes.Projects, @"default-permissions/[\w\d-]+$");

        /// <summary>
        /// Gets the simulated project create API method.
        /// </summary>
        public MethodSimulator CreateProject { get; }

        /// <summary>
        /// Gets the simulated project query API method.
        /// </summary>
        public MethodSimulator QueryProjects { get; }

        /// <summary>
        /// Gets the simulated project default permission create API method.
        /// </summary>
        public MethodSimulator CreateDefaultProjectPermissions { get; }

        /// <summary>
        /// Gets the simulated project default permission query API method.
        /// </summary>
        public MethodSimulator QueryDefaultProjectPermissions { get; }

        /// <summary>
        /// Gets the simulated update project API method.
        /// </summary>
        public MethodSimulator UpdateProject { get; }

        /// <summary>
        /// Creates a new <see cref="ProjectsRestApiSimulator"/> object.
        /// </summary>
        /// <param name="simulator">A response simulator to setup with REST API methods.</param>
        public ProjectsRestApiSimulator(TableauApiResponseSimulator simulator)
            : base(
                  simulator,
                  RestUrlPrefixes.Projects,
                  (data) => data.Projects)
        {
            CreateProject = simulator.SetupRestPost<CreateProjectResponse, CreateProjectResponse.ProjectType>(
                SiteUrl(ContentTypeUrlPrefix),
                new RestProjectCreateResponseBuilder(simulator.Data, simulator.Serializer));

            QueryProjects = simulator.SetupRestPagedList<ProjectsResponse, ProjectsResponse.ProjectType>(
                SiteUrl(ContentTypeUrlPrefix),
                (data, request) =>
                {
                    var filters = request.ParseFilters();

                    if (filters.Count == 0)
                        return data.Projects;

                    var results = data.Projects.AsEnumerable();

                    var nameFilter = filters.GetFilterValue("name", "eq");

                    if (nameFilter is not null)
                    {
                        results = results.Where(p => Project.NameComparer.Equals(nameFilter, p.Name));
                    }

                    var parentProjectIdFilter = filters.GetFilterValue("parentProjectId", "eq");

                    if (parentProjectIdFilter is not null)
                    {
                        results = results.Where(p => p.ParentProjectId == parentProjectIdFilter.ToString());
                    }

                    var topLevelProjectFilter = filters.GetFilterValue("topLevelProject", "eq");

                    if (topLevelProjectFilter is not null)
                    {
                        var topLevel = topLevelProjectFilter == "true";

                        results = results.Where(p => topLevel ? p.ParentProjectId is null : p.ParentProjectId is not null);
                    }

                    return results.ToList();
                });

            CreateDefaultProjectPermissions = simulator.SetupRestPut(
                _defaultProjectPermissionsRegex,
                new RestDefaultPermissionsCreateResponseBuilder(simulator.Data, simulator.Serializer));

            QueryDefaultProjectPermissions = simulator.SetupRestGet<PermissionsResponse, PermissionsType>(
                _defaultProjectPermissionsRegex,
                (data, request) =>
                {
                    var projectId = request.GetProjectIdFromUrl();

                    if (projectId is null)
                        return null;

                    var contentType = request.ParseDefaultPermissionsContentType();

                    if (data.DefaultProjectPermissions.TryGetValue(projectId.Value, out var permissions))
                    {
                        if (permissions.TryGetValue(contentType, out var contentTypePermissions))
                            return contentTypePermissions;
                    }

                    return new PermissionsType
                    {
                        GranteeCapabilities = Array.Empty<GranteeCapabilityType>()
                    };
                });

            UpdateProject = simulator.SetupRestPut<UpdateProjectResponse, UpdateProjectResponse.ProjectType>(
                SiteEntityUrl(ContentTypeUrlPrefix), UpdateProjectFromRequest);
        }

        private UpdateProjectResponse.ProjectType? UpdateProjectFromRequest(TableauData data, HttpRequestMessage request)
        {
            var projectId = request.GetIdAfterSegment(ContentTypeUrlPrefix);
            if (projectId is null)
            {
                return null;
            }

            var project = data.Projects.FirstOrDefault(ds => ds.Id == projectId.Value);
            if (project is null)
            {
                return null;
            }

            var updateProjectRequest = request.GetTableauServerRequest<UpdateProjectRequest>()?.Project;
            if (updateProjectRequest is null)
            {
                return null;
            }

            if (updateProjectRequest.Name is not null)
            {
                project.Name = updateProjectRequest.Name;
            }

            if (updateProjectRequest.Description is not null)
            {
                project.Description = updateProjectRequest.Description;
            }

            if (updateProjectRequest.ParentProjectId is not null)
            {
                if (updateProjectRequest.ParentProjectId == string.Empty)
                {
                    project.ParentProjectId = null;
                }
                else
                {
                    project.ParentProjectId = updateProjectRequest.ParentProjectId;
                }
            }

            if (updateProjectRequest.ContentPermissions is not null)
            {
                project.ContentPermissions = updateProjectRequest.ContentPermissions;
            }

            if (updateProjectRequest.ControllingPermissionsProjectId is not null)
            {
                if(updateProjectRequest.ControllingPermissionsProjectId == string.Empty)
                {
                    project.ControllingPermissionsProjectId = null;
                }
                else
                {
                    project.ControllingPermissionsProjectId = updateProjectRequest.ControllingPermissionsProjectId;
                }
            }

            if (updateProjectRequest.Owner is not null)
            {
                project.Owner!.Id = updateProjectRequest.Owner.Id;
            }

            return new UpdateProjectResponse.ProjectType
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                ParentProjectId = project.ParentProjectId,
                ContentPermissions = project.ContentPermissions,
                ControllingPermissionsProjectId = project.ControllingPermissionsProjectId,
                Owner = new() { Id = project.Owner!.Id }
            };
        }
    }
}
