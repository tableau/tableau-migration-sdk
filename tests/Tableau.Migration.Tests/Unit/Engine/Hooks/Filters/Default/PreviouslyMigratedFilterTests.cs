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

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Options;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class PreviouslyMigratedFilterTests
    {
        public class ExecuteAsync : AutoFixtureTestBase
        {
            private IMigrationManifest? PreviousManifest;

            private PreviouslyMigratedFilterOptions _options = new();

            public ExecuteAsync()
            {
                PreviousManifest = Create<IMigrationManifest>();

                var mockInput = Freeze<Mock<IMigrationInput>>();
                mockInput.SetupGet(x => x.PreviousManifest).Returns(() => PreviousManifest);

                var mockOptionsProvider = Freeze<Mock<IMigrationPlanOptionsProvider<PreviouslyMigratedFilterOptions>>>();
                mockOptionsProvider.Setup(x => x.Get()).Returns(() => _options);
            }

            [Fact]
            public async Task DisabledNoPreviousManifestAsync()
            {
                PreviousManifest = null;

                var ctx = Create<ContentFilterContext<TestContentType>>();

                var filter = Create<PreviouslyMigratedFilter<TestContentType>>();
                var results = await filter.ExecuteAsync(ctx, Cancel);

                Assert.NotNull(results);
                Assert.Same(ctx, results);

                Assert.All(results.Items, i => Assert.Equal(FilterStatus.Migrate, i.Status));
            }

            [Fact]
            public async Task DisabledManuallyAsync()
            {
                _options = new() { Disabled = true };

                var ctx = Create<ContentFilterContext<TestContentType>>();

                var filter = Create<PreviouslyMigratedFilter<TestContentType>>();
                var results = await filter.ExecuteAsync(ctx, Cancel);

                Assert.NotNull(results);
                Assert.Same(ctx, results);

                Assert.All(results.Items, i => Assert.Equal(FilterStatus.Migrate, i.Status));
            }

            [Fact]
            public async Task FiltersPreviouslyMigratedAsync()
            {
                var ctx = Create<ContentFilterContext<TestContentType>>();

                var filter = Create<PreviouslyMigratedFilter<TestContentType>>();
                var results = await filter.ExecuteAsync(ctx, Cancel);

                Assert.NotNull(results);
                Assert.Same(ctx, results);

                var previouslyMigratedItems = results.Items.Where(i => i.ManifestEntry.HasMigrated).ToImmutableArray();
                var nonMigratedItems = results.Items.Except(previouslyMigratedItems).ToImmutableArray();

                Assert.NotEmpty(previouslyMigratedItems);
                Assert.All(previouslyMigratedItems, i => Assert.Equal(FilterStatus.Skip, i.Status));

                Assert.NotEmpty(nonMigratedItems);
                Assert.All(nonMigratedItems, i => Assert.Equal(FilterStatus.Migrate, i.Status));
            }
        }
    }
}
