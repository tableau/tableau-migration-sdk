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
            internal readonly Mock<ILogger<NetworkTraceLogger>> _mockedLogger = new();
            internal readonly Mock<IConfigReader> _mockedConfigReader = new();
            internal readonly Mock<ISharedResourcesLocalizer> _mockedLocalizer = new();
            internal readonly Mock<INetworkTraceRedactor> _mockedTraceRedactor = new();
            internal readonly MigrationSdkOptions _sdkOptions = new();
            internal readonly NetworkTraceLogger _traceLogger;

            public NetworkTraceLoggerTestsBase()
            {
                _mockedConfigReader
                    .Setup(x => x.Get())
                    .Returns(_sdkOptions);
                _traceLogger = new NetworkTraceLogger(
                    _mockedLogger.Object,
                    _mockedConfigReader.Object,
                    _mockedLocalizer.Object,
                    _mockedTraceRedactor.Object);
            }
        }

        public class WriteNetworkLogsAsync
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task WriteDefaultLogs()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage();

                // Act
                await _traceLogger.WriteNetworkLogsAsync(
                    request,
                    response,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Once);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Never);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }
        }

        public class WriteHeadersForLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var response = new HttpResponseMessage();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkLogsAsync(
                    request,
                    response,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Once);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Never);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkLogsAsync(
                    request,
                    response,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Once);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Never);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkLogsAsync(
                    request,
                    response,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Once);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Never);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(3));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionResponseHeaders], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }
        }

        public class WriteExceptionLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task WriteDefaultLogs()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task EnableExceptionDetails()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.ExceptionsLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionException], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }
        }

        public class WriteHeadersForExceptionLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }
        }

        public class WriteContentHeadersForExceptionLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableHeadersDetailsWithoutHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new StringContent(string.Empty);
                request.Content.Headers.Clear();
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task EnableHeadersDetailsWithOnlyDisallowedHeaders()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new StringContent(string.Empty);
                request.Content.Headers.Clear();
                request.Content.Headers.Add("bearer", "test");
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task EnableHeadersDetailsWithContentTypeHeader()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new StringContent(string.Empty);
                request.Content.Headers.Clear();
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var exception = new Exception();
                _sdkOptions.Network.HeadersLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestHeaders], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }
        }

        public class WriteRequestContentForExceptionLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableContentDetailsWithoutContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyPdfContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new StringContent(string.Empty);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(3));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
            }

            [Fact]
            public async Task EnableBinaryContentDetailsWithEmptyPdfContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new StringContent(string.Empty);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;
                _sdkOptions.Network.BinaryContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableContentDetailsWithEmptyTextContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new StringContent(string.Empty);
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }
        }

        public class WriteMultipartRequestContentForExceptionLogs
            : NetworkTraceLoggerTestsBase
        {
            [Fact]
            public async Task EnableContentDetailsWithoutContent()
            {
                // Arrange
                var request = new HttpRequestMessage();
                request.Content = new MultipartFormDataContent();
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(3));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Never);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(3));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Never);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceTooLargeDetails], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Never);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
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
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(3));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceNotDisplayedDetails], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Once);
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }

            [Fact]
            public async Task EnableMultipleContentDetailsWithSensitiveData()
            {
                // Arrange
                var request = new HttpRequestMessage();
                var multipart = new MultipartFormDataContent();
                var content = new StringContent(string.Empty);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("test");
                content.Headers.ContentDisposition.Name = "password";
                multipart.Add(content);
                multipart.Add(new StringContent(string.Empty));
                request.Content = multipart;
                var exception = new Exception();
                _sdkOptions.Network.ContentLoggingEnabled = true;
                _sdkOptions.Network.BinaryContentLoggingEnabled = true;
                _mockedTraceRedactor
                    .Setup(x => x.IsSensitiveMultipartContent("password"))
                    .Returns(true);

                // Act
                await _traceLogger.WriteNetworkExceptionLogsAsync(
                    request,
                    exception,
                    CancellationToken.None);

                // Assert
                _mockedLogger.VerifyLogging(LogLevel.Information, Times.Never);
                _mockedLogger.VerifyLogging(LogLevel.Error, Times.Once);
                _mockedLocalizer.Verify(x => x[It.IsAny<string>()], Times.Exactly(2));
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.NetworkTraceExceptionLogMessage], Times.Once);
                _mockedLocalizer.Verify(x => x[SharedResourceKeys.SectionRequestContent], Times.Once);
                _mockedTraceRedactor.Verify(x => x.IsSensitiveMultipartContent(It.IsAny<string>()), Times.Exactly(2));
                _mockedTraceRedactor.Verify(x => x.ReplaceSensitiveData(It.IsAny<string>()), Times.Once);
            }
        }
    }
}
