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

using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Publishing;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;

namespace Tableau.Migration.Tests.Unit.Api
{
    internal interface IApiClientTestDependencies
    {
        Mock<IApiClientInput> MockApiClientInput { get; }
        Mock<IConfigReader> MockConfigReader { get; }
        Mock<IContentReferenceFinderFactory> MockContentFinderFactory { get; }
        Mock<IContentCacheFactory> MockContentCacheFactory { get; }
        Mock<IContentReferenceFinder<IProject>> MockProjectFinder { get; }
        Mock<IContentReferenceFinder<IUser>> MockUserFinder { get; }
        Mock<IContentReferenceFinder<IWorkbook>> MockWorkbookFinder { get; }
        Mock<IContentReferenceFinder<IDataSource>> MockDataSourceFinder { get; }
        Mock<IContentReferenceFinder<IServerSchedule>> MockScheduleFinder { get; }
        MockHttpClient MockHttpClient { get; }
        Mock<ILogger> MockLogger { get; }
        Mock<ILoggerFactory> MockLoggerFactory { get; }
        Mock<IPermissionsApiClientFactory> MockPermissionsClientFactory { get; }
        Mock<IRequestBuilderFactoryInput> MockRequestBuilderInput { get; }
        Mock<IServerSessionProvider> MockSessionProvider { get; }
        MockSharedResourcesLocalizer MockSharedResourcesLocalizer { get; }
        TestHttpStreamProcessor HttpStreamProcessor { get; }
        Mock<ITaskDelayer> MockTaskDelayer { get; }
        Mock<IDataSourcePublisher> MockDataSourcePublisher { get; }
        Mock<IWorkbookPublisher> MockWorkbookPublisher { get; }
        Mock<IAuthenticationTokenProvider> MockTokenProvider { get; }
        Mock<ITableauServerVersionProvider> MockVersionProvider { get; }
        IRestRequestBuilderFactory RestRequestBuilderFactory { get; }
        IHttpContentSerializer Serializer { get; }
        TableauServerVersion TableauServerVersion { get; }
        TableauSiteConnectionConfiguration SiteConnectionConfiguration { get; }
    }
}