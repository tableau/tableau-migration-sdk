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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Config;
using Tableau.Migration.Resources;
using static Tableau.Migration.Net.NetworkTraceRedactor;

namespace Tableau.Migration.Net
{
    internal class NetworkTraceLogger
        : INetworkTraceLogger
    {
        /// <summary>
        /// List of headers to log.
        /// </summary>
        private static readonly HashSet<string> _logAllowedHeaders = new(
            new[]
            {
                "Accept",
                "Cache-Control",
                "Content-Disposition",
                "Content-Length",
                "Content-Type"   ,
                "Etag",
                "Expires",
                "Last-Modified",
                "Status",
                "User-Agent"
            },
            StringComparer.OrdinalIgnoreCase);

        private readonly ILogger<NetworkTraceLogger> _logger;
        private readonly IConfigReader _configReader;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly INetworkTraceRedactor _traceRedactor;

        public NetworkTraceLogger(
            ILogger<NetworkTraceLogger> logger,
            IConfigReader configReader,
            ISharedResourcesLocalizer localizer,
            INetworkTraceRedactor traceRedactor)
        {
            _logger = logger;
            _configReader = configReader;
            _localizer = localizer;
            _traceRedactor = traceRedactor;
        }

        public async Task WriteNetworkLogsAsync(
            HttpRequestMessage request,
            HttpResponseMessage response,
            CancellationToken cancel)
        {
            var detailsBuilder = new StringBuilder();

            AddHttpHeaderDetails(
                detailsBuilder,
                request,
                response);

            await AddHttpContentDetailsAsync(
                detailsBuilder,
                request,
                response,
                cancel)
                .ConfigureAwait(false);

            _logger.LogInformation(
                _localizer[SharedResourceKeys.NetworkTraceLogMessage],
                request.Method,
                request.RequestUri,
                response.StatusCode,
                detailsBuilder.ToString());
        }

        public async Task WriteNetworkExceptionLogsAsync(
            HttpRequestMessage request,
            Exception exception,
            CancellationToken cancel)
        {
            var detailsBuilder = new StringBuilder();

            AddHttpHeaderDetails(detailsBuilder, request);

            await AddHttpContentDetailsAsync(
                detailsBuilder,
                request,
                null,
                cancel)
                .ConfigureAwait(false);

            AddHttpExceptionDetails(detailsBuilder, exception);

            _logger.LogError(
                _localizer[SharedResourceKeys.NetworkTraceExceptionLogMessage],
                request.Method,
                request.RequestUri,
                exception.Message,
                detailsBuilder.ToString());
        }

        private void AddHttpHeaderDetails(
            StringBuilder detailsBuilder,
            HttpRequestMessage request,
            HttpResponseMessage? response = null)
        {
            var headersLogging = _configReader.Get().Network.HeadersLoggingEnabled;

            if (!headersLogging)
            {
                return;
            }

            var requestSectionWritten = AppendHttpHeader(
                detailsBuilder,
                request.Headers,
                SharedResourceKeys.SectionRequestHeaders);
            AppendHttpHeader(
                detailsBuilder,
                request.Content?.Headers,
                SharedResourceKeys.SectionRequestHeaders,
                requestSectionWritten);

            var responseSectionWritten = AppendHttpHeader(
                detailsBuilder,
                response?.Headers,
                SharedResourceKeys.SectionResponseHeaders);
            AppendHttpHeader(
                detailsBuilder,
                response?.Content?.Headers,
                SharedResourceKeys.SectionResponseHeaders,
                responseSectionWritten);
        }

        private async Task AddHttpContentDetailsAsync(
            StringBuilder detailsBuilder,
            HttpRequestMessage request,
            HttpResponseMessage? response,
            CancellationToken cancellation)
        {
            var contentLogging = _configReader.Get().Network.ContentLoggingEnabled;

            if (!contentLogging)
            {
                return;
            }

            await AppendHttpContentAsync(
                detailsBuilder,
                request.Content,
                SharedResourceKeys.SectionRequestContent,
                cancellation)
                .ConfigureAwait(false);
            await AppendHttpContentAsync(
                detailsBuilder,
                response?.Content,
                SharedResourceKeys.SectionResponseContent,
                cancellation)
                .ConfigureAwait(false);
        }

        private void AddHttpExceptionDetails(
            StringBuilder detailsBuilder,
            Exception exception)
        {
            var exceptionLogging = _configReader.Get().Network.ExceptionsLoggingEnabled;

            if (!exceptionLogging)
            {
                return;
            }

            detailsBuilder.AppendLine(
                _localizer[SharedResourceKeys.SectionException]);

            detailsBuilder.AppendLine(exception.StackTrace);
        }

        private bool AppendHttpHeader(
            StringBuilder detailsBuilder,
            HttpHeaders? headers,
            string? localizedSectionName = null,
            bool sectionWritten = false)
        {
            if (headers is null)
            {
                return sectionWritten;
            }

            foreach (var headerValue in headers.Where(header =>
                _logAllowedHeaders.Contains(header.Key)))
            {
                if (!string.IsNullOrWhiteSpace(localizedSectionName) &&
                    !sectionWritten)
                {
                    detailsBuilder.AppendLine(
                        _localizer[localizedSectionName]);
                    sectionWritten = true;
                }

                foreach (var value in headerValue.Value)
                {
                    detailsBuilder.AppendLine($"{headerValue.Key}: {value}");
                }
            }

            return sectionWritten;
        }

        private async Task AppendHttpContentAsync(
            StringBuilder detailsBuilder,
            HttpContent? content,
            string localizedSectionName,
            CancellationToken cancellation)
        {
            if (content is null)
            {
                return;
            }

            var logBinaryContent = _configReader.Get().Network.BinaryContentLoggingEnabled;

            detailsBuilder.AppendLine();
            detailsBuilder.AppendLine(_localizer[localizedSectionName]);

            async Task WriteContentAsync(HttpContent contentToWrite)
            {
                if (logBinaryContent || contentToWrite.LogsAsTextContent())
                {
                    if (_traceRedactor.IsSensitiveMultipartContent(contentToWrite.Headers.ContentDisposition?.Name))
                    {
                        detailsBuilder.AppendLine(SENSITIVE_DATA_PLACEHOLDER);
                    }
                    else
                    {
                        if(contentToWrite.Headers.ContentLength > int.MaxValue)
                        {
                            detailsBuilder.AppendLine(_localizer[SharedResourceKeys.NetworkTraceTooLargeDetails]);
                        }
                        else
                        {
                            var text = await contentToWrite.ReadAsEncodedStringAsync(cancellation)
                            .ConfigureAwait(false);

                            detailsBuilder.AppendLine(_traceRedactor.ReplaceSensitiveData(text));
                        }
                    }
                }
                else
                {
                    detailsBuilder.AppendLine(_localizer[SharedResourceKeys.NetworkTraceNotDisplayedDetails]);
                }                
            }

            if (content is MultipartFormDataContent multipartContent)
            {
                foreach (var item in multipartContent)
                {
                    AppendHttpHeader(detailsBuilder, item.Headers);

                    await WriteContentAsync(item).ConfigureAwait(false);

                    detailsBuilder.AppendLine();
                }
            }
            else
            {
                await WriteContentAsync(content).ConfigureAwait(false);
            }
        }
    }
}
