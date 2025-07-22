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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.ContentClients
{
    public sealed class WorkbooksContentClientTests : AutoFixtureTestBase
    {
        private readonly Mock<IEndpointWorkbookViewsCache> _mockCache;
        private readonly WorkbooksContentClient _workbooksContentClient;

        public WorkbooksContentClientTests()
        {
            _mockCache = Freeze<Mock<IEndpointWorkbookViewsCache>>();
            _workbooksContentClient = Create<WorkbooksContentClient>();
        }

        [Fact]
        public async Task CachesWorkbookViewsAsync()
        {
            // Arrange
            var workbookId = Guid.NewGuid();
            var cacheResult = Result<IImmutableList<IView>>.Succeeded(CreateMany<IView>().ToImmutableArray());

            _mockCache.Setup(x => x.GetOrAddAsync(workbookId, Cancel)).ReturnsAsync(cacheResult);

            // Act
            var result = await _workbooksContentClient.GetViewsForWorkbookIdAsync(workbookId, Cancel);

            // Assert
            Assert.Same(cacheResult, result);
            _mockCache.Verify(x => x.GetOrAddAsync(workbookId, Cancel), Times.Once);
        }
    }
}
