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
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.ContentClients
{
    public class ApiContentClientFactoryTests

    {
        private readonly Mock<ILoggerFactory> _mockLoggerFactory;
        private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
        private readonly Mock<ISitesApiClient> _mockSitesApiClient;
        private readonly ApiContentClientFactory _factory;

        public ApiContentClientFactoryTests()
        {
            _mockLoggerFactory = new Mock<ILoggerFactory>();
            _mockLocalizer = new Mock<ISharedResourcesLocalizer>();
            _mockSitesApiClient = new Mock<ISitesApiClient>();
            _factory = new ApiContentClientFactory(_mockSitesApiClient.Object, _mockLoggerFactory.Object, _mockLocalizer.Object);
        }

        [Fact]
        public void GetContentClient_ReturnsWorkbookClient()
        {
            // Arrange
            var mockWorkbooksApiClient = new Mock<IWorkbooksApiClient>();
            _mockSitesApiClient.Setup(x => x.Workbooks).Returns(mockWorkbooksApiClient.Object);

            var mockLogger = new Mock<ILogger<WorkbooksContentClient>>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);

            // Act
            var client = _factory.GetContentClient<IWorkbook>();

            // Assert
            Assert.NotNull(client);
            Assert.IsAssignableFrom<IWorkbooksContentClient>(client);
        }

        [Fact]
        public void GetContentClient_ReturnsViewClient()
        {
            // Arrange
            var mockViewsApiClient = new Mock<IViewsApiClient>();
            _mockSitesApiClient.Setup(x => x.Views).Returns(mockViewsApiClient.Object);

            var mockLogger = new Mock<ILogger<ViewsContentClient>>();
            _mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(mockLogger.Object);

            // Act
            var client = _factory.GetContentClient<IView>();

            // Assert
            Assert.NotNull(client);
            Assert.IsAssignableFrom<IViewsContentClient>(client);
        }

        [Fact]
        public void GetContentClient_ThrowsInvalidOperationException_ForUnsupportedType()
        {
            Assert.Throws<InvalidOperationException>(() => _factory.GetContentClient<string>());
        }
    }
}
