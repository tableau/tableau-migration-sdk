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
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Api.Tags;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Tests.Unit.Api
{
    internal class ApiClientTestDependencies : IDisposable, IApiClientTestDependencies, IServiceProvider
    {
        private readonly IFixture _autoFixture;
        private readonly ServiceProvider _serviceProvider;

        public Mock<IApiClientInput> MockApiClientInput { get; } = new();
        public Mock<IRequestBuilderFactoryInput> MockRequestBuilderInput { get; } = new();
        public MockHttpClient MockHttpClient { get; } = new();
        public Mock<ITableauServerVersionProvider> MockVersionProvider { get; } = new();
        public Mock<IServerSessionProvider> MockSessionProvider { get; } = new();
        public Mock<IAuthenticationTokenProvider> MockTokenProvider { get; } = new();
        public Mock<ILoggerFactory> MockLoggerFactory { get; } = new();
        public Mock<IConfigReader> MockConfigReader { get; } = new();
        public MockSharedResourcesLocalizer MockSharedResourcesLocalizer { get; } = new();
        public Mock<IContentReferenceFinderFactory> MockContentFinderFactory { get; } = new() { CallBase = true };
        public Mock<IContentCacheFactory> MockContentCacheFactory { get; } = new();
        public Mock<IPermissionsApiClientFactory> MockPermissionsClientFactory { get; } = new();
        public Mock<ITaskDelayer> MockTaskDelayer { get; } = new();
        public TestHttpStreamProcessor HttpStreamProcessor { get; }
        public Mock<IDataSourcePublisher> MockDataSourcePublisher { get; } = new();
        public Mock<IWorkbookPublisher> MockWorkbookPublisher { get; } = new();
        public Mock<ITagsApiClientFactory> MockTagsApiClientFactory { get; } = new();
        public Mock<ITagsApiClient> MockTagsApiClient { get; } = new();
        public Mock<IViewsApiClientFactory> MockViewsApiClientFactory { get; } = new();
        public Mock<IViewsApiClient> MockViewsApiClient { get; } = new();
        
        public Mock<IViewsApiClient> MockCustomViewsApiClient { get; } = new();
        public IHttpContentSerializer Serializer { get; } = HttpContentSerializer.Instance;
        public IRestRequestBuilderFactory RestRequestBuilderFactory { get; }

        public IServiceCollection Services { get; }

        public Mock<ILogger> MockLogger { get; } = new();

        public Mock<IContentReferenceFinder<IProject>> MockProjectFinder { get; }
        public Mock<IContentReferenceFinder<IUser>> MockUserFinder { get; }
        public Mock<IContentReferenceFinder<IWorkbook>> MockWorkbookFinder { get; }
        public Mock<IContentReferenceFinder<IView>> MockViewFinder { get; }
        public Mock<IContentReferenceFinder<IDataSource>> MockDataSourceFinder { get; }
        public Mock<IContentReferenceFinder<IServerSchedule>> MockScheduleFinder { get; }

        public Mock<IContentCache<IServerSchedule>> MockScheduleCache { get; } = new();

        public TableauServerVersion TableauServerVersion { get; private set; }

        public TableauSiteConnectionConfiguration SiteConnectionConfiguration { get; }

        public ApiClientTestDependencies(IFixture autoFixture)
        {
            _autoFixture = autoFixture;

            TableauServerVersion = _autoFixture.Create<TableauServerVersion>();

            SiteConnectionConfiguration = _autoFixture.Build<TableauSiteConnectionConfiguration>()
                .With(c => c.ServerUrl, TestConstants.LocalhostUri)
                .Create();

            var httpRequestBuilderFactory = new HttpRequestBuilderFactory(MockHttpClient.Object, Serializer);
            RestRequestBuilderFactory = new RestRequestBuilderFactory(MockRequestBuilderInput.Object, MockSessionProvider.Object, httpRequestBuilderFactory);

            MockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(MockLogger.Object);

            MockConfigReader
                .Setup(x => x.Get())
                .Returns(new MigrationSdkOptions());

            MockProjectFinder = MockContentFinderFactory.SetupMockFinder<IProject>(autoFixture);
            MockUserFinder = MockContentFinderFactory.SetupMockFinder<IUser>(autoFixture);
            MockWorkbookFinder = MockContentFinderFactory.SetupMockFinder<IWorkbook>(autoFixture);
            MockViewFinder = MockContentFinderFactory.SetupMockFinder<IView>(autoFixture);
            MockDataSourceFinder = MockContentFinderFactory.SetupMockFinder<IDataSource>(autoFixture);
            MockScheduleFinder = MockContentFinderFactory.SetupMockFinder<IServerSchedule>(autoFixture);

            MockScheduleCache = MockContentCacheFactory.SetupMockCache<IServerSchedule>(autoFixture);


            MockApiClientInput.SetupGet(i => i.SiteConnectionConfiguration).Returns(SiteConnectionConfiguration);
            MockSessionProvider.SetupGet(p => p.Version).Returns(TableauServerVersion);

            MockSessionProvider
                .Setup(p => p.SetVersion(It.IsAny<TableauServerVersion>()))
                .Callback((TableauServerVersion v) =>
                {
                    TableauServerVersion = v;
                    MockSessionProvider.SetupGet(p => p.Version).Returns(TableauServerVersion);
                });

            MockRequestBuilderInput.SetupGet(i => i.ServerUri).Returns(SiteConnectionConfiguration.ServerUrl);
            MockRequestBuilderInput.SetupGet(i => i.IsInitialized).Returns(true);

            HttpStreamProcessor = new TestHttpStreamProcessor(MockHttpClient.Object, MockConfigReader.Object);

            MockTagsApiClientFactory.Setup(x => x.Create(It.IsAny<IContentApiClient>())).Returns(MockTagsApiClient.Object);

            MockViewsApiClientFactory.Setup(x => x.Create()).Returns(MockViewsApiClient.Object);

            Services = new ServiceCollection()
                .AddTableauMigrationSdk();

            ReplaceServices();

            _serviceProvider = Services.BuildServiceProvider();
        }

        private void ReplaceServices()
        {
            ReplaceService(MockApiClientInput);
            ReplaceService(MockRequestBuilderInput);
            ReplaceService(MockHttpClient);
            ReplaceService(MockVersionProvider);
            ReplaceService(MockSessionProvider);
            ReplaceService(MockTokenProvider);
            ReplaceService(MockLoggerFactory);
            ReplaceService(MockConfigReader);
            ReplaceService(MockSharedResourcesLocalizer);
            ReplaceService(MockContentFinderFactory);
            ReplaceService(MockContentCacheFactory);
            ReplaceService(MockPermissionsClientFactory);
            ReplaceService(MockDataSourcePublisher);
            ReplaceService(MockWorkbookPublisher);
            ReplaceService(MockTagsApiClientFactory);
            ReplaceService(MockViewsApiClientFactory);
            ReplaceService(MockTaskDelayer);
            ReplaceService(Serializer);
            ReplaceService(RestRequestBuilderFactory);
            ReplaceService<IHttpStreamProcessor>(HttpStreamProcessor);
        }

        public void ReplaceService<T>(T service)
            where T : class => Services.Replace(service);

        #region - API Client Creation Factory Methods -

        public TApiClient CreateClient<TApiClient>()
            where TApiClient : IContentApiClient
            => this.GetRequiredService<TApiClient>();

        #endregion

        #region - IServiceProvider -

        public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

        #endregion

        #region - IDisposable -

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _serviceProvider.Dispose();
        }

        #endregion
    }
}
