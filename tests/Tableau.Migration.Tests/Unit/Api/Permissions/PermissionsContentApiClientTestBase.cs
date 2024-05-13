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

using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Permissions;

namespace Tableau.Migration.Tests.Unit.Api.Permissions
{
    public abstract class PermissionsContentApiClientTestBase : ApiTestBase
    {
        internal readonly Mock<IPermissionsApiClient> MockPermissionsClient = new();

        public string UrlPrefix { get; }

        public PermissionsContentApiClientTestBase()
        {
            UrlPrefix = Create<string>();
            Setup(MockPermissionsClientFactory, MockPermissionsClient);
        }

        internal static void Setup(Mock<IPermissionsApiClientFactory> mockFactory, Mock<IPermissionsApiClient> mockClient)
        {
            mockFactory
                .Setup(f => f.Create(It.IsAny<IContentApiClient>()))
                .Returns(mockClient.Object);
        }
    }

    public abstract class PermissionsApiClientTestBase<TApiClient> : ApiClientTestBase<TApiClient>
        where TApiClient : class, IContentApiClient, IPermissionsContentApiClient
    {
        internal readonly Mock<IPermissionsApiClient> MockPermissionsClient = new();

        public PermissionsApiClientTestBase()
        {
            PermissionsContentApiClientTestBase.Setup(MockPermissionsClientFactory, MockPermissionsClient);
        }
    }
}
