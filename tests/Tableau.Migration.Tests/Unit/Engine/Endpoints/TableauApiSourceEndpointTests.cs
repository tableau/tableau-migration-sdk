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

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content.Files;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints
{
    public class TableauApiSourceEndpointTests
    {
        public class TableauApiSourceEndpointTest : TableauApiEndpointTestBase<TableauApiSourceEndpoint>
        {
            protected readonly TableauApiSourceEndpoint Endpoint;

            public TableauApiSourceEndpointTest()
            {
                Endpoint = new(MigrationServices.GetRequiredService<IServiceScopeFactory>(),
                    Create<ITableauApiEndpointConfiguration>(),
                    Create<ManifestSourceContentReferenceFinderFactory>(),
                    Create<IContentFileStore>(),
                    Create<ISharedResourcesLocalizer>()
                );
            }
        }

        #region - PullAsync -

        public class PullAsync : TableauApiSourceEndpointTest
        {
            private readonly Mock<IPullApiClient<TestContentType, TestPublishType>> _mockPullClient;

            public PullAsync()
            {
                _mockPullClient = Create<Mock<IPullApiClient<TestContentType, TestPublishType>>>();
                MockSiteApi.Setup(x => x.GetPullApiClient<TestContentType, TestPublishType>()).Returns(_mockPullClient.Object);
            }

            [Fact]
            public async Task UsesPullClientAsync()
            {
                var publishResult = Freeze<IResult<TestPublishType>>();

                await Endpoint.InitializeAsync(Cancel);

                var item = new TestContentType();

                var result = await Endpoint.PullAsync<TestContentType, TestPublishType>(item, Cancel);

                Assert.Same(publishResult, result);

                MockSiteApi.Verify(x => x.GetPullApiClient<TestContentType, TestPublishType>(), Times.Once);
                _mockPullClient.Verify(x => x.PullAsync(item, Cancel), Times.Once);
            }
        }

        #endregion
    }
}
