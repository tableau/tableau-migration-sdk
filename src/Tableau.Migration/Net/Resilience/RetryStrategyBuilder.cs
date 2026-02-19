//
//  Copyright (c) 2026, Salesforce, Inc.
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.RateLimiting;
using Polly.Retry;
using Polly.Timeout;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Resilience
{
    internal sealed class RetryStrategyBuilder
        : IResilienceStrategyBuilder
    {
        private static readonly ImmutableArray<Type> _standardRetryExceptions = new[]
        {
            typeof(HttpRequestException),
            typeof(TimeoutRejectedException),
            typeof(RateLimiterRejectedException)
        }.ToImmutableArray();

        public static bool IsStatusCodeRetriable(HttpStatusCode status, ResilienceOptions resilienceOptions)
        {
            return resilienceOptions.RetryOverrideResponseCodes.IsNullOrEmpty()
                ? (int)status >= 500 || status is HttpStatusCode.RequestTimeout
                : resilienceOptions.RetryOverrideResponseCodes.Contains((int)status);

        }
        private static bool ShouldRetry(RetryPredicateArguments<HttpResponseMessage> args, ResilienceOptions resilienceOptions)
        {
            if (args.Outcome.Exception is null)
            {
                var response = args.Outcome.Result;
                if (response is null)
                {
                    return false;
                }

                return IsStatusCodeRetriable(response.StatusCode, resilienceOptions);
            }

            return _standardRetryExceptions.Contains(args.Outcome.Exception.GetType());
        }

        /// <inheritdoc />
        public void Build(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, MigrationSdkOptions options, ref Action? onPipelineDisposed)
        {
            var resilienceOptions = options.Network.Resilience;

            if (!resilienceOptions.RetryEnabled || !resilienceOptions.RetryIntervals.Any())
            {
                return;
            }

            pipelineBuilder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = resilienceOptions.RetryIntervals.Length,
                ShouldHandle = args => ValueTask.FromResult(ShouldRetry(args, resilienceOptions)),
                DelayGenerator = args =>
                {
                    var interval = args.AttemptNumber < resilienceOptions.RetryIntervals.Length ?
                        resilienceOptions.RetryIntervals[args.AttemptNumber] : resilienceOptions.RetryIntervals[^1];

                    return ValueTask.FromResult<TimeSpan?>(interval);
                }
            });
        }
    }
}
