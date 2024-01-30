// Copyright (c) 2023, Salesforce, Inc.
//  SPDX-License-Identifier: Apache-2
//  
//  Licensed under the Apache License, Version 2.0 (the ""License"") 
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//  http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an ""AS IS"" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using System.Linq;
using System.Threading.Tasks;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public class ManifestSourceContentReferenceFinderTests
    {
        public class ManifestSourceContentReferenceFinderTest : AutoFixtureTestBase
        {
            protected readonly IMigrationManifestEditor Manifest;

            protected readonly ManifestSourceContentReferenceFinder<TestContentType> Finder;

            public ManifestSourceContentReferenceFinderTest()
            {
                Manifest = Create<MigrationManifest>();

                Finder = new ManifestSourceContentReferenceFinder<TestContentType>(Manifest);
            }
        }

        public class FindByIdAsync : ManifestSourceContentReferenceFinderTest
        {
            [Fact]
            public async Task FindsManifestReferenceAsync()
            {
                var sourceItem = Create<TestContentType>();

                var entry = Manifest.Entries.GetOrCreatePartition<TestContentType>().GetEntryBuilder(1)
                    .CreateEntries(new[] { sourceItem }, (i, e) => e)
                    .Single();

                var result = await Finder.FindByIdAsync(sourceItem.Id, Cancel);

                Assert.Same(entry.Source, result);
            }

            [Fact]
            public async Task NotFoundAsync()
            {
                var sourceItem = Create<TestContentType>();

                var result = await Finder.FindByIdAsync(sourceItem.Id, Cancel);

                Assert.Null(result);
            }
        }
    }
}
