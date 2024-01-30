// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Net.Http;
using Moq;
using Polly.Bulkhead;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies
{
    public class MaxConcurrencyPolicyBuilderTests
    {
        private readonly Mock<IConfigReader> _mockedConfigReader;
        private readonly MigrationSdkOptions _sdkOptions;
        private readonly MaxConcurrencyPolicyBuilder _builder;

        public MaxConcurrencyPolicyBuilderTests()
        {
            _mockedConfigReader = new Mock<IConfigReader>();
            _sdkOptions = new MigrationSdkOptions();
            _mockedConfigReader
                .Setup(x => x.Get())
                .Returns(_sdkOptions);
            _builder = new MaxConcurrencyPolicyBuilder(
                _mockedConfigReader.Object);
        }

        [Fact]
        public void BuildPolicy_ReturnsDefaultPolicy()
        {
            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.Null(policy);
        }

        [Fact]
        public void BuildPolicy_EnableRequestsLimit_ReturnsDefaultPolicy()
        {
            // Arrange
            _sdkOptions.Network.Resilience.ConcurrentRequestsLimitEnabled = true;

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncBulkheadPolicy<HttpResponseMessage>>(policy);
            var bulkheadPolicy = (AsyncBulkheadPolicy<HttpResponseMessage>)policy;
            Assert.Equal(_sdkOptions.Network.Resilience.MaxConcurrentRequests, bulkheadPolicy.BulkheadAvailableCount);
            Assert.Equal(_sdkOptions.Network.Resilience.ConcurrentWaitingRequestsOnQueue, bulkheadPolicy.QueueAvailableCount);
        }

        [Fact]
        public void BuildPolicy_CustomRequestsLimit_ReturnsPolicy()
        {
            // Arrange
            _sdkOptions.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
            _sdkOptions.Network.Resilience.MaxConcurrentRequests = 2;
            _sdkOptions.Network.Resilience.ConcurrentWaitingRequestsOnQueue = 3;

            // Act
            var policy = _builder.Build(new HttpRequestMessage());

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncBulkheadPolicy<HttpResponseMessage>>(policy);
            var bulkheadPolicy = (AsyncBulkheadPolicy<HttpResponseMessage>)policy;
            Assert.Equal(_sdkOptions.Network.Resilience.MaxConcurrentRequests, bulkheadPolicy.BulkheadAvailableCount);
            Assert.Equal(_sdkOptions.Network.Resilience.ConcurrentWaitingRequestsOnQueue, bulkheadPolicy.QueueAvailableCount);
        }
    }
}
