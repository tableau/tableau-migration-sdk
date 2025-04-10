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
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.ContentClients
{
    public class WorkbooksContentClientTests
    {
        private readonly Mock<IWorkbooksApiClient> _mockWorkbooksApiClient;
        private readonly Mock<ILogger<IWorkbooksContentClient>> _mockLogger;
        private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
        private readonly WorkbooksContentClient _workbooksContentClient;

        public WorkbooksContentClientTests()
        {
            _mockWorkbooksApiClient = new Mock<IWorkbooksApiClient>();
            _mockLogger = new Mock<ILogger<IWorkbooksContentClient>>();
            _mockLocalizer = new Mock<ISharedResourcesLocalizer>();
            _workbooksContentClient = new WorkbooksContentClient(_mockWorkbooksApiClient.Object, _mockLogger.Object, _mockLocalizer.Object);
        }

        [Fact]
        public async Task GetViewsForWorkbookIdAsync_Success()
        {
            // Arrange
            var workbookId = Guid.NewGuid();
            var mockWorkbook = new Mock<IWorkbookDetails>();
            mockWorkbook.Setup(x => x.Id).Returns(workbookId);
            var mockViews = ImmutableList.Create(Mock.Of<IView>());
            mockWorkbook.Setup(x => x.Views).Returns(mockViews);

            var workbookResult = Result<IWorkbookDetails>.Succeeded(mockWorkbook.Object);

            _mockWorkbooksApiClient.Setup(x => x.GetWorkbookAsync(workbookId, It.IsAny<CancellationToken>())).ReturnsAsync(workbookResult);

            // Act
            var result = await _workbooksContentClient.GetViewsForWorkbookIdAsync(workbookId, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(mockViews, result.Value);
            _mockWorkbooksApiClient.Verify(x => x.GetWorkbookAsync(workbookId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetViewsForWorkbookIdAsync_Error()
        {
            // Arrange
            var workbookId = Guid.NewGuid();
            var workbookResult = Result<IWorkbookDetails>.Failed(new Exception("Failed"));
            _mockWorkbooksApiClient.Setup(x => x.GetWorkbookAsync(workbookId, It.IsAny<CancellationToken>())).ReturnsAsync(workbookResult);

            // Act
            var result = await _workbooksContentClient.GetViewsForWorkbookIdAsync(workbookId, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            _mockWorkbooksApiClient.Verify(x => x.GetWorkbookAsync(workbookId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
