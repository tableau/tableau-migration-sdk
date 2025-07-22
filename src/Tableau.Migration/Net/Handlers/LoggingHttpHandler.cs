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
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Net.Logging;

namespace Tableau.Migration.Net.Handlers
{
    internal sealed class LoggingHttpHandler : DelegatingHandler
    {
        private readonly IHttpActivityLogger _activityLogger;

        public LoggingHttpHandler(IHttpActivityLogger activityLogger)
        {
            _activityLogger = activityLogger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancel)
        {
            var startTimestamp = Stopwatch.GetTimestamp();

            _activityLogger.LogRequestStarted(request);

            HttpResponseMessage response;
            try
            {
                response = await base.SendAsync(request, cancel).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await _activityLogger.LogExceptionAsync(request, ex, Stopwatch.GetElapsedTime(startTimestamp), cancel).ConfigureAwait(false);

                throw;
            }

            var duration = Stopwatch.GetElapsedTime(startTimestamp);
            await _activityLogger.LogResponseAsync(request, response, duration, cancel).ConfigureAwait(false);

            return response;
        }
    }
}
