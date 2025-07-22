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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Logging;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Logging
{
    public class HttpActivityLoggerTests
    {
        #region - Test Classes -

        public abstract class HttpActivityLoggerTestsBase : AutoFixtureTestBase
        {
            private const LogLevel DefaultLogLevel = LogLevel.Information;
            private const LogLevel DefaultUnsuccessfulReqLogLevel = LogLevel.Warning;
            private const LogLevel ExceptionLogLevel = LogLevel.Error;

            private readonly Mock<ILogger<HttpActivityLogger>> _mockLogger;

            internal readonly Mock<IHttpContentRedactor> MockContentRedactor;
            internal readonly Mock<IConfigReader> MockConfigReader;
            internal readonly Mock<ISharedResourcesLocalizer> MockLocalizer;

            internal readonly MigrationSdkOptions SdkOptions = new();

            internal readonly HttpActivityLogger HttpLogger;

            public HttpActivityLoggerTestsBase()
            {
                MockContentRedactor = Freeze<Mock<IHttpContentRedactor>>();

                MockConfigReader = Freeze<Mock<IConfigReader>>();
                MockConfigReader.Setup(x => x.Get()).Returns(() => SdkOptions);

                MockLocalizer = Freeze<Mock<ISharedResourcesLocalizer>>();

                _mockLogger = Freeze<Mock<ILogger<HttpActivityLogger>>>();

                HttpLogger = Create<HttpActivityLogger>();
            }

            protected void VerifyDefaultLogging(bool isSucess = true)
            {
                if (isSucess)
                {
                    _mockLogger.VerifyLogging(DefaultLogLevel, Times.Once);
                }
                else
                {
                    _mockLogger.VerifyLogging(DefaultUnsuccessfulReqLogLevel, Times.Once);
                }
                _mockLogger.VerifyLogging(ExceptionLogLevel, Times.Never);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpActivityLogMessage], Times.Once);
            }

            protected void VerifyExceptionLogging()
            {
                _mockLogger.VerifyLogging(DefaultLogLevel, Times.Never);
                _mockLogger.VerifyLogging(ExceptionLogLevel, Times.Once);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpActivityExceptionLogMessage], Times.Once);
            }

            protected void VerifyHttpContentRedactor()
            {
                MockContentRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                MockContentRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            protected void VerifyLocalizerInvocationCount(int count)
                => MockLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(count));
        }

        #endregion

        #region - LogRequestStarted -

        public class LogRequestStarted : HttpActivityLoggerTestsBase
        {
            [Fact]
            public void RequestLoggingDisabled()
            {
                var request = new HttpRequestMessage();

                SdkOptions.Network.RequestsLoggingEnabled = false;

                HttpLogger.LogRequestStarted(request);

                MockLocalizer.Verify(x => x[It.IsAny<string>()], Times.Never);
            }

            [Fact]
            public void RequestLoggingEnabled()
            {
                var request = new HttpRequestMessage();

                SdkOptions.Network.RequestsLoggingEnabled = true;

                HttpLogger.LogRequestStarted(request);

                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpActivityRequestLogMessage], Times.Once);
                MockLocalizer.VerifyNoOtherCalls();
            }
        }

        #endregion

        #region - LogResponseAsync -

        public class LogResponseAsync : HttpActivityLoggerTestsBase
        {
            [Fact]
            public async Task WriteDefaultLogsAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage();

                // Act
                await HttpLogger.LogResponseAsync(request, response, Create<TimeSpan>(), Cancel);

                // Assert
                VerifyDefaultLogging();
                VerifyLocalizerInvocationCount(2);
                VerifyHttpContentRedactor();
            }

            [Fact]
            public async Task WriteDefaultWarningLogsAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest };

                // Act
                await HttpLogger.LogResponseAsync(request, response, Create<TimeSpan>(), Cancel);

                // Assert
                VerifyDefaultLogging(false);
                VerifyLocalizerInvocationCount(2);
                VerifyHttpContentRedactor();
            }
        }

        #endregion

        #region - LogExceptionAsync -

        public class LogExceptionAsync : HttpActivityLoggerTestsBase
        {
            [Fact]
            public async Task WriteDefaultLogsAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();

                // Act
                await HttpLogger.LogExceptionAsync(request, exception, Create<TimeSpan>(), CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                VerifyHttpContentRedactor();
            }

            [Fact]
            public async Task EnableExceptionDetailsAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                SdkOptions.Network.ExceptionsLoggingEnabled = true;

                // Act
                await HttpLogger.LogExceptionAsync(request, exception, Create<TimeSpan>(), CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpExceptionSectionName], Times.Once);
                VerifyHttpContentRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithoutHeadersAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                SdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await HttpLogger.LogExceptionAsync(request, exception, Create<TimeSpan>(), CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                VerifyHttpContentRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithOnlyDisallowedHeadersAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", "test");
                var exception = new Exception();
                SdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await HttpLogger.LogExceptionAsync(request, exception, Create<TimeSpan>(), CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                VerifyHttpContentRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithAcceptHeaderAsync()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                var exception = new Exception();
                SdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await HttpLogger.LogExceptionAsync(request, exception, Create<TimeSpan>(), CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);

                MockLocalizer.Verify(x => x[SharedResourceKeys.HttpRequestHeadersSectionName], Times.Once);
                VerifyHttpContentRedactor();
            }
        }

        #endregion
    }
}
