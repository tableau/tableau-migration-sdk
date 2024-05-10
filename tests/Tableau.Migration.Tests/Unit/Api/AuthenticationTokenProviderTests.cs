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

using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Api;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public class AuthenticationTokenProviderTests
    {
        public abstract class AuthenticationTokenProviderTest : AutoFixtureTestBase
        {
            internal readonly AuthenticationTokenProvider Provider = new();
        }

        public class RequestRefreshAsync : AuthenticationTokenProviderTest
        {
            [Fact]
            public async Task HandlesNullEventAsync()
            {
                // Does not throw
                await Provider.RequestRefreshAsync(null, default);
            }

            [Fact]
            public async Task CallsRefreshAsync()
            {
                var count = 0;

                Provider.RefreshRequestedAsync += _ =>
                {
                    count++;
                    return Task.FromResult<IResult<string>>(Result<string>.Succeeded(Create<string>()));
                };

                await Provider.RequestRefreshAsync(null, Cancel);

                Assert.Equal(1, count);
            }

            [Fact]
            public async Task SingleRefreshWithConcurrentCallsAsync()
            {
                var refreshCount = 0;

                Provider.RefreshRequestedAsync += _ =>
                {
                    refreshCount++;
                    return Task.FromResult<IResult<string>>(Result<string>.Succeeded(Create<string>()));
                };

                var oldToken = Create<string>();
                await Provider.SetAsync(oldToken, Cancel);

                var syncWait = new ManualResetEventSlim();

                async Task RefreshTokenAsync()
                {
                    syncWait.Wait(Cancel);

                    await Provider.RequestRefreshAsync(oldToken, Cancel);
                }

                var tasks = Enumerable.Range(0, 20)
                    .Select(i => Task.Run(RefreshTokenAsync))
                    .ToImmutableArray();

                syncWait.Set();

                await Task.WhenAll(tasks);

                Assert.Equal(1, refreshCount);
            }
        }

        public class Set : AuthenticationTokenProviderTest
        {
            [Fact]
            public async Task SetsTokenAsync()
            {
                var token = Create<string>();

                await Provider.SetAsync(token, Cancel);

                Assert.Equal(token, await Provider.GetAsync(Cancel));
            }
        }

        public class Clear : AuthenticationTokenProviderTest
        {
            [Fact]
            public async Task ClearsTokenAsync()
            {
                var token = Create<string>();

                await Provider.SetAsync(token, Cancel);

                Assert.Equal(token, await Provider.GetAsync(Cancel));

                await Provider.ClearAsync(Cancel);

                Assert.Null(await Provider.GetAsync(Cancel));
            }
        }
    }
}
