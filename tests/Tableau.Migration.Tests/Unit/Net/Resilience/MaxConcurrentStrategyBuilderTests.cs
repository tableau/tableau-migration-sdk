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

using Polly.RateLimiting;
using Tableau.Migration.Net.Resilience;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Resilience
{
    public class MaxConcurrentStrategyBuilderTests
    {
        public class Build : ResilienceStrategyTestBase
        {
            protected override IResilienceStrategyBuilder GetBuilder()
                => Create<MaxConcurrencyStrategyBuilder>();

            [Fact]
            public void StrategyDisabledByDefault()
            {
                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                Assert.Empty(pipeline.Strategies);

                Assert.Null(onDispose);
            }

            [Fact]
            public void StrategyEnabled()
            {
                // Arrange
                var resilienceOptions = Options.Network.Resilience;
                resilienceOptions.ConcurrentRequestsLimitEnabled = true;

                // Act
                var (pipeline, onDispose) = Build();

                // Assert
                var strategy = Assert.Single(pipeline.Strategies);
                var options = Assert.IsType<RateLimiterStrategyOptions>(strategy.Options);

                Assert.Equal(resilienceOptions.MaxConcurrentRequests, options.DefaultRateLimiterOptions.PermitLimit);
                Assert.Equal(resilienceOptions.ConcurrentWaitingRequestsOnQueue, options.DefaultRateLimiterOptions.QueueLimit);

                Assert.Null(onDispose);
            }
        }
    }
}
