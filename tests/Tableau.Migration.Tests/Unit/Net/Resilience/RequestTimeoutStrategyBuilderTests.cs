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
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using Tableau.Migration.Net.Resilience;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Resilience
{
    public class RequestTimeoutStrategyBuilderTests
    {
        public class Build : ResilienceStrategyTestBase
        {
            protected override IResilienceStrategyBuilder GetBuilder()
                => Create<RequestTimeoutStrategyBuilder>();

            private async ValueTask<TimeSpan> GetTimeoutAsync(TimeoutStrategyOptions strategyOptions, HttpMethod method, string? url)
            {
                Assert.NotNull(strategyOptions.TimeoutGenerator);

                var ctx = ResilienceContextPool.Shared.Get();
                try
                {
                    var request = new HttpRequestMessage(method, url);
                    ctx.SetRequest(request);

                    var args = new TimeoutGeneratorArguments(ctx);

                    return await strategyOptions.TimeoutGenerator(args);
                }
                finally
                {
                    ResilienceContextPool.Shared.Return(ctx);
                }
            }

            private async ValueTask AssertTimeoutsAsync(TimeoutStrategyOptions strategyOptions)
            {
                var timeout = Options.Network.Resilience.PerRequestTimeout;
                var fileTimeout = Options.Network.Resilience.PerFileTransferRequestTimeout;

                // Fall back to default timeout when no method/URL match.
                Assert.Equal(timeout, await GetTimeoutAsync(strategyOptions, HttpMethod.Get, null));
                Assert.Equal(timeout, await GetTimeoutAsync(strategyOptions, HttpMethod.Get, "http://localhost/datasources"));
                Assert.Equal(timeout, await GetTimeoutAsync(strategyOptions, HttpMethod.Post, "http://localhost/datasources/id/content"));

                Assert.Equal(fileTimeout, await GetTimeoutAsync(strategyOptions, HttpMethod.Get, "http://localhost/api/datasources/id/content"));
                Assert.Equal(fileTimeout, await GetTimeoutAsync(strategyOptions, HttpMethod.Get, "http://localhost/api/workbooks/id/content"));
                Assert.Equal(fileTimeout, await GetTimeoutAsync(strategyOptions, HttpMethod.Put, "http://localhost/api/fileUploads/session"));
            }

            [Fact]
            public async Task TimeoutGeneratorConfiguredAsync()
            {
                // Arrange

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var options = Assert.IsType<TimeoutStrategyOptions>(strategy.Options);

                await AssertTimeoutsAsync(options);

                Assert.Null(onDispose);
            }
        }
    }
}
