//
//  Copyright (c) 2025, Salesforce, Inc.
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

using System.Threading.Tasks;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Pulled;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Pulled
{
    public sealed class ContentItemPulledContextTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var entry = Create<IMigrationManifestEntryEditor>();
                var item = Create<TestContentType>();

                var ctx = new ContentItemPulledContext<TestContentType>(entry, item);
                
                Assert.Same(entry, ctx.ManifestEntry);
                Assert.Same(item, ctx.PulledItem);
                Assert.Equal(FilterStatus.Migrate, ctx.Status);
            }
        }

        public sealed class ToTask : AutoFixtureTestBase
        {
            [Fact]
            public async Task CreatesTaskFromContextAsync()
            {
                var ctx = Create<ContentItemPulledContext<TestContentType>>();

                var taskCtx = await ctx.ToTask();

                Assert.Same(ctx, taskCtx);
            }
        }
    }
}
