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

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Permissions;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Rest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public class PermissionsApiClientFactoryTests
    {
        public abstract class PermissionsApiClientFactoryTest : AutoFixtureTestBase
        {
            protected readonly Mock<IRestRequestBuilderFactory> MockRestRequestBuilderFactory = new();
            protected readonly Mock<IHttpContentSerializer> MockSerializer = new();
            protected readonly Mock<ISharedResourcesLocalizer> MockSharedResourcesLocalizer = new();
            protected readonly Mock<IConfigReader> MockConfigReader = new();
            protected readonly Mock<ILoggerFactory> MockLoggerFactory = new();

            protected readonly Mock<IPermissionsUriBuilder> MockUriBuilder = new();

            internal readonly PermissionsApiClientFactory Factory;

            public PermissionsApiClientFactoryTest()
            {
                Factory = new(
                    MockRestRequestBuilderFactory.Object,
                    MockSerializer.Object,
                    MockSharedResourcesLocalizer.Object,
                    MockConfigReader.Object,
                    MockLoggerFactory.Object);
            }
        }

        public class Create : PermissionsApiClientFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var client = Factory.Create(MockUriBuilder.Object);

                Assert.Same(MockRestRequestBuilderFactory.Object, client.GetFieldValue("_restRequestBuilderFactory"));
                Assert.Same(MockSerializer.Object, client.GetFieldValue("_serializer"));
                Assert.Same(MockUriBuilder.Object, client.GetFieldValue("_uriBuilder"));
                Assert.Same(MockSharedResourcesLocalizer.Object, client.GetFieldValue("_sharedResourcesLocalizer"));
            }
        }

        public class CreateDefaultPermissionsClient : PermissionsApiClientFactoryTest
        {
            [Fact]
            public void Creates()
            {
                var sdkOptions = new MigrationSdkOptions
                {
                    DefaultPermissionsContentTypes = new(CreateMany<string>(10))
                };

                MockConfigReader.Setup(r => r.Get()).Returns(sdkOptions);

                var client = Factory.CreateDefaultPermissionsClient();

                Assert.Same(Factory, client.GetFieldValue("_permissionsClientFactory"));

                var contentTypeClients = Assert.IsType<ConcurrentDictionary<string, IPermissionsApiClient>>(client.GetFieldValue("_contentTypeClients"));

                foreach (var contentTypeUrlSegment in sdkOptions.DefaultPermissionsContentTypes.UrlSegments)
                    Assert.Contains(contentTypeUrlSegment, contentTypeClients.Keys);
            }
        }
    }
}
