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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
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
    internal sealed class GroupSetsApiClient : ContentApiClientBase, IGroupSetsApiClient
    {
        private readonly IHttpContentSerializer _serializer;
        private readonly IConfigReader _configReader;

        public GroupSetsApiClient(IRestRequestBuilderFactory restRequestBuilderFactory,
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

        #region - API Bindings -

        public async Task<IPagedResult<IGroupSet>> ListGroupSetsAsync(int pageNumber, int pageSize, IEnumerable<Filter> filters, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .WithPage(pageNumber, pageSize)
                .WithFilters(filters)
                .ForGetRequest()
                .SendAsync<GroupSetsResponse>(cancel)
                .ToPagedResultAsync(r => r.Items.Select(i => (IGroupSet)new GroupSet(i)).ToImmutableArray(), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<IResult<IPublishableGroupSet>> GetGroupSetAsync(Guid id, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{id.ToUrlSegment()}")
                .ForGetRequest()
                .SendAsync<GroupSetResponse>(cancel)
                .ToResultAsync(async (r, c) =>
                {
                    var groupSet = Guard.AgainstNull(r.Item, () => r.Item);

                    var groups = groupSet.Groups ?? Array.Empty<GroupSetResponse.GroupSetType.GroupType>();
                    var groupReferences = new List<IContentReference>(groups.Length);
                    foreach (var responseGroup in groups)
                    {
                        var group = await FindGroupAsync(responseGroup, throwIfNotFound: true, cancel).ConfigureAwait(false);
                        groupReferences.Add(group);
                    }

                    return (IPublishableGroupSet)new PublishableGroupSet(groupSet, groupReferences);
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        public async Task<IResult<IGroupSet>> CreateGroupSetAsync(string name, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .ForPostRequest()
                .WithXmlContent(new CreateGroupSetRequest(name))
                .SendAsync<CreateGroupSetResponse>(cancel)
                .ToResultAsync<CreateGroupSetResponse, IGroupSet>(r =>
                {
                    var groupSet = Guard.AgainstNull(r.Item, () => r.Item);
                    return new GroupSet(groupSet);
                }, SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - IGroupSetsApiClient Implementation -

        /// <inheritdoc />
        public async Task<IResult> AddGroupToGroupSetAsync(Guid groupSetId, Guid groupId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{groupSetId}/{RestUrlKeywords.Groups}/{groupId}")
                .ForPutRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult> RemoveGroupFromGroupSetAsync(Guid groupSetId, Guid groupId, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{groupSetId}/{RestUrlKeywords.Groups}/{groupId}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        #endregion

        #region - IPagedListApiClient<IGroupSet> Implementation -

        /// <inheritdoc />
        public IPager<IGroupSet> GetPager(int pageSize) => new ApiListPager<IGroupSet>(this, pageSize);

        #endregion

        #region - IApiPageAccessor<IGroupSet> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IGroupSet>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await ListGroupSetsAsync(pageNumber, pageSize, Enumerable.Empty<Filter>(), cancel).ConfigureAwait(false);

        #endregion

        #region - IReadApiClient<IGroupSet> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IGroupSet>> GetByIdAsync(Guid id, CancellationToken cancel)
            => (await GetGroupSetAsync(id, cancel).ConfigureAwait(false)).Cast<IGroupSet>();

        #endregion

        #region - IPullApiClient<IGroupSet, IPublishableGroupSet> Implementation -

        /// <inheritdoc />
        public async Task<IResult<IPublishableGroupSet>> PullAsync(IGroupSet contentItem, CancellationToken cancel)
            => await GetGroupSetAsync(contentItem.Id, cancel).ConfigureAwait(false);

        #endregion

        #region - IPublishApiClient<IPublishableGroupSet> Implementation -

        private async Task<IResult<IPublishableGroupSet>> FixupUniqueGroupSetErrorAsync(IPublishableGroupSet item, IResult<IGroupSet> createResult, CancellationToken cancel)
        {
            if (createResult.Success)
            {
                /*
                 * Successfully created group sets start with zero groups assigned,
                 * no need to pull the destination groups.
                 */
                return Result<IPublishableGroupSet>.Succeeded(new PublishableGroupSet(createResult.Value, []));
            }

            if (!createResult.Errors.OfType<RestException>().Any(e => RestErrorCodes.Equals(e.Code, RestErrorCodes.GROUP_SET_NAME_CONFLICT_ERROR_CODE)))
            {
                return createResult.CastFailure<IPublishableGroupSet>();
            }

            // If there's a conflict find the existing group set.
            var filters = ImmutableArray.Create(new Filter("name", FilterOperator.Equal, item.Name));

            /*
             * We grab two items here so we'll know if we match > 1.
             * This theoretically shouldn't happen since we're filtering on the same criteria as the
             * name uniqueness, but just in case.
             */
            var existingResult = await ListGroupSetsAsync(1, 2, filters, cancel).ConfigureAwait(false);

            if (!existingResult.Success)
            {
                return existingResult.CastFailure<IPublishableGroupSet>();
            }
            else if (existingResult.Value.Count == 0)
            {
                return Result<IPublishableGroupSet>.Failed(new Exception($@"Could not find a group set with the name ""{item.Location}""."));
            }
            else if (existingResult.Value.Count > 1)
            {
                return Result<IPublishableGroupSet>.Failed(new Exception($@"Found multiple group sets with the name ""{item.Location}""."));
            }

            // Get the groups for the existing group set.
            var existingGroupSet = existingResult.Value.Single();
            return await GetGroupSetAsync(existingGroupSet.Id, cancel).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IResult<IPublishableGroupSet>> PublishAsync(IPublishableGroupSet item, CancellationToken cancel)
        {
            var createResult = await CreateGroupSetAsync(item.Name, cancel).ConfigureAwait(false);

            var publishResult = await FixupUniqueGroupSetErrorAsync(item, createResult, cancel).ConfigureAwait(false);
            if (!publishResult.Success)
            {
                return publishResult;
            }

            // Update group set groups.
            var destinationGroupSetId = publishResult.Value.Id;

            var resultBuilder = new ResultBuilder();
            var destinationGroups = publishResult.Value.Groups.ToDictionary(g => g.Id, g => g);
            var targetGroupIds = item.Groups.Select(u => u.Id).ToImmutableHashSet();

            if (_configReader.Get<IGroupSet>().OverwriteGroupSetGroupsEnabled)
            {
                foreach (var destinationGroupId in destinationGroups.Keys.ToImmutableArray())
                {
                    cancel.ThrowIfCancellationRequested();
                    if (targetGroupIds.Contains(destinationGroupId))
                    {
                        continue;
                    }

                    var removeGroupResult = await RemoveGroupFromGroupSetAsync(destinationGroupSetId, destinationGroupId, cancel)
                        .ConfigureAwait(false);

                    resultBuilder.Add(removeGroupResult);
                    destinationGroups.Remove(destinationGroupId);
                }
            }

            foreach (var targetGroup in item.Groups)
            {
                cancel.ThrowIfCancellationRequested();
                if (destinationGroups.ContainsKey(targetGroup.Id))
                {
                    continue;
                }

                var addUserResult = await AddGroupToGroupSetAsync(destinationGroupSetId, targetGroup.Id, cancel)
                    .ConfigureAwait(false);

                resultBuilder.Add(addUserResult);
                destinationGroups[targetGroup.Id] = targetGroup;
            }

            var updateGroupsResult = resultBuilder.Build();
            PublishableGroupSet? resultValue = null;
            if (updateGroupsResult.Success)
            {
                resultValue = new PublishableGroupSet(publishResult.Value, destinationGroups.Values.ToList());
            }

            return Result<IPublishableGroupSet>.Create(updateGroupsResult, resultValue);
        }

        #endregion
    }
}
