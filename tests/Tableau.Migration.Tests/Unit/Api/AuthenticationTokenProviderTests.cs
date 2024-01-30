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
            public async Task Handles_null_event()
            {
                // Does not throw
                await Provider.RequestRefreshAsync(default);
            }

            [Fact]
            public async Task Calls_refresh()
            {
                var count = 0;

                Provider.RefreshRequestedAsync += _ =>
                {
                    count++;
                    return Task.CompletedTask;
                };

                await Provider.RequestRefreshAsync(default);

                Assert.Equal(1, count);
            }
        }

        public class Set : AuthenticationTokenProviderTest
        {
            [Fact]
            public void Sets_token()
            {
                var token = Create<string>();

                Provider.Set(token);

                Assert.Equal(token, Provider.Token);
            }
        }

        public class Clear : AuthenticationTokenProviderTest
        {
            [Fact]
            public void Clears_token()
            {
                var token = Create<string>();

                Provider.Set(token);

                Assert.Equal(token, Provider.Token);

                Provider.Clear();

                Assert.Null(Provider.Token);
            }
        }
    }
}
