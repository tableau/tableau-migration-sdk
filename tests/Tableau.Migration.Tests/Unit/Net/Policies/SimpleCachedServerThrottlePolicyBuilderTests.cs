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
    public class SimpleCachedServerThrottlePolicyBuilderTests
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
            }
        };

        private static readonly Action<MigrationSdkOptions>[] _setServerThrottleConfigurationList = new Action<MigrationSdkOptions>[]
        {
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ServerThrottleRetryIntervals = new TimeSpan[]
                {
                    TimeSpan.FromMilliseconds(250)
                };
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ServerThrottleRetryIntervals = new TimeSpan[]
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
                options.Network.Resilience.ServerThrottleRetryIntervals = new TimeSpan[]
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
                options.Network.Resilience.ServerThrottleLimitRetries = true;
            },
            (MigrationSdkOptions options) =>
            {
                options.Network.Resilience.ServerThrottleLimitRetries = true;
                options.Network.Resilience.ServerThrottleRetryIntervals = new TimeSpan[]
                {
                    TimeSpan.FromMilliseconds(10),
                    TimeSpan.FromMilliseconds(20),
                    TimeSpan.FromMilliseconds(30),
                    TimeSpan.FromMilliseconds(40),
                    TimeSpan.FromMilliseconds(50)
                };
            }
        };

        #region - GetCachedPolicyCases -

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

            foreach (var setConfigAction in _setServerThrottleConfigurationList)
            {
                // Custom server throttle configuration - No Changes - Different Requests - Cached Policy
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

            foreach (var setConfigAction in _setServerThrottleConfigurationList)
            {
                // Default configuration - With server throttle Changes - Same Request - New Policy
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

        public SimpleCachedServerThrottlePolicyBuilderTests()
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
            var selector = serviceProvider.GetRequiredService<SimpleCachedServerThrottlePolicyBuilder>();

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
