// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Policies
{
    internal sealed class ServerThrottlePolicyBuilder : IHttpPolicyBuilder
    {
        private static readonly TimeSpan DEFAULT_RETRY_INTERVAL_FALLBACK = TimeSpan.FromMinutes(1);

        private readonly IConfigReader _configReader;

        public ServerThrottlePolicyBuilder(IConfigReader configReader)
        {
            _configReader = configReader;
        }

        public IAsyncPolicy<HttpResponseMessage>? Build(HttpRequestMessage httpRequest)
        {
            var resilienceOptions = _configReader.Get().Network.Resilience;

            if (!resilienceOptions.ServerThrottleEnabled)
            {
                return null;
            }

            TimeSpan SleepDurationProvider(int i, DelegateResult<HttpResponseMessage> result, Context context)
            {
                //Obey the server Retry-After header value.
                var retryAfter = result.Result.Headers.RetryAfter;
                if (retryAfter is not null)
                {
                    if (retryAfter.Delta is not null)
                    {
                        return retryAfter.Delta.Value;
                    }
                    else if (retryAfter.Date is not null)
                    {
                        return retryAfter.Date.Value.Subtract(DateTime.UtcNow);
                    }
                }

                //If no Retry-After header use our configured retry intervals.
                //Falling back to an internal default if there are no configured intervals.
                if (resilienceOptions.ServerThrottleRetryIntervals.Length < 1)
                {
                    return DEFAULT_RETRY_INTERVAL_FALLBACK;
                }
                else if (i >= resilienceOptions.ServerThrottleRetryIntervals.Length)
                {
                    return resilienceOptions.ServerThrottleRetryIntervals.Last();
                }

                return resilienceOptions.ServerThrottleRetryIntervals[i];
            }

            var policy = Policy<HttpResponseMessage>
                .HandleResult(r => r.StatusCode is HttpStatusCode.TooManyRequests);

            AsyncRetryPolicy<HttpResponseMessage> result;
            if (resilienceOptions.ServerThrottleLimitRetries)
            {
                result = policy.WaitAndRetryAsync(resilienceOptions.ServerThrottleRetryIntervals.Length,
                    sleepDurationProvider: SleepDurationProvider,
                    onRetryAsync: async (_, _, _, _) => await Task.CompletedTask.ConfigureAwait(false));
            }
            else
            {
                result = policy.WaitAndRetryForeverAsync(
                    sleepDurationProvider: SleepDurationProvider,
                    onRetryAsync: async (_, _, _, _) => await Task.CompletedTask.ConfigureAwait(false)
                );
            }

            return result;
        }
    }
}
