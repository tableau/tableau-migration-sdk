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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Tableau.Migration.Config;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Net.Resilience
{
    internal sealed class ServerThrottleStrategyBuilder
        : IResilienceStrategyBuilder
    {
        private readonly TimeProvider _timeProvider;
        private readonly ISharedResourcesLocalizer _localizer;
        private readonly ILogger<ServerThrottleStrategyBuilder> _logger;

        internal static readonly TimeSpan DEFAULT_RETRY_INTERVAL_FALLBACK = TimeSpan.FromMinutes(1);

        public ServerThrottleStrategyBuilder(
            TimeProvider timeProvider,
            ISharedResourcesLocalizer localizer,
            ILogger<ServerThrottleStrategyBuilder> logger)
        {
            _timeProvider = timeProvider;
            _localizer = localizer;
            _logger = logger;
        }

        private TimeSpan DelayGenerator(RetryDelayGeneratorArguments<HttpResponseMessage> args, ResilienceOptions resilienceOptions)
        {
            TimeSpan delay;

            //Obey the server Retry-After header value.
            if (args.Outcome.Result is not null)
            {
                var retryAfter = args.Outcome.Result.Headers.RetryAfter;
                if (retryAfter is not null)
                {
                    if (retryAfter.Delta is not null)
                    {
                        delay = retryAfter.Delta.Value;
                        _logger.LogWarning(
                            _localizer[SharedResourceKeys.ServerThrottleRetryAfterDeltaLogMessage],
                            delay.TotalMinutes.ToString("F2"),
                            delay.TotalSeconds);
                        return delay;
                    }
                    else if (retryAfter.Date is not null)
                    {
                        delay = retryAfter.Date.Value - _timeProvider.GetUtcNow();
                        _logger.LogWarning(
                            _localizer[SharedResourceKeys.ServerThrottleRetryAfterDateLogMessage],
                            delay.TotalMinutes.ToString("F2"),
                            retryAfter.Date.Value);
                        return delay;
                    }
                }
            }

            //If no Retry-After header use our configured retry intervals.
            //Falling back to an internal default if there are no configured intervals.
            if (!resilienceOptions.ServerThrottleRetryIntervals.Any())
            {
                delay = DEFAULT_RETRY_INTERVAL_FALLBACK;
            }
            else if (args.AttemptNumber >= resilienceOptions.ServerThrottleRetryIntervals.Length)
            {
                delay = resilienceOptions.ServerThrottleRetryIntervals[^1];
            }
            else
            {
                delay = resilienceOptions.ServerThrottleRetryIntervals[args.AttemptNumber];
            }

            _logger.LogWarning(
                _localizer[SharedResourceKeys.ServerThrottleRetryIntervalLogMessage],
                delay.TotalMinutes.ToString("F2"),
                args.AttemptNumber + 1,
                delay.TotalSeconds);
            return delay;
        }

        /// <inheritdoc />
        public void Build(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, MigrationSdkOptions options, ref Action? onPipelineDisposed)
        {
            var resilienceOptions = options.Network.Resilience;

            if (!resilienceOptions.ServerThrottleEnabled)
            {
                return;
            }

            pipelineBuilder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = static args => ValueTask.FromResult(args.Outcome.Result?.StatusCode is HttpStatusCode.TooManyRequests),
                MaxRetryAttempts = resilienceOptions.ServerThrottleLimitRetries && resilienceOptions.ServerThrottleRetryIntervals.Any() ?
                    resilienceOptions.ServerThrottleRetryIntervals.Length : int.MaxValue,
                DelayGenerator = args => ValueTask.FromResult<TimeSpan?>(DelayGenerator(args, resilienceOptions))
            });
        }
    }
}
