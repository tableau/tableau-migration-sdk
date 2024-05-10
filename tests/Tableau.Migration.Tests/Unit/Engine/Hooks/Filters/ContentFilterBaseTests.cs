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

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Polly;
using Tableau.Migration.Resources;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentFilterBaseTests
    {
        public class TestFilter : ContentFilterBase<TestContentType>
        {
            public TestFilter(
                ISharedResourcesLocalizer localizer,
                ILogger<IContentFilter<TestContentType>> logger) 
                    : base(localizer, logger) { }

            public bool PublicDisabled
            {
                get => base.Disabled;
                set => base.Disabled = value;
            }

            public Func<ContentMigrationItem<TestContentType>, bool> FilterCallback { get; set; } = i => true;

            public override bool ShouldMigrate(ContentMigrationItem<TestContentType> item)
                => FilterCallback(item);
        }

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private readonly MockSharedResourcesLocalizer MockLocalizer = new();
            private readonly Mock<ILogger<IContentFilter<TestContentType>>> MockLogger = new();

            [Fact]
            public async Task NoAllocationOnDisabledAsync()
            {
                var filter = new TestFilter(MockLocalizer.Object, MockLogger.Object)
                {
                    PublicDisabled = true
                };

                var allItems = CreateMany<ContentMigrationItem<TestContentType>>();

                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.Same(allItems, results);
            }

            [Fact]
            public async Task AppliesFilterAsync()
            {
                var allItems = CreateMany<ContentMigrationItem<TestContentType>>().ToImmutableList();

                MockLogger.Setup(x => x.IsEnabled(LogLevel.Debug)).Returns(true);
                                
                var filter = new TestFilter(MockLocalizer.Object,MockLogger.Object)
                {
                    FilterCallback = i => allItems.IndexOf(i) % 2 == 0
                };

                var results = await filter.ExecuteAsync(allItems, Cancel);

                Assert.NotSame(allItems, results);
                Assert.Equal(allItems.Where(filter.FilterCallback), results);

                MockLogger.VerifyLogging(LogLevel.Debug, Times.AtLeastOnce());
            }
        }
    }
}
