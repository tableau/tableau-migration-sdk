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
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{

    internal sealed class CustomViewsApiClient : ContentApiClientBase, ICustomViewsApiClient
    {
        private readonly IHttpContentSerializer _serializer;
        private readonly IConfigReader _configReader;
        private readonly IContentFileStore _fileStore;
        private readonly ICustomViewPublisher _customViewPublisher;

        public CustomViewsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IContentReferenceFinderFactory finderFactory,
            IHttpContentSerializer serializer,
            IConfigReader configReader,
            IContentFileStore fileStore,
            ICustomViewPublisher customViewPublisher)
            : base(
                  restRequestBuilderFactory,
                  finderFactory,
                  loggerFactory,
                  sharedResourcesLocalizer,
                  RestUrlPrefixes.CustomViews)
        {
            _serializer = serializer;
            _configReader = configReader;
            _fileStore = fileStore;
            _customViewPublisher = customViewPublisher;
        }

        /// <inheritdoc />
        public async Task<IPagedResult<ICustomView>> GetAllCustomViewsAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            var getAllCustomViewsResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<CustomViewsResponse>(cancel)
                .ToPagedResultAsync<CustomViewsResponse, ICustomView>(async (r, c) =>
                {
                    // Take all items.
                    var results = ImmutableArray.CreateBuilder<ICustomView>(r.Items.Length);

                    foreach (var item in r.Items)
                    {
                        var workbook = await FindWorkbookAsync(item, false, c).ConfigureAwait(false);
                        var owner = await FindOwnerAsync(item, false, c).ConfigureAwait(false);

                        if (workbook is null || owner is null)
                        {
                            Logger.LogWarning(
                                SharedResourcesLocalizer[SharedResourceKeys.CustomViewSkippedMissingReferenceWarning],
                                item.Id,
                                item.Name,
                                item.Workbook!.Id,
                                workbook is null ? SharedResourcesLocalizer[SharedResourceKeys.NotFound] : SharedResourcesLocalizer[SharedResourceKeys.Found],
                                item.Owner!.Id,
                                owner is null ? SharedResourcesLocalizer[SharedResourceKeys.NotFound] : SharedResourcesLocalizer[SharedResourceKeys.Found]);
                            continue;
                        }

                        results.Add(new CustomView(item, workbook, owner));
                    }

                    // Produce immutable list of type ICustomView and return.
                    return results.ToImmutable();
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return getAllCustomViewsResult;
        }

        /// <inheritdoc />
        public async Task<IResult<ICustomView>> UpdateCustomViewAsync(
            Guid id,
            CancellationToken cancel,
            Guid? newOwnerId = null,
            string? newViewName = null)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{id.ToUrlSegment()}")
                .ForPutRequest()
                .WithXmlContent(new UpdateCustomViewRequest(newOwnerId, newViewName))
                .SendAsync<UpdateCustomViewResponse>(cancel)
                .ToResultAsync<UpdateCustomViewResponse, ICustomView>(async (r, c) =>
                {
                    var workbook = await FindWorkbookAsync(r.Item!, true, c).ConfigureAwait(false);
                    var owner = await FindOwnerAsync(r.Item!, true, c).ConfigureAwait(false);

                    return new CustomView(r.Item!, workbook, owner);
                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult> DeleteCustomViewAsync(Guid id, CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{id.ToUrlSegment()}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public async Task<IResult<IImmutableList<IContentReference>>> GetAllCustomViewDefaultUsersAsync(
            Guid id,
            CancellationToken cancel)
        {
            IPager<IContentReference> pager = new CustomViewDefaultUsersResponsePager(
                this,
                id,
                _configReader.Get<IUser>().BatchSize);

            return await pager.GetAllPagesAsync(cancel)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IPagedResult<IContentReference>> GetCustomViewDefaultUsersAsync(
            Guid id,
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
        {
            var result = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{id.ToUrlSegment()}/default/users")
                .WithPage(pageNumber, pageSize)
                .ForGetRequest()
                .SendAsync<UsersWithCustomViewAsDefaultViewResponse>(cancel)
                .ToResultAsync(r => r, SharedResourcesLocalizer)
                .ConfigureAwait(false);

            if (!result.Success)
            {
                return PagedResult<IContentReference>.Failed(
                    result.Errors);
            }

            // Take all items.
            var results = ImmutableArray.CreateBuilder<IContentReference>(result.Value.Items.Length);

            foreach (var item in result.Value.Items)
            {
                var user = await FindUserAsync(item, false, cancel).ConfigureAwait(false);

                if (user is null)
                {
                    Logger.LogWarning(
                        SharedResourcesLocalizer[SharedResourceKeys.UserWithCustomViewDefaultSkippedMissingReferenceWarning],
                        item.Id,
                        id);
                    continue;
                }

                results.Add(user);
            }

            return PagedResult<IContentReference>.Succeeded(
                results.ToImmutable(),
                result.Value.Pagination.PageNumber,
                result.Value.Pagination.PageSize,
                result.Value.Pagination.TotalAvailable,
                ((IPageInfo)result.Value).FetchedAllPages);
        }

        /// <inheritdoc />
        public async Task<IResult<IImmutableList<ICustomViewAsUserDefaultViewResult>>> SetCustomViewDefaultUsersAsync(
            Guid id,
            IEnumerable<IContentReference> users,
            CancellationToken cancel)
        {
            if (!users.Any())
            {
                return Result<IImmutableList<ICustomViewAsUserDefaultViewResult>>
                    .Succeeded(ImmutableArray<ICustomViewAsUserDefaultViewResult>.Empty);
            }

            var setResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{id.ToUrlSegment()}/default/users")
                .ForPostRequest()
                .WithXmlContent(new SetCustomViewDefaultUsersRequest(users))
                .SendAsync<CustomViewAsUsersDefaultViewResponse>(cancel)
                .ToResultAsync<CustomViewAsUsersDefaultViewResponse, IImmutableList<ICustomViewAsUserDefaultViewResult>>((response) =>
                        response.Items
                            .Select(item => (ICustomViewAsUserDefaultViewResult)new CustomViewAsUserDefaultViewResult(item))
                            .ToImmutableArray(),
                    SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return setResult;
        }

        /// <inheritdoc />
        public async Task<IAsyncDisposableResult<FileDownload>> DownloadCustomViewAsync(
            Guid customViewId,
            CancellationToken cancel)
        {
            var downloadResult = await RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{customViewId.ToUrlSegment()}/{RestUrlPrefixes.Content}", true)
                .ForGetRequest()
                .DownloadAsync(cancel)
                .ConfigureAwait(false);

            return downloadResult;
        }

        #region - IPullApiClient<ICustomView, IPublishableCustomView> Implementation -
        /// <inheritdoc />
        public async Task<IResult<IPublishableCustomView>> PullAsync(ICustomView contentItem, CancellationToken cancel)
        {
            var downloadResult = await DownloadCustomViewAsync(contentItem.Id, cancel).ConfigureAwait(false);

            if (!downloadResult.Success)
            {
                return downloadResult.CastFailure<IPublishableCustomView>();
            }

            var defaultUsersResult = await GetAllCustomViewDefaultUsersAsync(contentItem.Id, cancel).ConfigureAwait(false);

            if (!defaultUsersResult.Success)
            {
                return defaultUsersResult.CastFailure<IPublishableCustomView>();
            }

            await using (downloadResult)
            {
                var file = await _fileStore.CreateAsync(contentItem, downloadResult.Value, cancel).ConfigureAwait(false);

                var result = new PublishableCustomView(contentItem, [.. defaultUsersResult.Value], file);

                return Result<IPublishableCustomView>.Succeeded(result);
            }
        }

        #endregion

        /// <inheritdoc />
        public async Task<IResult<ICustomView>> PublishCustomViewAsync(
            IPublishCustomViewOptions options,
            CancellationToken cancel)
            => await _customViewPublisher.PublishAsync(options, cancel).ConfigureAwait(false);

        #region - IPublishApiClient<IPublishableCustomView, ICustomView> Implementation -
        /// <inheritdoc />
        public async Task<IResult<ICustomView>> PublishAsync(
            IPublishableCustomView contentItem,
            CancellationToken cancel)
        {
            var fileStream = await contentItem.File.OpenReadAsync(cancel).ConfigureAwait(false);
            await using (fileStream)
            {
                var publishResult = await PublishCustomViewAsync(
                    new PublishCustomViewOptions(contentItem, fileStream.Content),
                    cancel)
                    .ConfigureAwait(false);

                if (!publishResult.Success)
                {
                    return publishResult.CastFailure<ICustomView>();
                }

                return publishResult;
            }
        }
        #endregion

        #region - IPagedListApiClient<ICustomView> Implementation -

        /// <inheritdoc />
        public IPager<ICustomView> GetPager(int pageSize) => new ApiListPager<ICustomView>(this, pageSize);

        #endregion

        #region - IApiPageAccessor<ICustomView> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<ICustomView>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllCustomViewsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion

        #region - IReadApiClient<ICustomView> Implementation -

        /// <inheritdoc />
        public async Task<IResult<ICustomView>> GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            var getCustomViewResult = await RestRequestBuilderFactory
               .CreateUri($"/{UrlPrefix}/{contentId.ToUrlSegment()}")
               .ForGetRequest()
               .SendAsync<CustomViewResponse>(cancel)
               .ToResultAsync<CustomViewResponse, ICustomView>(async (r, c) =>
               {
                   var workbook = await FindWorkbookAsync(r.Item!, true, c).ConfigureAwait(false);
                   var owner = await FindOwnerAsync(r.Item!, true, c).ConfigureAwait(false);

                   return new CustomView(r.Item!, workbook, owner);
               }, SharedResourcesLocalizer, cancel)
               .ConfigureAwait(false);

            return getCustomViewResult;
        }
        #endregion
    }

}
