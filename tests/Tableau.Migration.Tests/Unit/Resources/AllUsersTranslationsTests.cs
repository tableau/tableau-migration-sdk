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
using Tableau.Migration.Resources;
using Tableau.Migration.Tests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Resources
{
    public class AllUsersTranslationsTests
    {
        public abstract class AllUsersTranslationsTest : AutoFixtureTestBase
        {
            protected static readonly IImmutableList<string> Translations =
                typeof(AllUsersTranslations).GetFieldValue<IImmutableList<string>>("_translations")!;

            static AllUsersTranslationsTest()
            {
                Assert.NotNull(Translations);
                Assert.NotEmpty(Translations);
            }
        }

        public class GetAll : AllUsersTranslationsTest
        {
            [Fact]
            public void Returns_default_values()
            {
                var all = AllUsersTranslations.GetAll();

                Assert.True(all.SequenceEqual(Translations));
            }

            [Fact]
            public void Includes_extras()
            {
                var extras = CreateMany<string>(5);

                var all = AllUsersTranslations.GetAll(extras);

                Assert.True(all.SequenceEqual(Translations.Concat(extras)));
            }

            [Fact]
            public void Removes_duplicates()
            {
                var extra = Create<string>();

                var extras = Enumerable.Range(0, 5).Select(_ => extra);

                var all = AllUsersTranslations.GetAll(extras);

                Assert.Single(all.Where(t => t == extra));
            }

            [Fact]
            public void English_first()
            {
                const string ENGLISH = "All Users";

                var all = AllUsersTranslations.GetAll();

                Assert.Equal(ENGLISH, all[0]);

                var extras = CreateMany<string>(5);

                all = AllUsersTranslations.GetAll(extras);

                Assert.Equal(ENGLISH, all[0]);
            }
        }
    }
}
