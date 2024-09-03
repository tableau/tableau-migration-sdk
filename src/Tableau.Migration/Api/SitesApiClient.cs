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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Api
{
    /// <summary>
    /// Default <see cref="ISitesApiClient"/> implementation.
    /// </summary>
    internal sealed class SitesApiClient : ContentApiClientBase, ISitesApiClient
    {
        private readonly IServerSessionProvider _sessionProvider;
        private readonly IHttpContentSerializer _serializer;

        private readonly ITasksApiClient _tasksApiClient;

        public SitesApiClient(
            IRestRequestBuilderFactory restRequestBuilderFactory,
            IContentReferenceFinderFactory finderFactory,
            IServerSessionProvider sessionProvider,
            IHttpContentSerializer serializer,
            ILoggerFactory loggerFactory,
            IGroupsApiClient groupsApiClient,
            IJobsApiClient jobsApiClient,
            ISchedulesApiClient schedulesApiClient,
            IProjectsApiClient projectsApiClient,
            IUsersApiClient usersApiClient,
            IDataSourcesApiClient dataSourcesApiClient,
            IWorkbooksApiClient workbooksApiClient,
            IViewsApiClient viewsApiClient,
            IFlowsApiClient flowsApiClient,
            ITasksApiClient tasksApiClient,
            ICustomViewsApiClient customViewsApiClient,
            ISharedResourcesLocalizer sharedResourcesLocalizer)
            : base(restRequestBuilderFactory, finderFactory, loggerFactory, sharedResourcesLocalizer)
        {
            _sessionProvider = sessionProvider;
            _serializer = serializer;

            Groups = groupsApiClient;
            Jobs = jobsApiClient;
            Schedules = schedulesApiClient;
            Projects = projectsApiClient;
            Users = usersApiClient;
            DataSources = dataSourcesApiClient;
            Workbooks = workbooksApiClient;
            Views = viewsApiClient;
            Flows = flowsApiClient;
            CustomViews = customViewsApiClient;

            _tasksApiClient = tasksApiClient;
        }

        private static readonly ImmutableDictionary<Type, Func<ISitesApiClient, object>> _contentTypeAccessors = new Dictionary<Type, Func<ISitesApiClient, object>>(InheritedTypeComparer.Instance)
        {
            { typeof(IUser), client => client.Users },
            { typeof(IGroup), client => client.Groups },
            { typeof(IProject), client => client.Projects },
            { typeof(IDataSource), client => client.DataSources },
            { typeof(IWorkbook), client => client.Workbooks },
            { typeof(IView), client => client.Views },
            { typeof(IFlow), client => client.Flows },
            { typeof(IServerSchedule), client => client.Schedules },
            { typeof(IServerExtractRefreshTask), client => client.ServerTasks },
            { typeof(ICloudExtractRefreshTask), client => client.CloudTasks },
            { typeof(ICustomView), client => client.CustomViews },
        }
        .ToImmutableDictionary(InheritedTypeComparer.Instance);

        private TApiClient? GetApiClientFromContentType<TApiClient>(Type contentType)
            where TApiClient : class
        {
            //TODO: validate content type, this needs heavy unit testing since we do runtime casting.
            return _contentTypeAccessors[contentType](this) as TApiClient;
        }

        #region - ISitesApiClient Implementation -

        /// <inheritdoc />
        public IGroupsApiClient Groups { get; }

        /// <inheritdoc />
        public IJobsApiClient Jobs { get; }

        /// <inheritdoc />
        public ISchedulesApiClient Schedules { get; }

        /// <inheritdoc />
        public IProjectsApiClient Projects { get; }

        /// <inheritdoc />
        public IUsersApiClient Users { get; }

        /// <inheritdoc />
        public IDataSourcesApiClient DataSources { get; }

        /// <inheritdoc />
        public IWorkbooksApiClient Workbooks { get; }

        /// <inheritdoc />
        public IViewsApiClient Views { get; }

        /// <inheritdoc />
        public IFlowsApiClient Flows { get; }

        /// <inheritdoc />
        public ICustomViewsApiClient CustomViews { get; }

        /// <inheritdoc />
        public IServerTasksApiClient ServerTasks
            => ReturnForInstanceType(TableauInstanceType.Server, _sessionProvider.InstanceType, _tasksApiClient);

        /// <inheritdoc />
        public ICloudTasksApiClient CloudTasks
            => ReturnForInstanceType(TableauInstanceType.Cloud, _sessionProvider.InstanceType, _tasksApiClient);

        /// <inheritdoc />
        public IReadApiClient<TContent>? GetReadApiClient<TContent>()
            where TContent : class
            => GetApiClientFromContentType<IReadApiClient<TContent>>(typeof(TContent));

        /// <inheritdoc />
        public IPagedListApiClient<TContent> GetListApiClient<TContent>()
            => GetApiClientFromContentType<IPagedListApiClient<TContent>>(typeof(TContent))!;

        /// <inheritdoc />
        public IPullApiClient<TContent, TPublish> GetPullApiClient<TContent, TPublish>()
            where TPublish : class
            => GetApiClientFromContentType<IPullApiClient<TContent, TPublish>>(typeof(TContent))!;

        /// <inheritdoc />
        public IPublishApiClient<TPublish, TPublishResult> GetPublishApiClient<TPublish, TPublishResult>()
            where TPublishResult : class, IContentReference
            => GetApiClientFromContentType<IPublishApiClient<TPublish, TPublishResult>>(typeof(TPublish))!; //TODO: Better resolution logic based on content/publish types

        /// <inheritdoc />
        public IBatchPublishApiClient<TPublish> GetBatchPublishApiClient<TPublish>()
            => GetApiClientFromContentType<IBatchPublishApiClient<TPublish>>(typeof(TPublish))!; //TODO: Better resolution logic based on content/publish types

        /// <inheritdoc />
        public IPermissionsApiClient GetPermissionsApiClient<TContent>()
            => GetApiClientFromContentType<IPermissionsContentApiClient>(typeof(TContent))!.Permissions; //TODO: Better resolution logic based on content/publish types

        /// <inheritdoc />
        public IPermissionsApiClient GetPermissionsApiClient(Type type)
            => GetApiClientFromContentType<IPermissionsContentApiClient>(type)!.Permissions; //TODO: Better resolution logic based on content/publish types

        /// <inheritdoc />
        public ITagsApiClient GetTagsApiClient<TContent>()
            where TContent : IWithTags
            => GetApiClientFromContentType<ITagsContentApiClient>(typeof(TContent))!.Tags; //TODO: Better resolution logic based on content/publish types

        /// <inheritdoc />
        public IOwnershipApiClient GetOwnershipApiClient<TContent>()
            where TContent : IWithOwner
            => GetApiClientFromContentType<IOwnershipApiClient>(typeof(TContent))!; //TODO: Better resolution logic based on content/publish types

        /// <inheritdoc />
        public IConnectionsApiClient GetConnectionsApiClient<TContent>()
            where TContent : IWithConnections
            => GetApiClientFromContentType<IConnectionsApiClient>(typeof(TContent))!; //TODO: Better resolution logic based on content/publish types

        private async Task<IResult<ISite>> GetSiteAsync(Func<IRestRequestBuilder, IRestRequestBuilder> setKey, CancellationToken cancel)
        {
            var request = RestRequestBuilderFactory.CreateUri("/"); //"sites" URL segment added by WithSiteId
            request = setKey(request);

            var getSiteResult = await request
                .ForGetRequest()
                .SendAsync<SiteResponse>(cancel)
                .ToResultAsync<SiteResponse, ISite>(r => new Site(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return getSiteResult;
        }

        /// <inheritdoc />
        public async Task<IResult<ISite>> GetSiteAsync(Guid siteId, CancellationToken cancel)
            => await GetSiteAsync(r => r.WithSiteId(siteId), cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IResult<ISite>> GetSiteAsync(string contentUrl, CancellationToken cancel)
            => await GetSiteAsync(r => r.WithSiteId(contentUrl).WithQuery(q => q.AddOrUpdate("key", "contentUrl")), cancel).ConfigureAwait(false);

        /// <inheritdoc />
        public async Task<IResult<ISite>> UpdateSiteAsync(ISiteSettingsUpdate update, CancellationToken cancel)
        {
            var updateResult = await RestRequestBuilderFactory
                .CreateUri("/") //"sites" URL segment added by WithSiteId
                .WithSiteId(update.SiteId)
                .ForPutRequest()
                .WithXmlContent(new UpdateSiteRequest(update))
                .SendAsync<SiteResponse>(cancel)
                .ToResultAsync<SiteResponse, ISite>(r => new Site(r), SharedResourcesLocalizer)
                .ConfigureAwait(false);

            return updateResult;
        }

        #endregion

        #region - SignOutAsync -

        /// <summary>
        /// Signs out of Tableau Server.
        /// </summary>
        /// <param name="cancel">The cancellation token to obey.</param>
        /// <returns>The sign out result.</returns>
        internal async Task<IResult> SignOutAsync(CancellationToken cancel)
        {
            if (_sessionProvider.UserId is null)
                return Result.Succeeded();

            var signOutResult = await RestRequestBuilderFactory
                .CreateUri("/auth/signout")
                .WithSiteId(null)
                .ForPostRequest()
                .SendAsync(cancel)
                .ToResultAsync(_serializer, SharedResourcesLocalizer, cancel)
                .ConfigureAwait(false);

            await _sessionProvider.ClearCurrentSessionAsync(cancel).ConfigureAwait(false);

            return signOutResult;
        }

        #endregion

        #region - IAsyncDisposable Implementation -

        public async ValueTask DisposeAsync()
        {
            try
            {
                await SignOutAsync(default).ConfigureAwait(false);
            }
            catch
            {
                //TODO: Log error
            }
        }

        #endregion
    }
}
