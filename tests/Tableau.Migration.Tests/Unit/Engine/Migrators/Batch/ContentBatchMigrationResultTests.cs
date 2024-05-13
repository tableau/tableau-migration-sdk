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
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators.Batch
{
    public class ContentBatchMigrationResultTests
    {
        public class Succeeded : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultNextBatch()
            {
                var itemResults = CreateMany<IContentItemMigrationResult<TestContentType>>().ToImmutableArray();
                var r = ContentBatchMigrationResult<TestContentType>.Succeeded(itemResults);

                Assert.True(r.Success);
                Assert.True(r.PerformNextBatch);
                Assert.Empty(r.Errors);
                Assert.Equal(itemResults, r.ItemResults);
            }

            [Fact]
            public void ExplicitNextBatch()
            {
                var itemResults = CreateMany<IContentItemMigrationResult<TestContentType>>().ToImmutableArray();
                var r = ContentBatchMigrationResult<TestContentType>.Succeeded(itemResults, false);

                Assert.True(r.Success);
                Assert.False(r.PerformNextBatch);
                Assert.Empty(r.Errors);
                Assert.Equal(itemResults, r.ItemResults);
            }
        }

        public class Failed : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultNextBatch()
            {
                var itemResults = CreateMany<IContentItemMigrationResult<TestContentType>>().ToImmutableArray();
                var errors = new[] { new Exception() };
                var r = ContentBatchMigrationResult<TestContentType>.Failed(itemResults, errors);

                Assert.False(r.Success);
                Assert.True(r.PerformNextBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Equal(itemResults, r.ItemResults);
            }

            [Fact]
            public void ExplicitNextBatch()
            {
                var itemResults = CreateMany<IContentItemMigrationResult<TestContentType>>().ToImmutableArray();
                var errors = new[] { new Exception() };
                var r = ContentBatchMigrationResult<TestContentType>.Failed(itemResults, errors, false);

                Assert.False(r.Success);
                Assert.False(r.PerformNextBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Equal(itemResults, r.ItemResults);
            }
        }

        public class ForNextBatch : AutoFixtureTestBase
        {
            [Fact]
            public void ModifiedNextBatch()
            {
                var itemResults = CreateMany<IContentItemMigrationResult<TestContentType>>().ToImmutableArray();
                var r1 = ContentBatchMigrationResult<TestContentType>.Succeeded(itemResults);
                var r2 = r1.ForNextBatch(false);

                Assert.Equal(r1.Success, r2.Success);
                Assert.Equal(r1.Errors, r2.Errors);
                Assert.Equal(r1.ItemResults, r2.ItemResults);
                Assert.NotEqual(r1.PerformNextBatch, r2.PerformNextBatch);
            }
        }
    }
}
