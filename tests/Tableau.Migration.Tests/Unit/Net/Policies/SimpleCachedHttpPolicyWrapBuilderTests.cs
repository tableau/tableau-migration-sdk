using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Polly;
using Tableau.Migration.Config;
using Tableau.Migration.Net.Policies;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Policies
{
    public class SimpleCachedHttpPolicyWrapBuilderTests
    {
        private readonly Mock<IConfigReader> _mockedConfigReader;
        private readonly MigrationSdkOptions _sdkOptions;
        private readonly IServiceCollection _services;

        private static readonly Action<MigrationSdkOptions>[] _setConfigurationList = new Action<MigrationSdkOptions>[]
        {
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = false;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryIntervals = Array.Empty<TimeSpan>();
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = false;
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = true;
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = false;
                options.Network.Resilience.ClientThrottleEnabled = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = true;
                options.Network.Resilience.ClientThrottleEnabled = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryEnabled = true;
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
                options.Network.Resilience.ClientThrottleEnabled = true;
            }
        };

        #region GetPolicyWrapCases

        public static IEnumerable<object[]> GetCachedPolicyWrapCases()
        {
            // Default Config - No Changes - Same Request - Cached Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(),
                // request1
                new HttpRequestMessage(),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.Same(policy1, policy2);
                }
            };

            // Default Config - No Changes - Different Requests - Different Policies
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(),
                // request1
                new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.NotSame(policy1, policy2);
                }
            };

            foreach (var setConfigAction in _setConfigurationList)
            {
                // Custom configuration - No Changes - Same Request - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request1
                    new HttpRequestMessage(),
                    // assertPolicy
                    (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                    {
                        Assert.Same(policy1, policy2);
                    },
                    // setInitialConfiguration
                    setConfigAction
                };
            }

            foreach (var setConfigAction in _setConfigurationList)
            {
                // Default configuration - With Changes - Same Request - New Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request1
                    new HttpRequestMessage(),
                    // assertPolicy
                    (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                    {
                        Assert.NotSame(policy1, policy2);
                    },
                    // setInitialConfiguration,
                    (MigrationSdkOptions options)=>{ },
                    // changeConfiguration
                    setConfigAction
                };
            }
        }

        #endregion

        public SimpleCachedHttpPolicyWrapBuilderTests()
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

        [Theory]
        [MemberData(nameof(GetCachedPolicyWrapCases))]
        public void GetCachedPolicyWrap(
            HttpRequestMessage request1,
            HttpRequestMessage request2,
            Action<IAsyncPolicy<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>> assertPolicies,
            Action<MigrationSdkOptions>? setInitialConfiguration = null,
            Action<MigrationSdkOptions>? changeConfiguration = null)
        {
            // Arrange
            setInitialConfiguration?.Invoke(_sdkOptions);
            var request = new HttpRequestMessage();
            using var serviceProvider = _services.BuildServiceProvider();
            var selector = serviceProvider.GetRequiredService<IHttpPolicyWrapBuilder>();

            // Act
            var policy1 = selector.GetRequestPolicies(request1);
            changeConfiguration?.Invoke(_sdkOptions);
            var policy2 = selector.GetRequestPolicies(request2);

            // Assert
            Assert.NotNull(policy1);
            Assert.NotNull(policy2);
            assertPolicies(policy1, policy2);
        }
    }
}