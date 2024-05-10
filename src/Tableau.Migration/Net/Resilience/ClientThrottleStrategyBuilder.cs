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
using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimiting;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Resilience
{
    internal sealed class ClientThrottleStrategyBuilder
        : IResilienceStrategyBuilder
    {
        /// <inheritdoc />
        public void Build(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, MigrationSdkOptions options, ref Action? onPipelineDisposed)
        {
            var resilienceOptions = options.Network.Resilience;

            if (resilienceOptions.ClientThrottleEnabled)
            {
                // Dynamically build the limiter so it is based on current configuration.
                var limiter = PartitionedRateLimiter.Create<HttpMethod, HttpMethod>(method =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(method, m =>
                    {
                        if(m == HttpMethod.Get)
                        {
                            return new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = resilienceOptions.MaxReadRequests,
                                Window = resilienceOptions.MaxReadRequestsInterval
                            };
                        }

                        return new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = resilienceOptions.MaxPublishRequests,
                            Window = resilienceOptions.MaxPublishRequestsInterval
                        };
                    });
                });

                pipelineBuilder.AddRateLimiter(new RateLimiterStrategyOptions
                {
                    RateLimiter = args => limiter.AcquireAsync(args.Context.GetRequest().Method, cancellationToken: args.Context.CancellationToken)
                });

                // Ensure the dynamic limiter is disposed when configuration changes.
                onPipelineDisposed = () => limiter.Dispose();
            }
        }
    }
}
