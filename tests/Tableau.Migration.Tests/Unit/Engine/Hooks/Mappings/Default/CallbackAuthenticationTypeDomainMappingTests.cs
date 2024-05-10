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
    public class CallbackAuthenticationTypeDomainMappingTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly CancellationToken _cancel = new();

            [Fact]
            public async Task MapsDomainAsync()
            {
                var callback = (ContentMappingContext<IUsernameContent> ctx, CancellationToken cancel)
                    => Task.FromResult("myDomain");

                var mapper = new CallbackAuthenticationTypeDomainMapping(callback);

                var ctx = Create<ContentMappingContext<IUser>>();
                var result = await mapper.ExecuteAsync(ctx, _cancel);

                Assert.NotNull(result);
                Assert.NotSame(ctx, result);
                Assert.Same(ctx.ContentItem, result.ContentItem);

                var expectedLoc = ContentLocation.ForUsername("myDomain", ctx.MappedLocation.Name);
                Assert.Equal(expectedLoc, result.MappedLocation);
            }

            [Fact]
            public async Task CallbackReturnsNullAsync()
            {
                var callback = (ContentMappingContext<IUsernameContent> ctx, CancellationToken cancel)
                    => Task.FromResult((string?)null);

                var mapper = new CallbackAuthenticationTypeDomainMapping(callback);

                var ctx = Create<ContentMappingContext<IUser>>();
                var result = await mapper.ExecuteAsync(ctx, _cancel);

                Assert.NotNull(result);
                Assert.Same(ctx, result);
            }
        }
    }
}
