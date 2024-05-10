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
using System.Threading.Tasks;
using Polly;
using Polly.RateLimiting;
using Polly.Retry;
using Polly.Timeout;
using Tableau.Migration.Net.Resilience;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Resilience
{
    public class RetryStrategyBuilderTests
    {
        public class Build : ResilienceStrategyTestBase
        {
            protected override IResilienceStrategyBuilder GetBuilder()
                => Create<RetryStrategyBuilder>();

            private async ValueTask<bool> GetPredicateResultAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions, HttpStatusCode? resultCode = null, Exception? ex = null)
            {
                var ctx = ResilienceContextPool.Shared.Get();
                try
                {
                    Outcome<HttpResponseMessage> outcome = ex is not null ?
                        Outcome.FromException<HttpResponseMessage>(ex) :
                        Outcome.FromResult(new HttpResponseMessage(resultCode ?? HttpStatusCode.OK));

                    var args = new RetryPredicateArguments<HttpResponseMessage>(ctx, outcome, 1);
                    return await strategyOptions.ShouldHandle(args);
                }
                finally
                {
                    ResilienceContextPool.Shared.Return(ctx);
                }
            }

            private async ValueTask AssertPredicateAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions)
            {
                Assert.True(await GetPredicateResultAsync(strategyOptions, ex: new HttpRequestException()));
                Assert.True(await GetPredicateResultAsync(strategyOptions, ex: new TimeoutRejectedException()));
                Assert.True(await GetPredicateResultAsync(strategyOptions, ex: new RateLimiterRejectedException()));
                
                //Non-transient exceptions
                Assert.False(await GetPredicateResultAsync(strategyOptions, ex: new Exception()));
                Assert.False(await GetPredicateResultAsync(strategyOptions, ex: new OperationCanceledException()));

                var resilienceOptions = Options.Network.Resilience;
                if(resilienceOptions.RetryOverrideResponseCodes.IsNullOrEmpty())
                {
                    foreach(var retryCode in resilienceOptions.RetryOverrideResponseCodes)
                    {
                        Assert.True(await GetPredicateResultAsync(strategyOptions, (HttpStatusCode)retryCode));
                    }
                }
                else
                {
                    Assert.True(await GetPredicateResultAsync(strategyOptions, HttpStatusCode.InternalServerError));
                    Assert.True(await GetPredicateResultAsync(strategyOptions, HttpStatusCode.ServiceUnavailable));
                    Assert.True(await GetPredicateResultAsync(strategyOptions, HttpStatusCode.RequestTimeout));

                    Assert.False(await GetPredicateResultAsync(strategyOptions, HttpStatusCode.NotFound));
                }
            }

            private async ValueTask<TimeSpan?> GetDelayAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions, int attemptNumber)
            {
                Assert.NotNull(strategyOptions.DelayGenerator);

                var ctx = ResilienceContextPool.Shared.Get();
                try
                {
                    var args = new RetryDelayGeneratorArguments<HttpResponseMessage>(ctx, Outcome.FromResult(new HttpResponseMessage()), attemptNumber);
                    return await strategyOptions.DelayGenerator(args);
                }
                finally
                {
                    ResilienceContextPool.Shared.Return(ctx);
                }
            }

            private async ValueTask AssertDelayGeneratorAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions)
            {
                var resilienceOptions = Options.Network.Resilience;
                if(!resilienceOptions.RetryIntervals.Any())
                {
                    return;
                }

                for(int i = 0; i < resilienceOptions.RetryIntervals.Length; i++)
                {
                    var interval = resilienceOptions.RetryIntervals[i];
                    Assert.Equal(interval, await GetDelayAsync(strategyOptions, i));
                }

                Assert.Equal(resilienceOptions.RetryIntervals[^1], await GetDelayAsync(strategyOptions, resilienceOptions.RetryIntervals.Length + 1));
            }

            private async ValueTask AssertStrategyOptionsAsync(RetryStrategyOptions<HttpResponseMessage> strategyOptions)
            {
                await AssertPredicateAsync(strategyOptions);
                await AssertDelayGeneratorAsync(strategyOptions);
            }

            [Fact]
            public void StrategyDisabled()
            {
                // Arrange
                Options.Network.Resilience.RetryEnabled = false;

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                Assert.Empty(pipeline.Strategies);

                Assert.Null(onDispose);
            }

            [Fact]
            public void NoRetryIntervals()
            {
                // Arrange
                Options.Network.Resilience.RetryIntervals = Array.Empty<TimeSpan>();

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                Assert.Empty(pipeline.Strategies);

                Assert.Null(onDispose);
            }

            [Fact]
            public async Task StrategyEnabledWithDefaultsAsync()
            {
                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var strategyOptions = Assert.IsType<RetryStrategyOptions<HttpResponseMessage>>(strategy.Options);

                await AssertStrategyOptionsAsync(strategyOptions);

                Assert.Null(onDispose);
            }

            [Fact]
            public async Task CustomIntervalsAsync()
            {
                // Arrange
                Options.Network.Resilience.RetryIntervals = new[] { TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1) };

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var strategyOptions = Assert.IsType<RetryStrategyOptions<HttpResponseMessage>>(strategy.Options);

                await AssertStrategyOptionsAsync(strategyOptions);

                Assert.Null(onDispose);
            }
        }
    }
}
