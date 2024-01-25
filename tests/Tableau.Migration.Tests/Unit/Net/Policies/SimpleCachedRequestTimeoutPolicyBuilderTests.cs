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
    public class SimpleCachedRequestTimeoutPolicyBuilderTests
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

        private static readonly Action<MigrationSdkOptions>[] _setTimeoutConfigurationList = new Action<MigrationSdkOptions>[]
        {
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerRequestTimeout = TimeSpan.FromSeconds(105);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerRequestTimeout = TimeSpan.FromDays(1);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerFileTransferRequestTimeout = TimeSpan.FromHours(2);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerFileTransferRequestTimeout = TimeSpan.FromDays(7);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerRequestTimeout = TimeSpan.FromSeconds(105);
                options.Network.Resilience.PerFileTransferRequestTimeout = TimeSpan.FromHours(1);
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.PerRequestTimeout = TimeSpan.FromMinutes(1);
                options.Network.Resilience.PerFileTransferRequestTimeout = TimeSpan.FromMinutes(15);
            }
        };

        #region GetCachedPolicyCases

        public static IEnumerable<object[]> GetCachedPolicyCases()
        {
            // Case #1: Default Config - No Changes - Same Request - Cached Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(),
                // request2
                new HttpRequestMessage(),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.Same(policy1, policy2);
                }
            };

            // Case #2: Default Config - No Changes - Different Requests - Cached Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(),
                // request2
                new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.Same(policy1, policy2);
                }
            };

            // Case #3: Default Config - No Changes - Different Requests (one file transfer) - New Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(),
                // request2
                new HttpRequestMessage(HttpMethod.Put, new Uri($"https://localhost/api/3.21/sites/{Guid.NewGuid()}/fileUploads/{Guid.NewGuid()}")),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.NotSame(policy1, policy2);
                }
            };

            // Case #4: Default Config - No Changes - Different Requests (two file transfer) - Same Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")),
                // request2
                new HttpRequestMessage(HttpMethod.Put, new Uri($"https://localhost/api/3.21/sites/{Guid.NewGuid()}/fileUploads/{Guid.NewGuid()}")),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.Same(policy1, policy2);
                }
            };

            // Case #5: Default Config - No Changes - Same Request (file transfer) - Same Policy
            yield return new object[]
            {
                // request1
                new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")),
                // request2
                new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")),
                // assertPolicy
                (IAsyncPolicy<HttpResponseMessage> policy1, IAsyncPolicy<HttpResponseMessage> policy2) =>
                {
                    Assert.Same(policy1, policy2);
                }
            };

            // Case #6: Custom Other Network Config
            foreach (var setConfigAction in _setOtherConfigurationList)
            {
                // Custom configuration - No Changes on Timeout Config - Different Requests - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request2
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

            // Case #7: Changes on other Network Config
            foreach (var setConfigAction in _setOtherConfigurationList)
            {
                // Default configuration - No Changes on Timeout Config - Different Requests - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request2
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

            // Case #8: Custom timeout configuration
            foreach (var setConfigAction in _setTimeoutConfigurationList)
            {
                // Custom timeout configuration - No Changes - Different Requests - Cached Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request2
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

            // Case #9: Validate Case #1
            foreach (var setConfigAction in _setTimeoutConfigurationList)
            {
                // Default configuration - With Timeout Changes - Same Request - New Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request2
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

            // Case #10: Validate Case #2
            foreach (var setConfigAction in _setTimeoutConfigurationList)
            {
                // Default configuration - With Timeout Changes - Different Requests - New Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(),
                    // request2
                    new HttpRequestMessage(HttpMethod.Put, (Uri?)null),
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

            // Case #11: Validate Case #4
            foreach (var setConfigAction in _setTimeoutConfigurationList)
            {
                // Default configuration - With Timeout Changes - Different Requests (one file transfer) - New Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")),
                    // request2
                    new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")),
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

            // Case #12: Validate Case #5
            foreach (var setConfigAction in _setTimeoutConfigurationList)
            {
                // Default configuration - With Timeout Changes - Different Requests (one file transfer) - New Policy
                yield return new object[]
                {
                    // request1
                    new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/3.21/sites/{Guid.NewGuid()}/datasources/{Guid.NewGuid()}/content?includeExtract=true")),
                    // request2
                    new HttpRequestMessage(HttpMethod.Put, new Uri($"https://localhost/api/3.21/sites/{Guid.NewGuid()}/fileUploads/{Guid.NewGuid()}")),
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

        public SimpleCachedRequestTimeoutPolicyBuilderTests()
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
            var selector = serviceProvider.GetRequiredService<SimpleCachedRequestTimeoutPolicyBuilder>();

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