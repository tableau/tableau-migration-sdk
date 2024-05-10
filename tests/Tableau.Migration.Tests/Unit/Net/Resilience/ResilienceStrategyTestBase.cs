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
using Moq;
using Polly;
using Polly.Testing;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Resilience;

namespace Tableau.Migration.Tests.Unit.Net.Resilience
{
    public abstract class ResilienceStrategyTestBase : AutoFixtureTestBase
    {
        protected ResiliencePipelineBuilder<HttpResponseMessage> PipelineBuilder { get; }

        protected MigrationSdkOptions Options { get; }

        protected DateTimeOffset UtcNow { get; set; } = DateTimeOffset.UtcNow - TimeSpan.FromMinutes(5);

        protected ResilienceStrategyTestBase()
        {
            PipelineBuilder = new();
            Options = new();

            var mockTimeProvider = Freeze<Mock<TimeProvider>>();
            mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(() => UtcNow);
        }

        protected abstract IResilienceStrategyBuilder GetBuilder();

        protected (ResiliencePipelineDescriptor, Action?) Build()
        {
            var builder = GetBuilder();

            Action? onPipelineDisposed = null;
            builder.Build(PipelineBuilder, Options, ref onPipelineDisposed);

            return (PipelineBuilder.Build().GetPipelineDescriptor(), onPipelineDisposed);
        }
    }
}
