//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Paging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;

using CloudModels = Tableau.Migration.Api.Models.Cloud;
using CloudRequests = Tableau.Migration.Api.Rest.Models.Requests.Cloud;
using CloudResponses = Tableau.Migration.Api.Rest.Models.Responses.Cloud;
using ServerResponses = Tableau.Migration.Api.Rest.Models.Responses.Server;

namespace Tableau.Migration.Api
{
    internal sealed class SubscriptionsApiClient : ContentApiClientBase, ISubscriptionsApiClient
    {
        private readonly IContentCacheFactory _contentCacheFactory;
        private readonly IServerSessionProvider _sessionProvider;
        private readonly ISchedulesApiClient _schedulesApiClient;
        private readonly IHttpContentSerializer _serializer;

        public SubscriptionsApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IContentCacheFactory contentCacheFactory,
            ISchedulesApiClientFactory schedulesApiClientFactory,
            ILoggerFactory loggerFactory,
            ISharedResourcesLocalizer sharedResourcesLocalizer,
            IServerSessionProvider sessionProvider,
            IHttpContentSerializer serializer)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer, RestUrlKeywords.Subscriptions)
        {
            _contentCacheFactory = contentCacheFactory;
            _sessionProvider = sessionProvider;
            _schedulesApiClient = schedulesApiClientFactory.Create();
            _serializer = serializer;
        }

        #region - Subscription Content Item Factories -

        private async Task<IContentReference?> GetSubscriptionContentReferenceAsync(ISubscriptionType response, CancellationToken cancel)
        {
            var contentResponse = Guard.AgainstNull(response.Content, () => response.Content);
            var contentType = contentResponse.GetContentType();

            IContentReference? contentReference;
            switch (contentType)
            {
                case SubscriptionContentType.Workbook:
                    contentReference = await FindWorkbookByIdAsync(contentResponse.Id, throwIfNotFound: false, cancel).ConfigureAwait(false);
                    break;
                case SubscriptionContentType.View:
                    contentReference = await FindViewByIdAsync(contentResponse.Id, throwIfNotFound: false, cancel).ConfigureAwait(false);
                    break;
                default:
                    Logger.LogWarning(SharedResourcesLocalizer[SharedResourceKeys.SubscriptionContentTypeNotSupportedWarning],
                        response.Subject, contentResponse.Type);
                    return null;
            }

            if (contentReference is null)
            {
                Logger.LogWarning(SharedResourcesLocalizer[SharedResourceKeys.SubscriptionSkippedMissingContentWarning],
                    contentType, contentResponse.Id);
            }

            return contentReference;
        }

        private async Task<IServerSubscription?> CreateServerSubscriptionAsync(ISubscriptionType response,
            IContentCache<IServerSchedule> scheduleCache, Guid scheduleId, CancellationToken cancel)
        {
            var user = await FindUserAsync(Guard.AgainstNull(response.User, () => response.User), throwIfNotFound: false, cancel).ConfigureAwait(false);
            if (user is null)
            {
                Logger.LogWarning(SharedResourcesLocalizer[SharedResourceKeys.SubscriptionSkippedMissingUserWarning],
                    response.User.Id);
                return null;
            }

            var schedule = await scheduleCache.ForIdAsync(scheduleId, cancel).ConfigureAwait(false);
            if (schedule is null)
            {
                var getScheduleResult = await _schedulesApiClient.GetByIdAsync(scheduleId, cancel)
                    .ConfigureAwait(false);

                if (!getScheduleResult.Success)
                {
                    throw new InvalidOperationException($"A schedule could not be fetched for Server Subscription. {response.Id}");
                }

                schedule = getScheduleResult.Value;
                scheduleCache.AddOrUpdate(schedule);
            }

            Guard.AgainstNull(schedule, nameof(schedule));

            var contentReference = await GetSubscriptionContentReferenceAsync(response, cancel).ConfigureAwait(false);
            if (contentReference is null)
            {
                return null;
            }

            return new ServerSubscription(response, user, schedule, contentReference);
        }

        private async Task<IImmutableList<IServerSubscription>> CreateServerSubscriptionsAsync(ServerResponses.GetSubscriptionsResponse response, CancellationToken cancel)
        {
            var scheduleCache = _contentCacheFactory.ForContentType<IServerSchedule>(throwIfNotAvailable: true);

            var results = ImmutableArray.CreateBuilder<IServerSubscription>(response.Items.Length);
            foreach (var item in response.Items)
            {
                var scheduleId = Guard.AgainstNull(item.Schedule, () => item.Schedule).Id;
                var sub = await CreateServerSubscriptionAsync(item, scheduleCache, scheduleId, cancel).ConfigureAwait(false);
                if (sub is not null)
                {
                    results.Add(sub);
                }
            }

            return results.ToImmutable();
        }

        private async Task<IServerSubscription> CreateServerSubscriptionAsync(ServerResponses.GetSubscriptionResponse response, CancellationToken cancel)
        {
            var scheduleCache = _contentCacheFactory.ForContentType<IServerSchedule>(throwIfNotAvailable: true);

            var item = Guard.AgainstNull(response.Item, nameof(response.Item));
            var scheduleId = Guard.AgainstNull(item.Schedule, () => item.Schedule).Id;

            var sub = await CreateServerSubscriptionAsync(item, scheduleCache, scheduleId, cancel).ConfigureAwait(false);

            return sub;
        }

        private async Task<ICloudSubscription?> CreateCloudSubscriptionAsync(ISubscriptionType response, CloudResponses.ICloudScheduleType schedule, CancellationToken cancel)
        {
            var user = await FindUserAsync(Guard.AgainstNull(response.User, () => response.User), throwIfNotFound: false, cancel).ConfigureAwait(false);
            if (user is null)
            {
                Logger.LogWarning(SharedResourcesLocalizer[SharedResourceKeys.SubscriptionSkippedMissingUserWarning],
                    response.User.Id);
                return null;
            }

            var contentReference = await GetSubscriptionContentReferenceAsync(response, cancel).ConfigureAwait(false);
            if (contentReference is null)
            {
                return null;
            }

            return new CloudSubscription(response, user, schedule, contentReference);
        }

        private async Task<IImmutableList<ICloudSubscription>> CreateCloudSubscriptionsAsync(CloudResponses.GetSubscriptionsResponse response, CancellationToken cancel)
        {
            var results = ImmutableArray.CreateBuilder<ICloudSubscription>(response.Items.Length);
            foreach (var item in response.Items)
            {
                var sub = await CreateCloudSubscriptionAsync(item, Guard.AgainstNull(item.Schedule, () => item.Schedule), cancel).ConfigureAwait(false);
                if (sub is not null)
                {
                    results.Add(sub);
                }
            }

            return results.ToImmutable();
        }

        private async Task<ICloudSubscription> CreateCloudSubscriptionAsync(CloudResponses.GetSubscriptionResponse response, CancellationToken cancel)
        {
            var item = Guard.AgainstNull(response.Item, nameof(response.Item));
            var sub = await CreateCloudSubscriptionAsync(item, Guard.AgainstNull(item.Schedule, () => item.Schedule), cancel).ConfigureAwait(false);

            return sub!;
        }

        #endregion

        /// <inheritdoc />
        public IServerSubscriptionsApiClient ForServer()
            => ExecuteForInstanceType(TableauInstanceType.Server, _sessionProvider.InstanceType, () => this);

        /// <inheritdoc />
        public ICloudSubscriptionsApiClient ForCloud()
            => ExecuteForInstanceType(TableauInstanceType.Cloud, _sessionProvider.InstanceType, () => this);

        private IHttpGetRequestBuilder BuildGetSubscriptionsRequest(int pageNumber, int pageSize)
            => RestRequestBuilderFactory.CreateUri(UrlPrefix).WithPage(pageNumber, pageSize).ForGetRequest();

        private IHttpGetRequestBuilder BuildGetSubscriptionRequest(Guid subscriptionId)
            => RestRequestBuilderFactory.CreateUri($"{UrlPrefix}/{subscriptionId.ToUrlSegment()}").ForGetRequest();

        private IHttpPostRequestBuilder BuildCreateSubscriptionRequest(TableauServerRequest request)
            => RestRequestBuilderFactory
                .CreateUri(UrlPrefix)
                .ForPostRequest()
            .WithXmlContent(request);

        private IHttpPutRequestBuilder BuildUpdateSubscriptionRequest<TRequest>(Guid subscriptionId, TRequest payload)
            where TRequest : class
            => RestRequestBuilderFactory
                .CreateUri($"{UrlPrefix}/{subscriptionId.ToUrlSegment()}")
                .ForPutRequest()
            .WithXmlContent(payload);

        #region - IServerSubscriptionsApiClient Implementation -

        async Task<IPagedResult<IServerSubscription>> IServerSubscriptionsApiClient.GetAllSubscriptionsAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllServerSubscriptionsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        async Task<IPagedResult<IServerSubscription>> GetAllServerSubscriptionsAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            return await BuildGetSubscriptionsRequest(pageNumber, pageSize)
                       .SendAsync<ServerResponses.GetSubscriptionsResponse>(cancel)
                       .ToPagedResultAsync(CreateServerSubscriptionsAsync, SharedResourcesLocalizer, cancel)
                       .ConfigureAwait(false);
        }

        #endregion

        #region - IPagedListApiClient<IServerSubscription> Implementation -

        /// <inheritdoc />
        public IPager<IServerSubscription> GetPager(int pageSize) => new ApiListPager<IServerSubscription>(this, pageSize);

        #endregion

        #region - IApiPageAccessor<IServerSubscription> Implementation -

        /// <inheritdoc />
        public async Task<IPagedResult<IServerSubscription>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancel)
            => await GetAllServerSubscriptionsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        #endregion

        #region - IReadApiClient<IServerSubscription> Implementation -

        /// <inheritdoc />
        async Task<IResult<IServerSubscription>> IReadApiClient<IServerSubscription>.GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            return await BuildGetSubscriptionRequest(contentId)
                .SendAsync<ServerResponses.GetSubscriptionResponse>(cancel)
                .ToResultAsync(CreateServerSubscriptionAsync, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        #endregion

        #region - IReadApiClient<ICloudSubscription> Implementation -

        /// <inheritdoc />
        async Task<IResult<ICloudSubscription>> IReadApiClient<ICloudSubscription>.GetByIdAsync(Guid contentId, CancellationToken cancel)
        {
            return await BuildGetSubscriptionRequest(contentId)
                .SendAsync<CloudResponses.GetSubscriptionResponse>(cancel)
                .ToResultAsync(CreateCloudSubscriptionAsync, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        #endregion

        #region - ICloudSubscriptionsApiClient Implementation -

        async Task<IPagedResult<ICloudSubscription>> ICloudSubscriptionsApiClient.GetAllSubscriptionsAsync(int pageNumber, int pageSize, CancellationToken cancel)
        {
            return await BuildGetSubscriptionsRequest(pageNumber, pageSize)
              .SendAsync<CloudResponses.GetSubscriptionsResponse>(cancel)
              .ToPagedResultAsync(CreateCloudSubscriptionsAsync, SharedResourcesLocalizer, cancel)
              .ConfigureAwait(false);
        }

        async Task<IResult<ICloudSubscription>> ICloudSubscriptionsApiClient.CreateSubscriptionAsync(ICloudSubscription subscription, CancellationToken cancel)
        {
            var options = new CloudModels.CreateSubscriptionOptions(subscription);
            return await BuildCreateSubscriptionRequest(new CloudRequests.CreateSubscriptionRequest(options))
                .SendAsync<CloudResponses.CreateSubscriptionResponse>(cancel)
                .ToResultAsync(async (r, c) =>
                {
                    var sub = Guard.AgainstNull(r.Item, () => r.Item);
                    var result = await CreateCloudSubscriptionAsync(sub, Guard.AgainstNull(sub.Schedule, () => sub.Schedule), cancel).ConfigureAwait(false);
                    return Guard.AgainstNull(result, () => result);

                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        async Task<IResult<ICloudSubscription>> ICloudSubscriptionsApiClient.UpdateSubscriptionAsync(Guid subscriptionId, CancellationToken cancel,
            string? newSubject, bool? newAttachImage, bool? newAttachPdf,
            string? newPageOrientation, string? newPageSizeOption, bool? newSuspended, string? newMessage,
            ISubscriptionContent? newContent, Guid? newUserId, ICloudSchedule? newSchedule)
        {
            return await BuildUpdateSubscriptionRequest(subscriptionId,
                new CloudRequests.UpdateSubscriptionRequest(newSubject, newAttachImage, newAttachPdf,
                newPageOrientation, newPageSizeOption, newSuspended, newMessage,
                newContent, newUserId, newSchedule))
                .SendAsync<CloudResponses.UpdateSubscriptionResponse>(cancel)
                .ToResultAsync(async (r, c) =>
                {
                    var sub = Guard.AgainstNull(r.Item, () => r.Item);
                    var result = await CreateCloudSubscriptionAsync(sub, Guard.AgainstNull(r.Schedule, () => r.Schedule), cancel).ConfigureAwait(false);
                    return Guard.AgainstNull(result, () => result);

                }, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        async Task<IResult> IDeleteApiClient.DeleteAsync(Guid subscriptionId, CancellationToken cancel)
        {
            return await RestRequestBuilderFactory
                .CreateUri($"/{UrlPrefix}/{subscriptionId}")
                .ForDeleteRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);
        }

        #endregion

        #region - IPublishApiClient<ICloudSubscription> Implementation -

        async Task<IResult<ICloudSubscription>> IPublishApiClient<ICloudSubscription, ICloudSubscription>.PublishAsync(ICloudSubscription item, CancellationToken cancel)
        {
            return await ForCloud().CreateSubscriptionAsync(item, cancel).ConfigureAwait(false);
        }

        #endregion
    }
}
