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

using System;
using System.Net.Http;
using Moq;
using Polly.RateLimit;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies;

public class ClientThrottlePolicyBuilderTests
{
    private readonly Mock<IConfigReader> _mockedConfigReader;
    private readonly MigrationSdkOptions _sdkOptions;
    private readonly ClientThrottlePolicyBuilder _builder;

    public ClientThrottlePolicyBuilderTests()
    {
        _mockedConfigReader = new Mock<IConfigReader>();
        _sdkOptions = new MigrationSdkOptions();
        _mockedConfigReader
            .Setup(x => x.Get())
            .Returns(_sdkOptions);
        _builder = new ClientThrottlePolicyBuilder(
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
    public void BuildPolicy_EnableClientThrottling_ReturnsDefaultPolicyForRead()
    {
        // Arrange
        _sdkOptions.Network.Resilience.ClientThrottleEnabled = true;

        // Act
        var policy = _builder.Build(new HttpRequestMessage());

        // Assert
        Assert.NotNull(policy);
        Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(policy);
        ClientThrottlePolicyBuilderTests.AssertLimiterProperties(
            (AsyncRateLimitPolicy<HttpResponseMessage>)policy,
            _sdkOptions.Network.Resilience.MaxReadRequests,
            _sdkOptions.Network.Resilience.MaxReadRequestsInterval,
            _sdkOptions.Network.Resilience.MaxBurstReadRequests);
    }

    [Fact]
    public void BuildPolicy_CustomReadConfiguration_ReturnsPolicyForRead()
    {
        // Arrange
        _sdkOptions.Network.Resilience.ClientThrottleEnabled = true;
        _sdkOptions.Network.Resilience.MaxReadRequests = 100;
        _sdkOptions.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromSeconds(1);
        _sdkOptions.Network.Resilience.MaxBurstReadRequests = 1;

        // Act
        var policy = _builder.Build(new HttpRequestMessage());

        // Assert
        Assert.NotNull(policy);
        Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(policy);
        ClientThrottlePolicyBuilderTests.AssertLimiterProperties(
            (AsyncRateLimitPolicy<HttpResponseMessage>)policy,
            _sdkOptions.Network.Resilience.MaxReadRequests,
            _sdkOptions.Network.Resilience.MaxReadRequestsInterval,
            _sdkOptions.Network.Resilience.MaxBurstReadRequests);
    }

    [Fact]
    public void BuildPolicy_EnableClientThrottling_ReturnsDefaultPolicyForPublish()
    {
        // Arrange
        _sdkOptions.Network.Resilience.ClientThrottleEnabled = true;

        // Act
        var policy = _builder.Build(
            new HttpRequestMessage(
                HttpMethod.Put,
                (Uri?)null));

        // Assert
        Assert.NotNull(policy);
        Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(policy);
        ClientThrottlePolicyBuilderTests.AssertLimiterProperties(
            (AsyncRateLimitPolicy<HttpResponseMessage>)policy,
            _sdkOptions.Network.Resilience.MaxPublishRequests,
            _sdkOptions.Network.Resilience.MaxPublishRequestsInterval,
            _sdkOptions.Network.Resilience.MaxBurstPublishRequests);
    }

    [Fact]
    public void BuildPolicy_CustomPublishConfiguration_ReturnsPolicyForPublish()
    {
        // Arrange
        _sdkOptions.Network.Resilience.ClientThrottleEnabled = true;
        _sdkOptions.Network.Resilience.MaxPublishRequests = 60;
        _sdkOptions.Network.Resilience.MaxPublishRequestsInterval = TimeSpan.FromMinutes(2);
        _sdkOptions.Network.Resilience.MaxBurstPublishRequests = 10;

        // Act
        var policy = _builder.Build(
            new HttpRequestMessage(
                HttpMethod.Put,
                (Uri?)null));

        // Assert
        Assert.NotNull(policy);
        Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(policy);
        ClientThrottlePolicyBuilderTests.AssertLimiterProperties(
            (AsyncRateLimitPolicy<HttpResponseMessage>)policy,
            _sdkOptions.Network.Resilience.MaxPublishRequests,
            _sdkOptions.Network.Resilience.MaxPublishRequestsInterval,
            _sdkOptions.Network.Resilience.MaxBurstPublishRequests);
    }

    private static void AssertLimiterProperties(
        AsyncRateLimitPolicy<HttpResponseMessage> policy,
        long rateLimit,
        TimeSpan rateLimitInterval,
        long burstRateLimit)
    {
        var limiter = policy.GetFieldValue("_rateLimiter");

        Assert.NotNull(limiter);

        var addTokenTickIntervalObject = limiter.GetFieldValue("addTokenTickInterval");
        var bucketCapacityObject = limiter.GetFieldValue("bucketCapacity");
        var currentTokensObject = limiter.GetFieldValue("currentTokens");

        Assert.NotNull(addTokenTickIntervalObject);
        Assert.NotNull(bucketCapacityObject);
        Assert.NotNull(currentTokensObject);
        Assert.IsType<long>(addTokenTickIntervalObject);
        Assert.IsType<long>(bucketCapacityObject);
        Assert.IsType<long>(currentTokensObject);

        Assert.Equal(burstRateLimit, (long)bucketCapacityObject);
        Assert.Equal(rateLimitInterval.Ticks / rateLimit, (long)addTokenTickIntervalObject);
        Assert.Equal(burstRateLimit, (long)currentTokensObject);
    }
}
