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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.ContentClients
{
    public sealed class ViewsContentClientTests
    {
        public abstract class ViewsContentClientTest
        {
            protected readonly Mock<IEndpointViewCache> MockViewsCache;
            protected readonly Mock<ILogger<ViewsContentClient>> MockLogger;
            protected readonly Mock<ISharedResourcesLocalizer> MockLocalizer;
            protected readonly ViewsContentClient ViewsContentClient;

            public ViewsContentClientTest()
            {
                MockViewsCache = new();
                MockLogger = new();
                MockLocalizer = new();
                ViewsContentClient = new ViewsContentClient(MockViewsCache.Object, MockLogger.Object, MockLocalizer.Object);
            }
        }

        public sealed class GetById : ViewsContentClientTest
        {
            [Fact]
            public async Task SuccessAsync()
            {
                // Arrange
                var viewId = Guid.NewGuid();
                var mockView = new Mock<IView>();
                mockView.Setup(x => x.Id).Returns(viewId);
                var viewResult = Result<IView>.Succeeded(Mock.Of<IView>());
                MockViewsCache.Setup(x => x.GetOrAddAsync(viewId, It.IsAny<CancellationToken>())).ReturnsAsync(viewResult);

                // Act
                var result = await ViewsContentClient.GetByIdAsync(viewId, CancellationToken.None);

                // Assert
                Assert.Equal(viewResult, result);
                MockViewsCache.Verify(x => x.GetOrAddAsync(viewId, It.IsAny<CancellationToken>()), Times.Once);
            }

            [Fact]
            public async Task ErrorAsync()
            {
                // Arrange
                var viewId = Guid.NewGuid();
                var viewResult = Result<IView>.Failed(new Exception("Failed"));
                MockViewsCache.Setup(x => x.GetOrAddAsync(viewId, It.IsAny<CancellationToken>())).ReturnsAsync(viewResult);

                // Act
                var result = await ViewsContentClient.GetByIdAsync(viewId, CancellationToken.None);

                // Assert
                Assert.Equal(viewResult, result);
                MockViewsCache.Verify(x => x.GetOrAddAsync(viewId, It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
