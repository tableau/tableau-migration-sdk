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
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class ProjectsApiClient :
        ContentApiClientBase, IProjectsApiClient, IProjectsResponseApiClient
    {
        internal const string PROJECT_NAME_CONFLICT_ERROR_CODE = "409006";

        private readonly IHttpContentSerializer _serializer;
        private readonly IDefaultPermissionsApiClient _defaultPermissionsClient;

        public ProjectsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IPermissionsApiClientFactory permissionsClientFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            IHttpContentSerializer serializer,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _defaultPermissionsClient = permissionsClientFactory.CreateDefaultPermissionsClient();
            _serializer = serializer;

            Permissions = permissionsClientFactory.Create(this);
        }

        /// <inheritdoc />
        public async Task<IResult<IProject>> CreateProjectAsync(ICreateProjectOptions options, CancellationToken cancel)
        {
            var uriBuilder = RestRequestBuilderFactory
                .CreateUri(UrlPrefix);

            if (options.PublishSamples)
                uriBuilder.WithQuery("publishSamples", "true");

            var projectResult = await uriBuilder
                .ForPostRequest()
                .WithXmlContent(new CreateProjectRequest(options))
                .SendAsync<CreateProjectResponse>(cancel)
                .ToResultAsync(async (r, c) =>
                {
                    var response = Guard.AgainstNull(r.Item, () => r.Item);
                    var project = await RestProjectBuilder.BuildProjectAsync(response, options.ParentProject, ContentFinderFactory.ForContentType<IUser>(), c)
                        .ConfigureAwait(false);
                    return project;
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return projectResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IUpdateProjectResult>> UpdateProjectAsync(
            Guid projectId,
            CancellationToken cancel,
            string? newName = null,
            string? newDescription = null,
            Guid? newParentProjectId = null,
            string? newContentPermissions = null,
            Guid? newControllingPermissionsProjectId = null,
            Guid? newOwnerId = null)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{projectId.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(
                    new UpdateProjectRequest(
                        newName,
                        newDescription,
                        newParentProjectId,
                        newContentPermissions,
                        newControllingPermissionsProjectId,
                        newOwnerId))
                .SendAsync<UpdateProjectResponse>(cancel)
                .ToResultAsync<UpdateProjectResponse, IUpdateProjectResult>(r => new UpdateProjectResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }

        // <inheritdoc />
        public async Task<IResult> DeleteProjectAsync(Guid projectId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{projectId.ToUrlSegment()}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        #region - IPermissionsContentApiClientImplementation -

        /// <inheritdoc />
        public IPermissionsApiClient Permissions { get; }

        #endregion

        #region - IProjectsResponseApiClient Implementation -

        IPager<ProjectsResponse.ProjectType> IProjectsResponseApiClient.GetPager(int pageSize)
            => new RestProjectResponsePager(this, pageSize);

        /// <inheritdoc />
        public async Task<IPagedResult<ProjectsResponse.ProjectType>> GetAllProjectsAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllProjectsAsync(pageNumber, pageSize, Enumerable.Empty<Filter>(), cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IPagedResult<ProjectsResponse.ProjectType>> GetAllProjectsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, CancellationToken cancel)
        {
            var getAllProjectsResult = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .WithPage(pageNumber, pageSize)
                .WithFilters(filters)
                .ForGetRequest()
                .SendAsync<ProjectsResponse>(cancel)
                .ToPagedResultAsync(r => r.Items.ToImmutableArray(), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getAllProjectsResult;
        }

        #endregion

        #region - IListApiClient<IProject> Implementation -

        public IPager<IProject> GetPager(int pageSize)
            => new BreadthFirstPathHierarchyPager<IProject>(new RestProjectBuilderPager(this, ContentFinderFactory.ForContentType<IUser>(), pageSize), pageSize);

        #endregion

        #region - IPublishApiClient<IProject> Implementation -

        public async Task<IResult<IProject>> PublishAsync(IProject item, CancellationToken cancel)
        {
            var options = new CreateProjectOptions(
                item.ParentProject,
                item.Name,
                item.Description,
                item.ContentPermissions,
                false);

            var projectResult = await CreateProjectAsync(options, cancel).ConfigureAwait(false);

            if (projectResult.Success
                || !projectResult.Errors.OfType<RestException>().Any(e => e.Code == PROJECT_NAME_CONFLICT_ERROR_CODE))
            {
                return projectResult;
            }

            // If there's a conflict find the existing project.
            var filters = new List<Filter>
            {
                new Filter("name", FilterOperator.Equal, options.Name)
            };

            if (options.ParentProject is not null)
            {
                filters.Add(new Filter("parentProjectId", FilterOperator.Equal, options.ParentProject.Id.ToString()));
            }
            else
            {
                filters.Add(new Filter("topLevelProject", FilterOperator.Equal, "true"));
            }

            // We grab two items here so we'll know if we match > 1.
            // This theoretically shouldn't happen since we're filtering on name and project ID, but just in case.
            var existingProjectResult = await GetAllProjectsAsync(1, 2, filters, cancel).ConfigureAwait(false);

            if (existingProjectResult.Success && existingProjectResult.Value.Count == 1)
            {
                // We don't need to load the parent project here since it's already published.
                // The owner may be different, though, which we'll want to know for the post-publish hook.

                var existingProject = await RestProjectBuilder.BuildProjectAsync(existingProjectResult.Value[0],
                    item.ParentProject, ContentFinderFactory.ForContentType<IUser>(), cancel).ConfigureAwait(false);

                return Result<IProject>.Succeeded(existingProject);
            }

            var conflictResultBuilder = new ResultBuilder();
            conflictResultBuilder.Add(projectResult);

            if (!existingProjectResult.Success)
            {
                conflictResultBuilder.Add(existingProjectResult);
            }
            else if (existingProjectResult.Value.Count == 0)
            {
                conflictResultBuilder.Add(
                    new Exception($@"Could not find a project with the name ""{options.Name}"" and parent project ID {options.ParentProject?.Id.ToString() ?? "<null>"}."));
            }
            else if (existingProjectResult.Value.Count > 1)
            {
                conflictResultBuilder.Add(
                    new Exception($@"Found multiple projects with the name ""{options.Name}"" and parent project ID {options.ParentProject?.Id.ToString() ?? "<null>"}."));
            }

            return conflictResultBuilder.Build().CastFailure<IProject>();
        }

        #endregion

        #region - IOwnershipApiClient Implementation -

        public async Task<IResult> ChangeOwnerAsync(Guid contentItemId, Guid newOwnerId, CancellationToken cancel)
        {
            var result = await UpdateProjectAsync(contentItemId, cancel, newOwnerId: newOwnerId)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - Default Permissions -

        public async Task<IResult<IImmutableDictionary<string, IPermissions>>> GetAllDefaultPermissionsAsync(
            Guid projectId,
            CancellationToken cancel)
        {
            return await _defaultPermissionsClient.GetAllPermissionsAsync(
                projectId,
                cancel)
                .ConfigureAwait(false);
        }

        public async Task<IResult<IImmutableDictionary<string, IPermissions>>> UpdateAllDefaultPermissionsAsync(
            Guid projectId,
            IReadOnlyDictionary<string, IPermissions> permissions,
            CancellationToken cancel)
        {
            return await _defaultPermissionsClient.UpdateAllPermissionsAsync(
                projectId,
                permissions,
                cancel)
                .ConfigureAwait(false);
        }

        #endregion
    }
}
