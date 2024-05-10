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
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Tableau.Migration.Config;

namespace Tableau.Migration.Net.Resilience
{
    /// <summary>
    /// Interface for an object that can build and add a resilience strategy to a resilience pipeline builder.
    /// </summary>
    public interface IResilienceStrategyBuilder
    {
        /// <summary>
        /// Adds a resiliance strategy to the pipeline builder.
        /// </summary>
        /// <param name="pipelineBuilder">The resilience pipeline builder to add strategies to.</param>
        /// <param name="options">The current SDK options.</param>
        /// <param name="onPipelineDisposed">
        /// An action to perform when the pipeline is disposed, or null.
        /// Supplied as an out parameter because <see cref="ResilienceHandlerContext"/> is not unit test-able.
        /// </param>
        void Build(ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder, MigrationSdkOptions options, ref Action? onPipelineDisposed);
    }
}
