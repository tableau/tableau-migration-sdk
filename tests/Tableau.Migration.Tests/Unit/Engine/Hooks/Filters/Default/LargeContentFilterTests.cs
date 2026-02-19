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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class LargeContentFilterTests
    {
        #region - Test Content Types -

        public class TestSizeContent : TestContentType, ISizeContent
        {
            public long Size { get; set; }
        }

        #endregion

        public class ExecuteAsync : AutoFixtureTestBase
        {
            private ContentTypesOptions _options = new();

            public ExecuteAsync()
            {
                var mockConfigReader = Freeze<Mock<IConfigReader>>();
                mockConfigReader.Setup(x => x.Get<IContentReference>()).Returns(() => _options);
            }

            private ContentMigrationItem<IContentReference> CreateSizeItem(long size)
            {
                var sizeContent = Create<TestSizeContent>();
                sizeContent.Size = size;
                var manifestEntry = new Mock<IMigrationManifestEntryEditor>().Object;
                return new ContentMigrationItem<IContentReference>(sizeContent, manifestEntry);
            }

            [Fact]
            public async Task AllowsAllContentWhenMaxSizeNotConfiguredAsync()
            {
                _options = new ContentTypesOptions(); // No max size set

                var items = Enumerable.Range(0, 10)
                    .Select(_ => CreateSizeItem(Create<long>()))
                    .ToImmutableList();

                var filter = Create<LargeContentFilter<IContentReference>>();
                var results = await filter.ExecuteAsync(items, Cancel);

                Assert.Equal(items, results);
            }

            [Fact]
            public async Task FiltersContentLargerThanMaxSizeAsync()
            {
                const long maxSize = 1048576L; // 1MB
                _options = new ContentTypesOptions { MaxContentSize = maxSize };

                var smallItem = CreateSizeItem(maxSize / 2); // 512KB
                var exactItem = CreateSizeItem(maxSize); // Exactly 1MB
                var largeItem = CreateSizeItem(maxSize * 2); // 2MB

                var items = ImmutableArray.Create(smallItem, exactItem, largeItem);

                var filter = Create<LargeContentFilter<IContentReference>>();
                var result = await filter.ExecuteAsync(items, Cancel);
                Assert.NotNull(result);
                var results = result.ToList();

                Assert.Equal(2, results.Count);
                Assert.Contains(smallItem, results);
                Assert.Contains(exactItem, results);
                Assert.DoesNotContain(largeItem, results);
            }

            [Fact]
            public async Task AllowsContentWithoutSizePropertyAsync()
            {
                const long maxSize = 1048576L; // 1MB
                _options = new ContentTypesOptions { MaxContentSize = maxSize };

                var items = CreateMany<ContentMigrationItem<IContentReference>>(10).ToImmutableList();

                var filter = Create<LargeContentFilter<IContentReference>>();
                var results = await filter.ExecuteAsync(items, Cancel);

                // Content without ISizeContent should not be filtered
                Assert.Equal(items, results);
            }

            [Fact]
            public async Task HandlesMixedContentTypesAsync()
            {
                const long maxSize = 1048576L; // 1MB
                _options = new ContentTypesOptions { MaxContentSize = maxSize };

                var sizeableSmall = CreateSizeItem(maxSize / 2);
                var sizeableLarge = CreateSizeItem(maxSize * 2);
                var nonSizeable = Create<ContentMigrationItem<IContentReference>>();

                var items = ImmutableArray.Create(
                    sizeableSmall, 
                    sizeableLarge, 
                    nonSizeable
                );

                var filter = Create<LargeContentFilter<IContentReference>>();
                var result = await filter.ExecuteAsync(items, Cancel);
                Assert.NotNull(result);
                var results = result.ToList();

                Assert.Equal(2, results.Count);
                Assert.Contains(sizeableSmall, results);
                Assert.Contains(nonSizeable, results);
                Assert.DoesNotContain(sizeableLarge, results);
            }

            [Fact]
            public async Task FiltersAllWhenAllExceedMaxSizeAsync()
            {
                const long maxSize = 1048576L; // 1MB
                _options = new ContentTypesOptions { MaxContentSize = maxSize };

                var items = Enumerable.Range(0, 10)
                    .Select(_ => CreateSizeItem(maxSize * 2)) // All are 2MB
                    .ToImmutableList();

                var filter = Create<LargeContentFilter<IContentReference>>();
                var result = await filter.ExecuteAsync(items, Cancel);
                Assert.NotNull(result);
                var results = result.ToList();

                Assert.Empty(results);
            }

            [Fact]
            public async Task AllowsAllWhenAllBelowMaxSizeAsync()
            {
                const long maxSize = 1048576L; // 1MB
                _options = new ContentTypesOptions { MaxContentSize = maxSize };

                var items = Enumerable.Range(0, 10)
                    .Select(_ => CreateSizeItem(maxSize / 2)) // All are 512KB
                    .ToImmutableList();

                var filter = Create<LargeContentFilter<IContentReference>>();
                var results = await filter.ExecuteAsync(items, Cancel);

                Assert.Equal(items, results);
            }
        }
    }
}
