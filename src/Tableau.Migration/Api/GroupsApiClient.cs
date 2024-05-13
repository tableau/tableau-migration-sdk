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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Net.Rest.Filtering;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    internal sealed class GroupsApiClient : ContentApiClientBase, IGroupsApiClient
    {
        internal const string GROUP_NAME_CONFLICT_ERROR_CODE = "409009";

        private readonly IHttpContentSerializer _serializer;
        private readonly IConfigReader _configReader;

        public GroupsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IHttpContentSerializer serializer,
            IConfigReader configReader)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _serializer = serializer;
            _configReader = configReader;
        }

        /// <inheritdoc />
        public async Task<IResult<IGroup>> CreateLocalGroupAsync(string name, string? minimumSiteRole, CancellationToken cancel)
        {
            var groupResult = await RestRequestBuilderFactory
                .CreateUri("/groups")
                .ForPostRequest()
                .WithXmlContent(new CreateLocalGroupRequest(name, minimumSiteRole))
                .SendAsync<CreateGroupResponse>(cancel)
                .ToResultAsync<CreateGroupResponse, IGroup>(r => new Group(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return groupResult;
        }

        public async Task<IResult<IGroup>> ImportGroupFromActiveDirectoryAsync(
            string name,
            string domainName,
            string minimumSiteRole,
            string? grantLicenseMode,
            CancellationToken cancel)
        {
            var jobResult = await RestRequestBuilderFactory
                .CreateUri("/groups")
                .ForPostRequest()
                .WithXmlContent(new ImportGroupRequest(name, ImportGroupRequest.ActiveDirectorySource, domainName, minimumSiteRole, grantLicenseMode))
                .SendAsync<CreateGroupResponse>(cancel)
                .ToResultAsync<CreateGroupResponse, IGroup>(r => new Group(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return jobResult;
        }

        public async Task<IResult<IImportJob>> ImportGroupFromActiveDirectoryBackgroundProcessAsync(
            string name,
            string domainName,
            string minimumSiteRole,
            string? grantLicenseMode,
            CancellationToken cancel)
        {
            var jobResult = await RestRequestBuilderFactory
                .CreateUri("/groups")
                .WithQuery("asJob", "true")
                .ForPostRequest()
                .WithXmlContent(new ImportGroupRequest(name, ImportGroupRequest.ActiveDirectorySource, domainName, minimumSiteRole, grantLicenseMode))
                .SendAsync<ImportJobResponse>(cancel)
                .ToResultAsync<ImportJobResponse, IImportJob>(r => new ImportJob(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return jobResult;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<IGroup>> GetAllGroupsAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllGroupsAsync(pageNumber, pageSize, Enumerable.Empty<Filter>(), cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IPagedResult<IGroup>> GetAllGroupsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, CancellationToken cancel)
        {
            var getAllGroupsResult = await RestRequestBuilderFactory
                .CreateUri("/groups")
                .WithPage(pageNumber, pageSize)
                .WithFilters(filters)
                .ForGetRequest()
                .SendAsync<GroupsResponse>(cancel)
                .ToPagedResultAsync(r => r.GetGroupsFromResponse(), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getAllGroupsResult;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<IUser>> GetGroupUsersAsync(Guid groupId, int pageNumber, int pageSize, CancellationToken cancel)
        {
            var getGroupUsersResult = await RestRequestBuilderFactory
                .CreateUri($"/groups/{groupId.ToUrlSegment()}/users")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<UsersResponse>(cancel)
                .ToPagedResultAsync(r => r.GetUsersFromResponse(), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getGroupUsersResult;
        }

        /// <inheritdoc />
        public async Task<IResult<IAddUserToGroupResult>> AddUserToGroupAsync(Guid groupId, Guid userId, CancellationToken cancel)
        {
            var userResult = await RestRequestBuilderFactory
                .CreateUri($"/groups/{groupId}/users")
                .ForPostRequest()
                .WithXmlContent(new AddUserToGroupRequest(userId))
                .SendAsync<AddUserResponse>(cancel)
                .ToResultAsync<AddUserResponse, IAddUserToGroupResult>(r => new AddUserToGroupResult(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return userResult;
        }

        /// <inheritdoc />
        public async Task<IResult> RemoveUserFromGroupAsync(Guid groupId, Guid userId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"/groups/{groupId}/users/{userId}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult> DeleteGroupAsync(Guid groupId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"/groups/{groupId.ToUrlSegment()}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        #region - IPagedListApiClient<IGroup> Implementation -

        public IPager<IGroup> GetPager(int pageSize) => new ApiListPager<IGroup>(this, pageSize);

        #endregion - IPagedListApiClient<IGroup> Implementation -

        #region - IApiPageAccessor<IGroup> Implementation -

        public async Task<IPagedResult<IGroup>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllGroupsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion - IApiPageAccessor<IGroup> Implementation -

        #region - IPublishApiClient<IGroupWithUsers> Implementation -

        private async Task<IResult<IGroup>> FixupUniqueGroupErrorAsync(IPublishableGroup item, IResult<IGroup> publishResult, CancellationToken cancel)
        {
            if (publishResult.Success
                || !publishResult.Errors.OfType<RestException>().Any(e => e.Code == GROUP_NAME_CONFLICT_ERROR_CODE))
            {
                return publishResult;
            }

            // If there's a conflict find the existing project.
            var filters = ImmutableArray.Create<Filter>(
                new Filter("domainName", FilterOperator.Equal, item.Domain),
                new Filter("name", FilterOperator.Equal, item.Name)
            );

            // We grab two items here so we'll know if we match > 1.
            // This theoretically shouldn't happen since we're filtering on the same criteria as the
            // name uniqueness, but just in case.
            var existingResult = await GetAllGroupsAsync(1, 2, filters, cancel).ConfigureAwait(false);

            if (existingResult.Success && existingResult.Value.Count == 1)
            {
                //We don't need to load the parent project here since it's already published.
                return Result<IGroup>.Succeeded(existingResult.Value[0]);
            }

            var conflictResultBuilder = new ResultBuilder();
            conflictResultBuilder.Add(publishResult);

            if (!existingResult.Success)
            {
                conflictResultBuilder.Add(existingResult);
            }
            else if (existingResult.Value.Count == 0)
            {
                conflictResultBuilder.Add(
                new Exception($@"Could not find a group with the name ""{item.Location}""."));
            }
            else if (existingResult.Value.Count > 1)
            {
                conflictResultBuilder.Add(
                new Exception($@"Found multiple groups with the name ""{item.Location}""."));
            }
            return conflictResultBuilder.Build().CastFailure<IGroup>();
        }

        public async Task<IResult<IGroup>> PublishAsync(IPublishableGroup item, CancellationToken cancel)
        {
            IResult<IGroup> groupResult;
            if (string.Equals(item.Domain, Constants.LocalDomain, Group.NameComparison))
            {
                groupResult = await CreateLocalGroupAsync(item.Name, item.SiteRole, cancel).ConfigureAwait(false);
            }
            else
            {
                if (item.SiteRole is null)
                {
                    return Result<IGroup>.Failed(new Exception("Missing site role name for Active Directory group"));
                }

                groupResult = await ImportGroupFromActiveDirectoryAsync(item.Name, item.Domain, item.SiteRole, item.GrantLicenseMode, cancel).ConfigureAwait(false);
            }

            groupResult = await FixupUniqueGroupErrorAsync(item, groupResult, cancel).ConfigureAwait(false);
            if (!groupResult.Success)
            {
                return groupResult;
            }

            //Update group user membership.
            var destinationGroupId = groupResult.Value.Id;
            var destinationGroupUsersResult = await PullAsync(groupResult.Value, cancel).ConfigureAwait(false);
            if (!destinationGroupUsersResult.Success)
            {
                return destinationGroupUsersResult.CastFailure<IGroup>();
            }

            var resultBuilder = new ResultBuilder();
            var destinationUserIds = destinationGroupUsersResult.Value.Users.Select(u => u.User.Id).ToImmutableHashSet();
            var targetUserIds = item.Users.Select(u => u.User.Id).ToImmutableHashSet();
            foreach (var destinationUserId in destinationUserIds)
            {
                cancel.ThrowIfCancellationRequested();
                if (targetUserIds.Contains(destinationUserId))
                {
                    continue;
                }

                var addUserResult = await RemoveUserFromGroupAsync(destinationGroupId, destinationUserId, cancel)
                    .ConfigureAwait(false);

                resultBuilder.Add(addUserResult);
            }

            foreach (var targetUserId in targetUserIds)
            {
                cancel.ThrowIfCancellationRequested();
                if (destinationUserIds.Contains(targetUserId))
                {
                    continue;
                }

                var addUserResult = await AddUserToGroupAsync(destinationGroupId, targetUserId, cancel)
                    .ConfigureAwait(false);

                resultBuilder.Add(addUserResult);
            }

            return Result<IGroup>.Create(resultBuilder.Build(), groupResult.Value);
        }

        #endregion - IPublishApiClient<IGroupWithUsers> Implementation -

        #region - IPullApiClient<IGroup, IGroupWithUsers> Implementation -

        public async Task<IResult<IPublishableGroup>> PullAsync(IGroup contentItem, CancellationToken cancel)
        {
            IPager<IUser> pager = new GroupUsersResponsePager(this, contentItem.Id, _configReader.Get<IUser>().BatchSize);

            var result = await pager.GetAllPagesAsync(cancel)
                .ConfigureAwait(false);

            if (!result.Success)
            {
                return result.CastFailure<IPublishableGroup>();
            }

            var groupUsers = result.Value.Select(user => new GroupUser(user));
            return Result<IPublishableGroup>.Succeeded(new PublishableGroup(contentItem, groupUsers));
        }

        #endregion - IPullApiClient<IGroup, IGroupWithUsers> Implementation -
    }
}
