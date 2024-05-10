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
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class ScopedApiClientFactoryTests
    {
        public class Initialize : AutoFixtureTestBase
        {
            [Fact]
            public void InitializesInput()
            {
                var mockApiInput = Freeze<Mock<IApiClientInputInitializer>>();
                var mockRestInput = Freeze<Mock<IRequestBuilderFactoryInputInitializer>>();
                var apiClient = Freeze<IApiClient>();
                var mockServices = new MockServiceProvider(AutoFixture);

                var factory = new ScopedApiClientFactory(mockServices.Object);

                var config = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "mySite", "tokenName", "token");
                var result = factory.Initialize(config);

                Assert.Same(apiClient, result);

                mockApiInput.Verify(x => x.Initialize(config, null, null), Times.Once);

                mockRestInput.Verify(x => x.Initialize(config.ServerUrl), Times.Once);

                mockServices.Verify(x => x.GetService(typeof(IApiClientInputInitializer)), Times.Once);
                mockServices.Verify(x => x.GetService(typeof(IRequestBuilderFactoryInputInitializer)), Times.Once);
                mockServices.Verify(x => x.GetService(typeof(IApiClient)), Times.Once);
            }

            [Fact]
            public void FinderFactoryOverride()
            {
                var finderFactory = Freeze<IContentReferenceFinderFactory>();
                var mockApiInput = Freeze<Mock<IApiClientInputInitializer>>();
                var mockRestInput = Freeze<Mock<IRequestBuilderFactoryInputInitializer>>();
                var apiClient = Freeze<IApiClient>();
                var mockServices = new MockServiceProvider(AutoFixture);

                var factory = new ScopedApiClientFactory(mockServices.Object);

                var config = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "mySite", "tokenName", "token");
                var result = factory.Initialize(config, finderFactory);

                Assert.Same(apiClient, result);

                mockApiInput.Verify(x => x.Initialize(config, finderFactory, null), Times.Once);
            }

            [Fact]
            public void FileStoreOverride()
            {
                var fileStore = Freeze<IContentFileStore>();
                var mockApiInput = Freeze<Mock<IApiClientInputInitializer>>();
                var mockRestInput = Freeze<Mock<IRequestBuilderFactoryInputInitializer>>();
                var apiClient = Freeze<IApiClient>();
                var mockServices = new MockServiceProvider(AutoFixture);

                var factory = new ScopedApiClientFactory(mockServices.Object);

                var config = new TableauSiteConnectionConfiguration(new Uri("https://localhost"), "mySite", "tokenName", "token");
                var result = factory.Initialize(config, null, fileStore);

                Assert.Same(apiClient, result);

                mockApiInput.Verify(x => x.Initialize(config, null, fileStore), Times.Once);
            }
        }
    }
}
