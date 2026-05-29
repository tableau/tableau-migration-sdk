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
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public class ContentLocationInPathFilterTests : AutoFixtureTestBase
    {
        [Fact]
        public async Task FilterAsync()
        {
            //Setup
            // Create mock data
            var users = CreateMany<ContentMigrationItem<IUser>>().ToImmutableArray();

            // Choose one of the items and create a filter from it
            var user1 = users.FirstOrDefault();
            Assert.NotNull(user1);
            var filter1 = new ContentLocationInPathFilter<IUser>(user1.SourceItem.Location.Path,
                Create<ISharedResourcesLocalizer>(), Create < ILogger<IContentFilter<IUser>>>());


            // Act- Filter with the chosen filter
            var filterResult = await filter1.ExecuteAsync(new ContentFilterContext<IUser>(users), Cancel);

            // Verify - The filter returns only the item that matches the filter.
            Assert.NotNull(filterResult);
            var migrateItem = Assert.Single(filterResult.Items, i => i.Status is FilterStatus.Migrate);
            Assert.Equal(user1, migrateItem);
        }
    }
}
