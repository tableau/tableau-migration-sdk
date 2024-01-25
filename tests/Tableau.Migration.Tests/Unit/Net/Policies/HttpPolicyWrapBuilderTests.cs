using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly;
using Polly.Bulkhead;
using Polly.RateLimit;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies;

public class HttpPolicyWrapBuilderTests
{
    private readonly Mock<IConfigReader> _mockedConfigReader;
    private readonly MigrationSdkOptions _sdkOptions;
    private readonly IServiceCollection _services;

    #region - GetPolicyWrapCases -

    public static IEnumerable<object[]> GetPolicyWrapCases()
    {
        // GetRequestPolicies_ReturnsDefaultPolicy
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            }
        };

        // GetRequestPolicies_ReturnsDefaultPolicy
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ServerThrottleEnabled = false;
            }
        };

        // GetRequestPolicies_DisableRetry_ReturnsTimeout
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = false;
            }
        };

        // GetRequestPolicies_EmptyRetryIntervals_ReturnsTimeout
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryIntervals = Array.Empty<TimeSpan>();
            }
        };

        // GetRequestPolicies_EnableMaxConcurrency_ReturnsBulkheadPolicy
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncBulkheadPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = false;
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
            }
        };

        // GetRequestPolicies_EnableMaxConcurrencyAndRetry_ReturnsPoliciesWrapped
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncBulkheadPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = true;
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
            }
        };

        // GetRequestPolicies_EnableClientThrottle_ReturnsRateLimitPolicy
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = false;
                options.Network.Resilience.ClientThrottleEnabled = true;
            }
        };

        // GetRequestPolicies_EnableClientThrottle_ReturnsRateLimitPolicy
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = true;
                options.Network.Resilience.ClientThrottleEnabled = true;
            }
        };

        // GetRequestPolicies_EnableAll_ReturnsPoliciesWrapped
        yield return new object[]
        {
            // assertPolicy
            (IAsyncPolicy<HttpResponseMessage> policy) =>
            {
                var wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(policy);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncBulkheadPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRetryPolicy<HttpResponseMessage>>(wrap.Outer);

                wrap = Assert.IsType<AsyncPolicyWrap<HttpResponseMessage>>(wrap.Inner);

                Assert.IsType<AsyncRateLimitPolicy<HttpResponseMessage>>(wrap.Outer);
                Assert.IsType<AsyncTimeoutPolicy<HttpResponseMessage>>(wrap.Inner);
            },
            // setConfiguration
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = true;
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
                options.Network.Resilience.ClientThrottleEnabled = true;
            }
        };
    }

    #endregion

    public HttpPolicyWrapBuilderTests()
    {
        _mockedConfigReader = new Mock<IConfigReader>();
        _sdkOptions = new MigrationSdkOptions();
        _mockedConfigReader
            .Setup(x => x.Get())
            .Returns(_sdkOptions);

        _services = new ServiceCollection()
            .AddTableauMigrationSdk()
            .AddSingleton(_mockedConfigReader.Object);
    }

    [Fact]
    public void GetRequestPoliciesTwoTimes_DifferentObjects()
    {
        // Arrange
        var request = new HttpRequestMessage();
        using var serviceProvider = _services.BuildServiceProvider();
        var selector = serviceProvider.GetRequiredService<HttpPolicyWrapBuilder>();

        // Act
        var policy1 = selector.GetRequestPolicies(request);
        var policy2 = selector.GetRequestPolicies(request);

        // Assert
        Assert.NotNull(policy1);
        Assert.NotNull(policy2);
        Assert.NotSame(policy1, policy2);
    }

    [Theory]
    [MemberData(nameof(GetPolicyWrapCases))]
    public void GetRequestPoliciesCases(
        Action<IAsyncPolicy<HttpResponseMessage>> assertPolicy,
        Action<MigrationSdkOptions>? setConfiguration = null)
    {
        // Arrange
        setConfiguration?.Invoke(_sdkOptions);

        using var serviceProvider = _services.BuildServiceProvider();
        var selector = serviceProvider.GetRequiredService<HttpPolicyWrapBuilder>();

        // Act
        var policy = selector.GetRequestPolicies(new HttpRequestMessage());

        // Assert
        Assert.NotNull(policy);
        assertPolicy(policy);
    }
}
