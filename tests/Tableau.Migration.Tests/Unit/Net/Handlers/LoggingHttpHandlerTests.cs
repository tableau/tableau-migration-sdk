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
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Net.Handlers;
using Tableau.Migration.Net.Logging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class LoggingHttpHandlerTests
    {
        private readonly Mock<IHttpActivityLogger> _mockActivityLogger;

        public LoggingHttpHandlerTests()
        {
            _mockActivityLogger = new Mock<IHttpActivityLogger>();
        }

        [Fact]
        public void SuccessfullySendAsyncCallTraceLogger()
        {
            // Arrange
            var handler = new LoggingHttpHandler(_mockActivityLogger.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(LoggingHttpHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();

            // Act
            var task = (Task<HttpResponseMessage>)methodInfo.Invoke(handler, new object[] { message, CancellationToken.None })!;

            // Assert
            _mockActivityLogger.Verify(x => x.LogRequestStarted(It.IsAny<HttpRequestMessage>()), Times.Once);
            _mockActivityLogger.Verify(x => x.LogResponseAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<HttpResponseMessage>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
            _mockActivityLogger.Verify(x => x.LogExceptionAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<Exception>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
            Assert.Null(task.Exception);
        }

        [Fact]
        public void SendAsyncWithExceptionCallTraceLogger()
        {
            // Arrange
            var handler = new LoggingHttpHandler(_mockActivityLogger.Object);
            var exception = new Exception("Failed to Send");
            var innerHandler = new MockDelegatingHandler(_ => throw exception);
            handler.InnerHandler = innerHandler;
            var methodInfo = typeof(LoggingHttpHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();

            // Act
            var task = (Task<HttpResponseMessage>)methodInfo.Invoke(handler, new object[] { message, CancellationToken.None })!;

            // Assert
            _mockActivityLogger.Verify(x => x.LogRequestStarted(It.IsAny<HttpRequestMessage>()), Times.Once);
            _mockActivityLogger.Verify(x => x.LogResponseAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<HttpResponseMessage>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Never());
            _mockActivityLogger.Verify(x => x.LogExceptionAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<Exception>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.NotNull(task.Exception);
            Assert.NotNull(task.Exception.InnerException);
            Assert.Same(exception, task.Exception.InnerException);
        }
    }
}
