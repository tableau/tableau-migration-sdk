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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Config;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Net.Logging
{
    internal sealed class HttpActivityLogger : IHttpActivityLogger
    {
        private readonly ILogger<HttpActivityLogger> _logger;
        private readonly IConfigReader _configReader;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly IHttpContentRedactor _contentRedactor;

        public HttpActivityLogger(IHttpContentRedactor contentRedactor, IConfigReader configReader,
            ISharedResourcesLocalizer localizer, ILogger<HttpActivityLogger> logger)
        {
            _contentRedactor = contentRedactor;
            _configReader = configReader;
            _localizer = localizer;
            _logger = logger;
        }

        public void LogRequestStarted(HttpRequestMessage request)
        {
            var config = _configReader.Get().Network;
            if(!config.RequestsLoggingEnabled)
            {
                return;
            }

            var correlationId = request.Headers.GetCorrelationId();

            _logger.LogInformation(
                _localizer[SharedResourceKeys.HttpActivityRequestLogMessage],
                request.Method,
                request.RequestUri,
                correlationId);
        }

        public async Task LogResponseAsync(HttpRequestMessage request, HttpResponseMessage response, TimeSpan duration, CancellationToken cancel)
        {
            var config = _configReader.Get().Network;
            var detailsBuilder = new HttpActivityDetailsBuilder(config, _contentRedactor, _localizer);
                
            detailsBuilder.AddHttpHeaderDetails(request, response);
            await detailsBuilder.AddHttpContentDetailsAsync(request, response, cancel).ConfigureAwait(false);

            var correlationId = response.Headers.GetCorrelationId();

            _logger.Log(
                response.IsSuccessStatusCode ? LogLevel.Information : LogLevel.Warning,
                _localizer[SharedResourceKeys.HttpActivityLogMessage],
                request.Method,
                request.RequestUri,
                response.StatusCode,
                correlationId,
                duration.TotalMilliseconds,
                detailsBuilder.Build());
        }

        public async Task LogExceptionAsync(HttpRequestMessage request, Exception exception, TimeSpan duration, CancellationToken cancel)
        {
            var config = _configReader.Get().Network;
            var detailsBuilder = new HttpActivityDetailsBuilder(config, _contentRedactor, _localizer);
                
            detailsBuilder.AddHttpHeaderDetails(request);
            await detailsBuilder.AddHttpContentDetailsAsync(request, null, cancel).ConfigureAwait(false);
            detailsBuilder.AddHttpExceptionDetails(exception);

            var correlationId = request.Headers.GetCorrelationId();

            _logger.LogError(
                _localizer[SharedResourceKeys.HttpActivityExceptionLogMessage],
                request.Method,
                request.RequestUri,
                exception.Message,
                correlationId,
                duration.TotalMilliseconds,
                detailsBuilder.Build());
        }
    }
}