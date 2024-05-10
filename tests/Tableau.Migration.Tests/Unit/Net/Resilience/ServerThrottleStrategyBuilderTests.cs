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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Tableau.Migration.Net.Resilience;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Resilience
{
    public class ServerThrottleStrategyBuilderTests
    {
        public class Build : ResilienceStrategyTestBase
        {
            protected override IResilienceStrategyBuilder GetBuilder()
                => Create<ServerThrottleStrategyBuilder>();

            private async ValueTask<bool> GetHandleResponseAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions, HttpStatusCode statusCode)
            {
                var ctx = ResilienceContextPool.Shared.Get();
                try
                {
                    var outcome = Outcome.FromResult(new HttpResponseMessage(statusCode));
                    var args = new RetryPredicateArguments<HttpResponseMessage>(ctx, outcome, 1);

                    return await strategyOptions.ShouldHandle(args);
                }
                finally
                {
                    ResilienceContextPool.Shared.Return(ctx);
                }
            }

            private async ValueTask AssertHandleResponseAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions)
            {
                Assert.True(await GetHandleResponseAsync(strategyOptions, HttpStatusCode.TooManyRequests));
                Assert.False(await GetHandleResponseAsync(strategyOptions, HttpStatusCode.OK));
            }

            private async ValueTask<TimeSpan?> GetDelayAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions, 
                RetryConditionHeaderValue? retryHeader = null, int attemptNumber = 1)
            {
                Assert.NotNull(strategyOptions.DelayGenerator);

                var ctx = ResilienceContextPool.Shared.Get();
                try
                {
                    var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
                    if(retryHeader is not null)
                    {
                        response.Headers.RetryAfter = retryHeader;
                    }

                    var outcome = Outcome.FromResult(response);
                    var args = new RetryDelayGeneratorArguments<HttpResponseMessage>(ctx, outcome, attemptNumber);

                    return await strategyOptions.DelayGenerator(args);
                }
                finally
                {
                    ResilienceContextPool.Shared.Return(ctx);
                }
            }

            private async ValueTask AssertDelayGeneratorAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions)
            {
                var delta = TimeSpan.FromMinutes(47);
                Assert.Equal(delta, await GetDelayAsync(strategyOptions, new(delta)));

                var targetDate = DateTimeOffset.UtcNow.AddMinutes(12);
                Assert.Equal(targetDate - UtcNow, await GetDelayAsync(strategyOptions, new(targetDate)));

                var resilienceOptions = Options.Network.Resilience;
                if(!resilienceOptions.ServerThrottleRetryIntervals.Any())
                {
                    Assert.Equal(ServerThrottleStrategyBuilder.DEFAULT_RETRY_INTERVAL_FALLBACK, await GetDelayAsync(strategyOptions));
                }
                else
                {
                    for(var i = 0; i < resilienceOptions.ServerThrottleRetryIntervals.Length; i++)
                    {
                        var interval = resilienceOptions.ServerThrottleRetryIntervals[i];
                        Assert.Equal(interval, await GetDelayAsync(strategyOptions, attemptNumber: i));
                    }

                    Assert.Equal(resilienceOptions.ServerThrottleRetryIntervals[^1], await GetDelayAsync(strategyOptions, attemptNumber: resilienceOptions.ServerThrottleRetryIntervals.Length));
                }
            }

            private async ValueTask AssertStrategyOptionsAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions)
            {
                await AssertHandleResponseAsync(strategyOptions);
                await AssertDelayGeneratorAsync(strategyOptions);
            }

            [Fact]
            public async Task StrategyEnabledWithDefaultsAsync()
            {
                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var options = Assert.IsType<RetryStrategyOptions<HttpResponseMessage>>(strategy.Options);

                await AssertStrategyOptionsAsync(options);

                Assert.Null(onDispose);
            }

            [Fact]
            public async Task NoConfiguredRetryIntervalsAsync()
            {
                // Arrange
                Options.Network.Resilience.ServerThrottleRetryIntervals = Array.Empty<TimeSpan>();

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var options = Assert.IsType<RetryStrategyOptions<HttpResponseMessage>>(strategy.Options);

                await AssertStrategyOptionsAsync(options);

                Assert.Equal(int.MaxValue, options.MaxRetryAttempts);

                Assert.Null(onDispose);
            }

            [Fact]
            public async Task RetryLimitDisabledAsync()
            {
                // Arrange
                Options.Network.Resilience.ServerThrottleLimitRetries = false;

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var options = Assert.IsType<RetryStrategyOptions<HttpResponseMessage>>(strategy.Options);

                await AssertStrategyOptionsAsync(options);

                Assert.Equal(int.MaxValue, options.MaxRetryAttempts);

                Assert.Null(onDispose);
            }

            [Fact]
            public void StrategyDisabled()
            {
                // Arrange
                Options.Network.Resilience.ServerThrottleEnabled = false;

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                Assert.Empty(pipeline.Strategies);

                Assert.Null(onDispose);
            }
        }
    }
}
