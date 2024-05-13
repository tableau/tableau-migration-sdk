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

using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Net;
using Tableau.Migration.Net.Handlers;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Handlers
{
    public class LoggingHandlerTests
    {
        private readonly Mock<INetworkTraceLogger> _mockedTraceLogger;

        public LoggingHandlerTests()
        {
            _mockedTraceLogger = new Mock<INetworkTraceLogger>();
        }

        [Fact]
        public void SuccessfullySendAsync_CallTraceLogger()
        {
            // Arrange
            var handler = new LoggingHandler(_mockedTraceLogger.Object);
            handler.InnerHandler = Mock.Of<HttpMessageHandler>();
            var methodInfo = typeof(LoggingHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();

            // Act
            var task = (Task<HttpResponseMessage>)methodInfo.Invoke(handler, new object[] { message, CancellationToken.None })!;

            // Assert
            _mockedTraceLogger.Verify(x => x.WriteNetworkLogsAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<HttpResponseMessage>(),
                It.IsAny<CancellationToken>()),
                Times.Once());
            _mockedTraceLogger.Verify(x => x.WriteNetworkExceptionLogsAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<Exception>(),
                It.IsAny<CancellationToken>()),
                Times.Never());
            Assert.Null(task.Exception);
        }

        [Fact]
        public void SendAsyncWithException_CallTraceLogger()
        {
            // Arrange
            var handler = new LoggingHandler(_mockedTraceLogger.Object);
            var exception = new Exception("Failed to Send");
            var innerHandler = new MockDelegatingHandler(_ => throw exception);
            handler.InnerHandler = innerHandler;
            var methodInfo = typeof(LoggingHandler).GetMethod("SendAsync", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var message = new HttpRequestMessage();

            // Act
            var task = (Task<HttpResponseMessage>)methodInfo.Invoke(handler, new object[] { message, CancellationToken.None })!;

            // Assert
            _mockedTraceLogger.Verify(x => x.WriteNetworkLogsAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<HttpResponseMessage>(),
                It.IsAny<CancellationToken>()),
                Times.Never());
            _mockedTraceLogger.Verify(x => x.WriteNetworkExceptionLogsAsync(
                It.IsAny<HttpRequestMessage>(),
                It.IsAny<Exception>(),
                It.IsAny<CancellationToken>()),
                Times.Once());
            Assert.NotNull(task.Exception);
            Assert.NotNull(task.Exception.InnerException);
            Assert.Same(exception, task.Exception.InnerException);
        }
    }
}
