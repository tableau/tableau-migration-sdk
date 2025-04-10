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
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.ContentClients
{
    public class ViewsContentClientTests
    {
        private readonly Mock<IViewsApiClient> _mockViewsApiClient;
        private readonly Mock<ILogger<IViewsContentClient>> _mockLogger;
        private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
        private readonly ViewsContentClient _viewsContentClient;

        public ViewsContentClientTests()
        {
            _mockViewsApiClient = new Mock<IViewsApiClient>();
            _mockLogger = new Mock<ILogger<IViewsContentClient>>();
            _mockLocalizer = new Mock<ISharedResourcesLocalizer>();
            _viewsContentClient = new ViewsContentClient(_mockViewsApiClient.Object, _mockLogger.Object, _mockLocalizer.Object);
        }

        [Fact]
        public async Task GetByIdAsync_Success()
        {
            // Arrange
            var viewId = Guid.NewGuid();
            var mockView = new Mock<IView>();
            mockView.Setup(x => x.Id).Returns(viewId);
            var viewResult = Result<IView>.Succeeded(Mock.Of<IView>());
            _mockViewsApiClient.Setup(x => x.GetByIdAsync(viewId, It.IsAny<CancellationToken>())).ReturnsAsync(viewResult);

            // Act
            var result = await _viewsContentClient.GetByIdAsync(viewId, CancellationToken.None);

            // Assert
            Assert.Equal(viewResult, result);
            _mockViewsApiClient.Verify(x => x.GetByIdAsync(viewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_Error()
        {
            // Arrange
            var viewId = Guid.NewGuid();
            var viewResult = Result<IView>.Failed(new Exception("Failed"));
            _mockViewsApiClient.Setup(x => x.GetByIdAsync(viewId, It.IsAny<CancellationToken>())).ReturnsAsync(viewResult);

            // Act
            var result = await _viewsContentClient.GetByIdAsync(viewId, CancellationToken.None);

            // Assert
            Assert.Equal(viewResult, result);
            _mockViewsApiClient.Verify(x => x.GetByIdAsync(viewId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
