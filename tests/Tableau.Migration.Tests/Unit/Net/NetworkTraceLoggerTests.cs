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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Net;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class NetworkTraceLoggerTests
    {
        public abstract class NetworkTraceLoggerTestsBase
        {
            private const LogLevel DefaultLogLevel = LogLevel.Information;
            private const LogLevel ExceptionLogLevel = LogLevel.Error;

            private readonly Mock<ILogger<NetworkTraceLogger>> _mockLogger = new();
            internal readonly Mock<IConfigReader> _mockConfigReader = new();
            internal readonly Mock<ISharedResourcesLocalizer> _mockLocalizer = new();
            internal readonly Mock<INetworkTraceRedactor> _mockTraceRedactor = new();
            internal readonly MigrationSdkOptions _sdkOptions = new();
            internal readonly NetworkTraceLogger _traceLogger;

            public NetworkTraceLoggerTestsBase()
            {
                _mockConfigReader.Setup(x => x.Get()).Returns(_sdkOptions);

                _traceLogger = new NetworkTraceLogger(
                    _mockLogger.Object,
                    _mockConfigReader.Object,
                    _mockLocalizer.Object,
                    _mockTraceRedactor.Object);
            }

            protected void VerifyDefaultLogging()
            {
                _mockLogger.VerifyLogging(DefaultLogLevel, Times.Once);
                _mockLogger.VerifyLogging(ExceptionLogLevel, Times.Never);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceLogMessage], Times.Once);
            }

            protected void VerifyExceptionLogging()
            {
                _mockLogger.VerifyLogging(DefaultLogLevel, Times.Never);
                _mockLogger.VerifyLogging(ExceptionLogLevel, Times.Once);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
            }

            protected void VerifyNetworkTraceRedactor()
            {
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            protected void VerifyLocalizerInvocationCount(int count)
                => _mockLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(count));
        }

        public class WriteNetworkLogsAsync : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task WriteDefaultLogs()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage();

                // Act
                await _traceLogger.WriteNetworkLogsAsync(request, response, CancellationToken.None);

                // Assert
                VerifyDefaultLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();
            }
        }

        public class WriteHeadersForLogs : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkLogsAsync(request, response, CancellationToken.None);

                // Assert
                VerifyDefaultLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableHeadersDetailsWithOnlyDisallowedHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", "test");
                var response = new HttpResponseMessage();
                response.Headers.Date = DateTimeOffset.UtcNow;
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkLogsAsync(request, response, CancellationToken.None);

                // Assert
                VerifyDefaultLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableHeadersDetailsWithAcceptHeader()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                var response = new HttpResponseMessage();
                response.Headers.CacheControl = new CacheControlHeaderValue
                {
                    NoCache = true
                };
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkLogsAsync(request, response, CancellationToken.None);

                // Assert
                VerifyDefaultLogging();
                VerifyLocalizerInvocationCount(3);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionResponseHeaders], Times.Once);
                VerifyNetworkTraceRedactor();

            }
        }

        public class WriteExceptionLogs : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task WriteDefaultLogs()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableExceptionDetails()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.ExceptionsLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionException], Times.Once);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithOnlyDisallowedHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", "test");
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithAcceptHeader()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                VerifyNetworkTraceRedactor();

            }
        }

        public class WriteHeadersForExceptionLogs : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableHeadersDetailsWithOnlyDisallowedHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Authorization = new AuthenticationHeaderValue("bearer", "test");
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableHeadersDetailsWithAcceptHeader()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                VerifyNetworkTraceRedactor();
            }
        }

        public class WriteContentHeadersForExceptionLogs : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.Clear();
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithOnlyDisallowedHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.Clear();
                request.Content.Headers.Add("bearer", "test");
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();
            }

            [Fact]
            public async Task EnableHeadersDetailsWithContentTypeHeader()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.Clear();
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                VerifyNetworkTraceRedactor();

            }
        }

        public class WriteRequestContentForExceptionLogs : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableContentDetailsWithoutContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(1);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyPdfContent()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(3);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Once);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableBinaryContentDetailsWithEmptyPdfContent()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;
                _sdkOptions.Network.BinaryContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyTextContent()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new StringContent(string.Empty)
                };
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }
        }

        public class WriteMultipartRequestContentForExceptionLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableContentDetailsWithoutContent()
            {
                // Arrange
                var request = new HttpRequestMessage
                {
                    Content = new MultipartFormDataContent()
                };
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                VerifyNetworkTraceRedactor();
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyPdfContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                multipart.Add(content);
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(3);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Once);
                VerifyNetworkTraceRedactor();

            }

            [Fact]
            public async Task EnableContentDetailsTooLarge()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentLength = (long)int.MaxValue + 1;
                multipart.Add(content);
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;
                _sdkOptions.Network.BinaryContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(3);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Never);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceTooLargeDetails], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);

            }

            [Fact]
            public async Task EnableBinaryContentDetailsWithEmptyPdfContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                multipart.Add(content);
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;
                _sdkOptions.Network.BinaryContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyTextContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                multipart.Add(content);
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipleContentDetails()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                multipart.Add(content);
                multipart.Add(new StringContent(string.Empty));
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(3);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipleContentDetailsWithSensitiveData()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("test")
                {
                    Name = "password"
                };
                multipart.Add(content);
                multipart.Add(new StringContent(string.Empty));
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;
                _sdkOptions.Network.BinaryContentLoggingEnabled = true;
                _mockTraceRedactor
                    .Setup(x => x.IsSensitiveMultipartContent("password"))
                    .Returns(true);

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(request, exception, CancellationToken.None);

                // Assert
                VerifyExceptionLogging();
                VerifyLocalizerInvocationCount(2);

                _mockLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Exactly(2));
                _mockTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }
        }
    }
}
