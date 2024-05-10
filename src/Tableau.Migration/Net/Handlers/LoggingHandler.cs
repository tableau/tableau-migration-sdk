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
using System.Threading;
using System.Threading.Tasks;

namespace Tableau.Migration.Net.Handlers
{
    internal class LoggingHandler : DelegatingHandler
    {
        private readonly INetworkTraceLogger _traceLogger;

        public LoggingHandler(INetworkTraceLogger traceLogger)
        {
            _traceLogger = traceLogger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                var response = await base
                    .SendAsync(request, cancellationToken)
                    .ConfigureAwait(false);

                await _traceLogger
                    .WriteNetworkLogsAsync(
                        request,
                        response,
                        cancellationToken)
                    .ConfigureAwait(false);

                return response;
            }
            catch (Exception ex)
            {
                await _traceLogger
                    .WriteNetworkExceptionLogsAsync(
                        request,
                        ex,
                        cancellationToken)
                    .ConfigureAwait(false);

                throw;
            }
        }
    }
}
