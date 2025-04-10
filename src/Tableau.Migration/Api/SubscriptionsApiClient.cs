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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
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
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer, RestUrlPrefixes.Subscriptions)
        {
            _contentCacheFactory = contentCacheFactory;
            _sessionProvider = sessionProvider;
            _schedulesApiClient = schedulesApiClientFactory.Create();
            _serializer = serializer;
        }

        /// <inheritdoc />
        public IServerSubscriptionsApiClient ForServer()
            => ExecuteForInstanceType(TableauInstanceType.Server, _sessionProvider.InstanceType, () => this);

        /// <inheritdoc />
        public ICloudSubscriptionsApiClient ForCloud()
            => ExecuteForInstanceType(TableauInstanceType.Cloud, _sessionProvider.InstanceType, () => this);

        private IHttpGetRequestBuilder BuildGetSubscriptionRequest(int pageNumber, int pageSize)
            => RestRequestBuilderFactory.CreateUri(UrlPrefix).WithPage(pageNumber, pageSize).ForGetRequest();

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
        async Task<IPagedResult<IServerSubscription>> IServerSubscriptionsApiClient.GetAllSubscriptionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
            => await GetAllServerSubscriptionsAsync(pageNumber, pageSize, cancel).ConfigureAwait(false);

        async Task<IPagedResult<IServerSubscription>> GetAllServerSubscriptionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
        {
            return await BuildGetSubscriptionRequest(pageNumber, pageSize)
                       .SendAsync<ServerResponses.GetSubscriptionsResponse>(cancel)
                       .ToPagedResultAsync(
                            async (r, c) => await ServerSubscription.CreateManyAsync(r, ContentFinderFactory, _contentCacheFactory, _schedulesApiClient.GetByIdAsync, Logger, SharedResourcesLocalizer, c).ConfigureAwait(false),
                            SharedResourcesLocalizer,
                            cancel)
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

        #region - ICloudSubscriptionsApiClient Implementation -

        async Task<IPagedResult<ICloudSubscription>> ICloudSubscriptionsApiClient.GetAllSubscriptionsAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancel)
        {
            return await BuildGetSubscriptionRequest(pageNumber, pageSize)
              .SendAsync<CloudResponses.GetSubscriptionsResponse>(cancel)
              .ToPagedResultAsync(
                async (r, c) => await CloudSubscription.CreateManyAsync(r, ContentFinderFactory, Logger, SharedResourcesLocalizer, c).ConfigureAwait(false),
                SharedResourcesLocalizer,
                cancel)
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
                    var user = await FindUserAsync(Guard.AgainstNull(sub.User, () => sub.User), true, c).ConfigureAwait(false);

                    return (ICloudSubscription)new CloudSubscription(sub, user, Guard.AgainstNull(r.Item.Schedule, () => r.Item.Schedule));
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
                    var user = await FindUserAsync(Guard.AgainstNull(sub.User, () => sub.User), true, c).ConfigureAwait(false);

                    return (ICloudSubscription)new CloudSubscription(sub, user, Guard.AgainstNull(r.Schedule, () => r.Schedule));
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
