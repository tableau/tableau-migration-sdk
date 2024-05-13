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

using System.Threading;
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public class AuthenticationTypeDomainMappingBaseTests
    {
        public class TestAuthenticationTypeDomainMapping : AuthenticationTypeDomainMappingBase
        {
            public int CallCount { get; private set; }

            protected override Task<ContentMappingContext<T>?> ExecuteAsync<T>(ContentMappingContext<T> context, CancellationToken cancel)
            {
                CallCount++;
                return context.ToTask();
            }
        }

        public class AuthenticationTypeDomainMappingBaseTest : AutoFixtureTestBase
        {
            protected readonly TestAuthenticationTypeDomainMapping Mapping;

            public AuthenticationTypeDomainMappingBaseTest()
            {
                Mapping = new();
            }
        }

        public class ExecuteAsync : AuthenticationTypeDomainMappingBaseTest
        {
            [Fact]
            public async Task WithUserAsync()
            {
                var ctx = Create<ContentMappingContext<IUser>>();

                var result = await Mapping.ExecuteAsync(ctx, Cancel);

                Assert.Equal(1, Mapping.CallCount);
                Assert.Same(result, ctx);
            }

            [Fact]
            public async Task WithGroupAsync()
            {
                var ctx = Create<ContentMappingContext<IGroup>>();

                var result = await Mapping.ExecuteAsync(ctx, Cancel);

                Assert.Equal(1, Mapping.CallCount);
                Assert.Same(result, ctx);
            }
        }
    }
}
