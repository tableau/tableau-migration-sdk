﻿//
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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Resources;
using static Tableau.Migration.Net.NetworkTraceRedactor;

namespace Tableau.Migration.Net
{
    internal class NetworkTraceLogger : INetworkTraceLogger
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
                "User-Agent",
                Constants.REQUEST_CORRELATION_ID_HEADER
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

        public async Task WriteNetworkLogsAsync(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancel)
        {
            var detailsBuilder = new StringBuilder();

            AddHttpHeaderDetails(detailsBuilder, request, response);

            await AddHttpContentDetailsAsync(detailsBuilder, request, response, cancel).ConfigureAwait(false);

            var correlationId = response.Headers.GetCorrelationId();

            _logger.LogInformation(
                _localizer[SharedResourceKeys.NetworkTraceLogMessage],
                request.Method,
                request.RequestUri,
                response.StatusCode,
                correlationId,
                detailsBuilder.ToString());
        }

        public async Task WriteNetworkExceptionLogsAsync(HttpRequestMessage request, Exception exception, CancellationToken cancel)
        {
            var detailsBuilder = new StringBuilder();

            AddHttpHeaderDetails(detailsBuilder, request);

            await AddHttpContentDetailsAsync(detailsBuilder, request, null, cancel).ConfigureAwait(false);

            AddHttpExceptionDetails(detailsBuilder, exception);

            var correlationId = request.Headers.GetCorrelationId();

            _logger.LogError(
                _localizer[SharedResourceKeys.NetworkTraceExceptionLogMessage],
                request.Method,
                request.RequestUri,
                exception.Message,
                correlationId,
                detailsBuilder.ToString());
        }

        private void AddHttpHeaderDetails(StringBuilder detailsBuilder, HttpRequestMessage request, HttpResponseMessage? response = null)
        {
            var headersLogging = _configReader.Get().Network.HeadersLoggingEnabled;

            if (!headersLogging)
            {
                return;
            }

            var requestSectionWritten = AppendHttpHeader(detailsBuilder, request.Headers, SharedResourceKeys.SectionRequestHeaders);

            AppendHttpHeader(detailsBuilder, request.Content?.Headers, SharedResourceKeys.SectionRequestHeaders, requestSectionWritten);

            var responseSectionWritten = AppendHttpHeader(detailsBuilder, response?.Headers, SharedResourceKeys.SectionResponseHeaders);
            AppendHttpHeader(detailsBuilder, response?.Content?.Headers, SharedResourceKeys.SectionResponseHeaders, responseSectionWritten);
        }

        private async Task AddHttpContentDetailsAsync(StringBuilder detailsBuilder, HttpRequestMessage request, HttpResponseMessage? response, CancellationToken cancellation)
        {
            var contentLogging = _configReader.Get().Network.ContentLoggingEnabled;
            var workbookContentLogging = _configReader.Get().Network.WorkbookContentLoggingEnabled;

            if (!contentLogging)
            {
                return;
            }

            // Don't log the content of the request if it is a workbook download request and it's disabled.
            if (IsWorkbookDownloadRequest(request) && !workbookContentLogging)
            {
                return;
            }

            await AppendHttpContentAsync(detailsBuilder, request.Content, SharedResourceKeys.SectionRequestContent, cancellation)
                .ConfigureAwait(false);
            await AppendHttpContentAsync(detailsBuilder, response?.Content, SharedResourceKeys.SectionResponseContent, cancellation)
                .ConfigureAwait(false);
        }

        private void AddHttpExceptionDetails(StringBuilder detailsBuilder, Exception exception)
        {
            var exceptionLogging = _configReader.Get().Network.ExceptionsLoggingEnabled;

            if (!exceptionLogging)
            {
                return;
            }

            detailsBuilder.AppendLine(_localizer[SharedResourceKeys.SectionException]);

            detailsBuilder.AppendLine(exception.StackTrace);
        }

        private bool AppendHttpHeader(StringBuilder detailsBuilder, HttpHeaders? headers, string? localizedSectionName = null, bool sectionWritten = false)
        {
            if (headers is null)
            {
                return sectionWritten;
            }

            foreach (var headerValue in headers.Where(header => _logAllowedHeaders.Contains(header.Key)))
            {
                if (!string.IsNullOrWhiteSpace(localizedSectionName) && !sectionWritten)
                {
                    detailsBuilder.AppendLine(_localizer[localizedSectionName]);
                    sectionWritten = true;
                }

                foreach (var value in headerValue.Value)
                {
                    detailsBuilder.AppendLine($"{headerValue.Key}: {value}");
                }
            }

            return sectionWritten;
        }

        private async Task AppendHttpContentAsync(StringBuilder detailsBuilder, HttpContent? content, string localizedSectionName, CancellationToken cancellation)
        {
            if (content is null)
            {
                return;
            }

            var logBinaryContent = _configReader.Get().Network.BinaryContentLoggingEnabled;

            detailsBuilder.AppendLine();
            detailsBuilder.AppendLine(_localizer[localizedSectionName]);

            if (content is not MultipartFormDataContent multipartContent)
            {
                await WriteContentAsync(content).ConfigureAwait(false);
                return;
            }

            foreach (var item in multipartContent)
            {
                AppendHttpHeader(detailsBuilder, item.Headers);

                await WriteContentAsync(item).ConfigureAwait(false);

                detailsBuilder.AppendLine();
            }

            async Task WriteContentAsync(HttpContent contentToWrite)
            {
                // If the content is binary (i.e. it is not text) and we are not logging binary content, then we should not log the content.
                if (!logBinaryContent && !contentToWrite.LogsAsTextContent())
                {
                    detailsBuilder.AppendLine(_localizer[SharedResourceKeys.NetworkTraceNotDisplayedDetails]);
                    return;
                }

                if (_traceRedactor.IsSensitiveMultipartContent(contentToWrite.Headers.ContentDisposition?.Name))
                {
                    detailsBuilder.AppendLine(SENSITIVE_DATA_PLACEHOLDER);
                    return;
                }

                if (contentToWrite.Headers.ContentLength > int.MaxValue)
                {
                    detailsBuilder.AppendLine(_localizer[SharedResourceKeys.NetworkTraceTooLargeDetails]);
                    return;
                }

                var text = await contentToWrite.ReadAsEncodedStringAsync(cancellation).ConfigureAwait(false);

                detailsBuilder.AppendLine(_traceRedactor.ReplaceSensitiveData(text));

                return;

            }
        }

        private bool IsWorkbookDownloadRequest(HttpRequestMessage request)
        {
            if (request?.RequestUri == null)
            {
                return false;
            }

            var segments = request.RequestUri.Segments.Select(segment => segment.Trim('/')).ToArray();

            for (int i = 0; i < segments.Length; i++)
            {
                // Check if this is a workbooks RestAPI
                if (string.Equals(segments[i], RestUrlKeywords.Workbooks, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if this is a workbook download request. "contents" will always show up 2 after the "workbooks" in the URI.
                    if (i + 2 < segments.Length && string.Equals(segments[i + 2], RestUrlKeywords.Content, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}