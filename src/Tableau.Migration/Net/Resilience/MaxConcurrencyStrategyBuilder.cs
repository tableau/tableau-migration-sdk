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
using Polly;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Resilience
{
    internal sealed class MaxConcurrencyStrategyBuilder
        : IResilienceStrategyBuilder
    {
        /// <inheritdoc />
        public void Build(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, MigrationSdkOptions options, ref Action? onPipelineDisposed)
        {
            if(options.Network.Resilience.ConcurrentRequestsLimitEnabled)
            {
                pipelineBuilder.AddConcurrencyLimiter(new()
                {
                    PermitLimit = options.Network.Resilience.MaxConcurrentRequests,
                    QueueLimit = options.Network.Resilience.ConcurrentWaitingRequestsOnQueue
                });
            }
        }
    }
}
