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

using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters
{
    public sealed class ContentFilterContextItemTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var item = Create<IUser>();

                var result = new ContentFilterContextItem<IUser>(item, manifestEntry);

                Assert.Same(manifestEntry, result.ManifestEntry);
                Assert.Same(item, result.SourceItem);
                Assert.Equal(FilterStatus.Migrate, result.Status);
            }
        }
    }
}
