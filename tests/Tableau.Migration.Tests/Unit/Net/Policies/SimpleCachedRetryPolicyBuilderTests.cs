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
    public class SimpleCachedRetryPolicyBuilderTests
    {
        private readonly Mock<IConfigReader> _mockedConfigReader;
        private readonly MigrationSdkOptions _sdkOptions;
        private readonly IServiceCollection _services;

        private static readonly Action<MigrationSdkOptions>[] _setOtherConfigurationList = new Action<MigrationSdkOptions>[]
        {
            // Default ConcurrentRequests Config
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
                options.Network.Resilience.MaxConcurrentRequests = 13;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
                options.Network.Resilience.ConcurrentWaitingRequestsOnQueue = 100;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
                options.Network.Resilience.MaxConcurrentRequests = 4;
                options.Network.Resilience.ConcurrentWaitingRequestsOnQueue = 50;
            },
            // Default ClientThrottle Config
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxReadRequests = 111;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(3);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxBurstReadRequests = 54;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxReadRequests = 111;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(3);
                options.Network.Resilience.MaxBurstReadRequests = 54;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxPublishRequests = 112;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxPublishRequestsInterval = TimeSpan.FromHours(4);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxBurstPublishRequests = 55;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxReadRequests = 112;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(4);
                options.Network.Resilience.MaxBurstReadRequests = 55;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxReadRequests = 111;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(3);
                options.Network.Resilience.MaxBurstReadRequests = 54;
                options.Network.Resilience.MaxReadRequests = 112;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(4);
                options.Network.Resilience.MaxBurstReadRequests = 55;
            },
            // Custom Timeout config
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerRequestTimeout = TimeSpan.FromSeconds(105);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ConcurrentRequestsLimitEnabled = true;
                options.Network.Resilience.MaxConcurrentRequests = 4;
                options.Network.Resilience.ConcurrentWaitingRequestsOnQueue = 50;
                options.Network.Resilience.ClientThrottleEnabled = true;
                options.Network.Resilience.MaxReadRequests = 111;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(3);
                options.Network.Resilience.MaxBurstReadRequests = 54;
                options.Network.Resilience.MaxReadRequests = 112;
                options.Network.Resilience.MaxReadRequestsInterval = TimeSpan.FromHours(4);
                options.Network.Resilience.MaxBurstReadRequests = 55;
                options.Network.Resilience.PerRequestTimeout = TimeSpan.FromSeconds(105);
            }
        };

        private static readonly Action<MigrationSdkOptions>[] _setRetryConfigurationList = new Action<MigrationSdkOptions>[]
        {
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryIntervals = new TimeSpan[]
                {
                    TimeSpan.FromMilliseconds(250)
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryIntervals = new TimeSpan[]
                {
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromMilliseconds(50),
                    TimeSpan.FromMilliseconds(50)
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryIntervals = new TimeSpan[]
                {
                    TimeSpan.FromMilliseconds(10),
                    TimeSpan.FromMilliseconds(20),
                    TimeSpan.FromMilliseconds(30),
                    TimeSpan.FromMilliseconds(40),
                    TimeSpan.FromMilliseconds(50)
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryOverrideResponseCodes = new int[]
                {
                    500
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryOverrideResponseCodes = new int[]
                {
                    500,
                    200,
                    208
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryOverrideResponseCodes = new int[]
                {
                    408
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.RetryIntervals = new TimeSpan[]
                {
                    TimeSpan.FromMilliseconds(10),
                    TimeSpan.FromMilliseconds(20),
                    TimeSpan.FromMilliseconds(30),
                    TimeSpan.FromMilliseconds(40),
                    TimeSpan.FromMilliseconds(50)
                };
                options.Network.Resilience.RetryOverrideResponseCodes = new int[]
                {
                    500,
                    200,
                    208
                };
            }
        };

        #region GetCachedPolicyCases

        public static IEnumerable<object[]> GetCachedPolicyCases()
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

            // Default Config - No Changes - Different Requests - Cached Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(),
                // request1
                new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.Same(policy1, policy2);
                }
            };

            foreach (var setConfigAction in _setOtherConfigurationList)
            {
                // Custom configuration - No Changes - Different Requests - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request1
                    new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
                    // assertPolicy
                    (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                    {
                        Assert.Same(policy1, policy2);
                    },
                    // setInitialConfiguration
                    setConfigAction
                };
            }

            foreach (var setConfigAction in _setOtherConfigurationList)
            {
                // Default configuration - With Changes - Different Requests - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request1
                    new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
                    // assertPolicy
                    (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                    {
                        Assert.Same(policy1, policy2);
                    },
                    // setInitialConfiguration,
                    (MigrationSdkOptions options)=>{ },
                    // changeConfiguration
                    setConfigAction
                };
            }

            foreach (var setConfigAction in _setRetryConfigurationList)
            {
                // Custom retry configuration - No Changes - Different Requests - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request1
                    new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
                    // assertPolicy
                    (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                    {
                        Assert.Same(policy1, policy2);
                    },
                    // setInitialConfiguration
                    setConfigAction
                };
            }

            foreach (var setConfigAction in _setRetryConfigurationList)
            {
                // Default configuration - With Retry Changes - Same Request - New Policy
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

        #endregion GetCachedPolicyCases

        public SimpleCachedRetryPolicyBuilderTests()
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
        [MemberData(nameof(GetCachedPolicyCases))]
        public void BuildCachedPolicy(
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
            var selector = serviceProvider.GetRequiredService<SimpleCachedRetryPolicyBuilder>();

            // Act
            var policy1 = selector.Build(request1);
            changeConfiguration?.Invoke(_sdkOptions);
            var policy2 = selector.Build(request2);

            // Assert
            Assert.NotNull(policy1);
            Assert.NotNull(policy2);
            assertPolicies(policy1, policy2);
        }
    }
}