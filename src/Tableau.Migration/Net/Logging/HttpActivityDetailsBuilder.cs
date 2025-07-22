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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Config;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Net.Logging
{
    internal readonly struct HttpActivityDetailsBuilder
    {
        /// <summary>
        /// List of headers to log.
        /// </summary>
        private static readonly ImmutableHashSet<string> _logAllowedHeaders = ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase,
            [
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
            ]);

        private readonly NetworkOptions _config;
        private readonly IHttpContentRedactor _redactor;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly StringBuilder _details;

        public HttpActivityDetailsBuilder(NetworkOptions config, IHttpContentRedactor redactor, ISharedResourcesLocalizer localizer)
        {
            _config = config;
            _redactor = redactor;
            _localizer = localizer;
            _details = new();
        }

        public string Build() => _details.Length > 0 ? _details.ToString() : _localizer[SharedResourceKeys.HttpActivityEmptyText];

        public void AddHttpHeaderDetails(HttpRequestMessage request, HttpResponseMessage? response = null)
        {
            if (!_config.HeadersLoggingEnabled)
            {
                return;
            }

            var requestSectionWritten = AppendHttpHeader(request.Headers, SharedResourceKeys.HttpRequestHeadersSectionName);

            AppendHttpHeader(request.Content?.Headers, SharedResourceKeys.HttpRequestHeadersSectionName, requestSectionWritten);

            var responseSectionWritten = AppendHttpHeader(response?.Headers, SharedResourceKeys.HttpResponseHeadersSectionName);
            AppendHttpHeader(response?.Content?.Headers, SharedResourceKeys.HttpResponseHeadersSectionName, responseSectionWritten);
        }

        private bool AppendHttpHeader(HttpHeaders? headers, string? localizedSectionName = null, bool sectionWritten = false)
        {
            if (headers is null)
            {
                return sectionWritten;
            }

            foreach (var headerValue in headers.Where(header => _logAllowedHeaders.Contains(header.Key)))
            {
                if (!string.IsNullOrWhiteSpace(localizedSectionName) && !sectionWritten)
                {
                    _details.AppendLine(_localizer[localizedSectionName]);
                    sectionWritten = true;
                }

                foreach (var value in headerValue.Value)
                {
                    _details.AppendLine($"{headerValue.Key}: {value}");
                }
            }

            return sectionWritten;
        }

        public async Task AddHttpContentDetailsAsync(HttpRequestMessage request, HttpResponseMessage? response, CancellationToken cancel)
        {
            if (!_config.ContentLoggingEnabled)
            {
                return;
            }

            // Don't log the content of the request if it is a workbook download request and it's disabled.
            if (IsWorkbookDownloadRequest(request) && !_config.WorkbookContentLoggingEnabled)
            {
                return;
            }

            await AppendHttpContentAsync(request.Content, SharedResourceKeys.HttpRequestContentSectionName, cancel)
                .ConfigureAwait(false);
            await AppendHttpContentAsync(response?.Content, SharedResourceKeys.HttpResponseContentSectionName, cancel)
                .ConfigureAwait(false);
        }

        private async Task AppendHttpContentAsync(HttpContent? content, string localizedSectionName, CancellationToken cancel)
        {
            if (content is null)
            {
                return;
            }

            _details.AppendLine();
            _details.AppendLine(_localizer[localizedSectionName]);

            if (content is not MultipartFormDataContent multipartContent)
            {
                await WriteContentAsync(_config, _redactor, _localizer, _details, content, cancel).ConfigureAwait(false);
                return;
            }

            foreach (var item in multipartContent)
            {
                AppendHttpHeader(item.Headers);

                await WriteContentAsync(_config, _redactor, _localizer, _details, item, cancel).ConfigureAwait(false);

                _details.AppendLine();
            }

            static async Task WriteContentAsync(NetworkOptions config, IHttpContentRedactor redactor, ISharedResourcesLocalizer localizer, 
                StringBuilder details, HttpContent contentToWrite, CancellationToken cancel)
            {
                // If the content is binary (i.e. it is not text) and we are not logging binary content, then we should not log the content.
                if (!config.BinaryContentLoggingEnabled && !contentToWrite.LogsAsTextContent())
                {
                    details.AppendLine(localizer[SharedResourceKeys.HttpActivityNotDisplayedText]);
                    return;
                }

                if (redactor.IsSensitiveMultipartContent(contentToWrite.Headers.ContentDisposition?.Name))
                {
                    details.AppendLine(HttpContentRedactor.SENSITIVE_DATA_PLACEHOLDER);
                    return;
                }

                if (contentToWrite.Headers.ContentLength > int.MaxValue)
                {
                    details.AppendLine(localizer[SharedResourceKeys.HttpContentTooLargeText]);
                    return;
                }

                var text = await contentToWrite.ReadAsEncodedStringAsync(cancel).ConfigureAwait(false);

                details.AppendLine(redactor.ReplaceSensitiveData(text));

                return;
            }
        }

        private static bool IsWorkbookDownloadRequest(HttpRequestMessage request)
        {
            if (request?.RequestUri is null)
            {
                return false;
            }

            var segments = request.RequestUri.Segments.Select(segment => segment.Trim('/')).ToImmutableArray();

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

        public void AddHttpExceptionDetails(Exception exception)
        {
            if (!_config.ExceptionsLoggingEnabled)
            {
                return;
            }

            _details.AppendLine(_localizer[SharedResourceKeys.HttpExceptionSectionName]);
            _details.AppendLine(exception.StackTrace);
            _details.AppendLine(exception.InnerException?.Message);
        }
    }
}
