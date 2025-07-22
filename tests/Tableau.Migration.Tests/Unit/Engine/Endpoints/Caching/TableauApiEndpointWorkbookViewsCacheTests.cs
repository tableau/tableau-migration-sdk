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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Caching;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Caching
{
    public sealed class TableauApiEndpointWorkbookViewsCacheTests
    {
        public abstract class TableauApiEndpointWorkbookViewsCacheTest : AutoFixtureTestBase
        {
            protected readonly Mock<IWorkbooksApiClient> MockWorkbooksApiClient;

            protected readonly TableauApiEndpointWorkbookViewsCache Cache;

            public TableauApiEndpointWorkbookViewsCacheTest()
            {
                MockWorkbooksApiClient = Freeze<Mock<IWorkbooksApiClient>>();

                Cache = Create<TableauApiEndpointWorkbookViewsCache>();
            }
        }

        public sealed class FindCacheMissAsync : TableauApiEndpointWorkbookViewsCacheTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                var id = Guid.NewGuid();
                var wb = Create<IWorkbookDetails>();

                MockWorkbooksApiClient.Setup(x => x.GetWorkbookAsync(id, Cancel))
                    .ReturnsAsync(Result<IWorkbookDetails>.Succeeded(wb));

                var result = await Cache.GetOrAddAsync(id, Cancel);

                result.AssertSuccess();
                Assert.Equal(wb.Views, result.Value);

                MockWorkbooksApiClient.Verify(x => x.GetWorkbookAsync(id, Cancel), Times.Once);
            }

            [Fact]
            public async Task FailureAsync()
            {
                var id = Guid.NewGuid();
                var errors = CreateMany<Exception>();

                MockWorkbooksApiClient.Setup(x => x.GetWorkbookAsync(id, Cancel))
                    .ReturnsAsync(Result<IWorkbookDetails>.Failed(errors));

                var result = await Cache.GetOrAddAsync(id, Cancel);

                result.AssertFailure(errors);
                MockWorkbooksApiClient.Verify(x => x.GetWorkbookAsync(id, Cancel), Times.Once);
            }
        }
    }
}
