//
//  Copyright (c) 2026, Salesforce, Inc.
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

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public sealed class AsyncContentFilterBaseTests
    {
        public class TestFilter : AsyncContentFilterBase<TestContentType>
        {
            public TestFilter(ISharedResourcesLocalizer localizer, ILogger<IContentFilter<TestContentType>> logger)
                : base(localizer, logger)
            { }

            public bool PublicDisabled
            {
                get => base.Disabled;
                set => base.Disabled = value;
            }

            public Func<ContentMigrationItem<TestContentType>, Task<bool>> ShouldMigrateAsyncCallback { get; set; } 
                = i => Task.FromResult(true);

            public Func<ContentFilterContextItem<TestContentType>, Task>? FilterAsyncCallback { get; set; }

            public override async Task<bool> ShouldMigrateAsync(ContentMigrationItem<TestContentType> item, CancellationToken cancel)
                => await ShouldMigrateAsyncCallback(item);

            public override async Task FilterAsync(ContentFilterContextItem<TestContentType> item, CancellationToken cancel)
            {
                if (FilterAsyncCallback is not null)
                    await FilterAsyncCallback(item).ConfigureAwait(false);
                else
                    await base.FilterAsync(item, cancel);
            }
        }

        public sealed class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly MockSharedResourcesLocalizer MockLocalizer = new();
            private readonly Mock<ILogger<IContentFilter<TestContentType>>> MockLogger;

            public ExecuteAsync()
            {
                MockLogger = Create<Mock<ILogger<IContentFilter<TestContentType>>>>();
            }

            [Fact]
            public async Task DisabledSkipsAsync()
            {
                var filter = new TestFilter(MockLocalizer.Object, MockLogger.Object)
                {
                    PublicDisabled = true,
                    ShouldMigrateAsyncCallback = i => Task.FromResult(false)
                };

                var ctx = Create<ContentFilterContext<TestContentType>>();

                var results = await filter.ExecuteAsync(ctx, Cancel);

                Assert.NotNull(results);
                Assert.Same(ctx, results);
                Assert.All(results.Items, i => Assert.Equal(FilterStatus.Migrate, i.Status));
            }

            [Fact]
            public async Task AppliesShouldMigrateFilterAsync()
            {
                var ctx = Create<ContentFilterContext<TestContentType>>();

                MockLogger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);

                var filter = new TestFilter(MockLocalizer.Object, MockLogger.Object)
                {
                    ShouldMigrateAsyncCallback = i => Task.FromResult(ctx.Items.IndexOf(ctx.Items.Single(it => i == it)) % 2 == 0)
                };

                var results = await filter.ExecuteAsync(ctx, Cancel);

                Assert.NotNull(results);
                Assert.Same(ctx, results);

                var shouldMigrateItems = ctx.Items.Where(i => filter.ShouldMigrateAsyncCallback(i).Result).ToImmutableArray();
                var filterItems = ctx.Items.Except(shouldMigrateItems).ToImmutableArray();
                Assert.All(shouldMigrateItems, i => Assert.Equal(FilterStatus.Migrate, i.Status));
                Assert.All(filterItems, i => Assert.Equal(FilterStatus.Skip, i.Status));
            }

            [Fact]
            public async Task CanCascadeFilterAsync()
            {
                var ctx = Create<ContentFilterContext<TestContentType>>();

                MockLogger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);

                bool IsEvenItem(ContentFilterContextItem<TestContentType> i)
                    => ctx.Items.IndexOf(ctx.Items.Single(it => i == it)) % 2 == 0;

                var filter = new TestFilter(MockLocalizer.Object, MockLogger.Object)
                {
                    FilterAsyncCallback = i =>
                    {
                        if (IsEvenItem(i))
                            i.Status = FilterStatus.CascadeSkip;
                        return Task.CompletedTask;
                    }
                };

                var results = await filter.ExecuteAsync(ctx, Cancel);

                Assert.NotNull(results);
                Assert.Same(ctx, results);

                var cascadeFilterItems = ctx.Items.Where(IsEvenItem).ToImmutableArray();
                var migrateItems = ctx.Items.Except(cascadeFilterItems).ToImmutableArray();
                Assert.All(cascadeFilterItems, i => Assert.Equal(FilterStatus.CascadeSkip, i.Status));
                Assert.All(migrateItems, i => Assert.Equal(FilterStatus.Migrate, i.Status));
            }
        }
    }
}
