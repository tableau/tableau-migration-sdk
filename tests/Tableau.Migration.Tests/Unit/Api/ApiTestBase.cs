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
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Tests.Unit.Api
{
    public abstract class ApiTestBase : AutoFixtureTestBase, IApiClientTestDependencies
    {
        #region - IApiClientTestDependencies -

        public Mock<IApiClientInput> MockApiClientInput => Dependencies.MockApiClientInput;
        public Mock<IRequestBuilderFactoryInput> MockRequestBuilderInput => Dependencies.MockRequestBuilderInput;
        public MockHttpClient MockHttpClient => Dependencies.MockHttpClient;
        public Mock<ITableauServerVersionProvider> MockVersionProvider => Dependencies.MockVersionProvider;
        public Mock<IServerSessionProvider> MockSessionProvider => Dependencies.MockSessionProvider;
        public Mock<IAuthenticationTokenProvider> MockTokenProvider => Dependencies.MockTokenProvider;
        public Mock<ILoggerFactory> MockLoggerFactory => Dependencies.MockLoggerFactory;
        public Mock<IConfigReader> MockConfigReader => Dependencies.MockConfigReader;
        public Mock<ILogger> MockLogger => Dependencies.MockLogger;
        public MockSharedResourcesLocalizer MockSharedResourcesLocalizer => Dependencies.MockSharedResourcesLocalizer;
        public Mock<IContentReferenceFinderFactory> MockContentFinderFactory => Dependencies.MockContentFinderFactory;
        public Mock<IContentReferenceFinder<IProject>> MockProjectFinder => Dependencies.MockProjectFinder;
        public Mock<IContentReferenceFinder<IUser>> MockUserFinder => Dependencies.MockUserFinder;
        public Mock<IContentReferenceFinder<IWorkbook>> MockWorkbookFinder => Dependencies.MockWorkbookFinder;
        public Mock<IContentReferenceFinder<IView>> MockViewFinder => Dependencies.MockViewFinder;
        public Mock<IContentReferenceFinder<IDataSource>> MockDataSourceFinder => Dependencies.MockDataSourceFinder;
        public Mock<IContentReferenceFinder<IServerSchedule>> MockScheduleFinder => Dependencies.MockScheduleFinder;
        public Mock<IContentCacheFactory> MockContentCacheFactory => Dependencies.MockContentCacheFactory;
        public Mock<IContentCache<IServerSchedule>> MockScheduleCache => Dependencies.MockScheduleCache;
        public Mock<IPermissionsApiClientFactory> MockPermissionsClientFactory => Dependencies.MockPermissionsClientFactory;
        public Mock<ITaskDelayer> MockTaskDelayer => Dependencies.MockTaskDelayer;
        public Mock<IDataSourcePublisher> MockDataSourcePublisher => Dependencies.MockDataSourcePublisher;
        public Mock<IWorkbookPublisher> MockWorkbookPublisher => Dependencies.MockWorkbookPublisher;
        public Mock<ITagsApiClient> MockTagsApiClient => Dependencies.MockTagsApiClient;
        public Mock<IViewsApiClient> MockViewsApiClient => Dependencies.MockViewsApiClient;
        public TestHttpStreamProcessor HttpStreamProcessor => Dependencies.HttpStreamProcessor;
        public IHttpContentSerializer Serializer => Dependencies.Serializer;
        public IRestRequestBuilderFactory RestRequestBuilderFactory => Dependencies.RestRequestBuilderFactory;
        public TableauServerVersion TableauServerVersion => Dependencies.TableauServerVersion;
        public TableauSiteConnectionConfiguration SiteConnectionConfiguration => Dependencies.SiteConnectionConfiguration;

        #endregion

        internal readonly ApiClientTestDependencies Dependencies;

        public ApiTestBase()
        {
            Dependencies = new(AutoFixture);
        }

        protected void AssertUri(HttpRequestMessage request, string expectedRelativeUri)
            => request.AssertUri(SiteConnectionConfiguration, MockSessionProvider.Object, expectedRelativeUri);

        protected void AssertSiteUri(HttpRequestMessage request, string expectedRelativeUri)
            => request.AssertSiteUri(SiteConnectionConfiguration, MockSessionProvider.Object, expectedRelativeUri);

        protected IHttpResponseMessage SetupSuccessResponse(HttpContent? content = null)
            => MockHttpClient.SetupSuccessResponse(content);

        protected (IHttpResponseMessage<TContent> Response, TContent Content) SetupSuccessResponse<TContent>(
            Action<TContent>? configureContent = null)
            where TContent : TableauServerResponse, new()
            => MockHttpClient.SetupSuccessResponse(AutoFixture, configureContent);

        protected (IHttpResponseMessage<TContent> Response, TContent Content) SetupSuccessResponse<TContent>(TContent content)
            where TContent : TableauServerResponse, new()
            => MockHttpClient.SetupSuccessResponse(content);

        protected (IHttpResponseMessage<TContent> Response, TContent Content, TItem Item) SetupSuccessResponse<TContent, TItem>(
            Action<TContent>? configureContent = null,
            Action<TItem>? configureItem = null)
            where TContent : TableauServerResponse<TItem>, new()
            where TItem : class
            => MockHttpClient.SetupSuccessResponse(AutoFixture, configureContent, configureItem);

        protected (IHttpResponseMessage Response, Error Error) SetupErrorResponse(Action<Error>? configureError = null)
            => MockHttpClient.SetupErrorResponse(AutoFixture, configureError);

        protected (IHttpResponseMessage<TContent> Response, Error Error) SetupErrorResponse<TContent>(Action<Error>? configureError = null)
            where TContent : TableauServerResponse, new()
            => MockHttpClient.SetupErrorResponse<TContent>(AutoFixture, configureError);

        protected (IHttpResponseMessage<TContent> Response, Error Error) SetupErrorResponse<TContent>(Error error)
            where TContent : TableauServerResponse, new()
            => MockHttpClient.SetupErrorResponse<TContent>(error);

        protected (IHttpResponseMessage Response, Exception Exception) SetupExceptionResponse(Exception? exception = null)
            => MockHttpClient.SetupExceptionResponse(exception);

        protected (IHttpResponseMessage<TContent> Response, Exception Exception) SetupExceptionResponse<TContent>(Exception? exception = null)
            where TContent : TableauServerResponse, new()
            => MockHttpClient.SetupExceptionResponse<TContent>(exception);

        protected TService CreateService<TService>()
            where TService : class
            => ActivatorUtilities.CreateInstance<TService>(Dependencies);
    }
}
