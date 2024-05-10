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

using System.Collections.Immutable;
using System.Linq;
using Moq;
using Microsoft.Extensions.Logging;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class GroupAllUsersFilterTests
    {
        public abstract class GroupAllUsersFilterTest : OptionsHookTestBase<GroupAllUsersFilterOptions>
        {
            protected readonly MockSharedResourcesLocalizer MockLocalizer = new();
            protected readonly Mock<ILogger<IContentFilter<IGroup>>> MockLogger = new();
        }

        public class Ctor : GroupAllUsersFilterTest
        {
            [Fact]
            public void Initializes()
            {
                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object, MockLocalizer.Object, MockLogger.Object);

                var translations = filter.GetFieldValue("_allUsersTranslations") as IImmutableList<string>;

                Assert.NotNull(translations);

                Assert.Contains(translations, t => AllUsersTranslations.GetAll().Count(n => t == n) == 1);
            }

            [Fact]
            public void Initializes_with_options()
            {
                var extraTranslations = CreateMany<string>(5);
                Options = new GroupAllUsersFilterOptions(extraTranslations);

                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object, MockLocalizer.Object, MockLogger.Object);

                var translations = filter.GetFieldValue("_allUsersTranslations") as IImmutableList<string>;

                Assert.NotNull(translations);

                Assert.Contains(translations, t => extraTranslations.Count(n => t == n) == 1);
                Assert.Contains(translations, t => AllUsersTranslations.GetAll().Count(n => t == n) == 1);
            }
        }

        public class ShouldMigrate : GroupAllUsersFilterTest
        {
            [Fact]
            public void False_when_All_Users_translation()
            {
                var extraTranslations = CreateMany<string>(5);
                Options = new GroupAllUsersFilterOptions(extraTranslations);

                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object, MockLocalizer.Object, MockLogger.Object);

                var translations = AllUsersTranslations.GetAll(extraTranslations);

                foreach (var translation in translations)
                {
                    var mockGroup = new Mock<IGroup>();
                    mockGroup.SetupGet(g => g.Name).Returns(translation);

                    var item = new ContentMigrationItem<IGroup>(mockGroup.Object, new Mock<IMigrationManifestEntryEditor>().Object);

                    Assert.False(filter.ShouldMigrate(item));
                }
            }

            [Fact]
            public void True_when_not_All_Users_translation()
            {
                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object, MockLocalizer.Object, MockLogger.Object);

                var mockGroup = new Mock<IGroup>();
                mockGroup.SetupGet(g => g.Name).Returns(Create<string>());

                var item = new ContentMigrationItem<IGroup>(mockGroup.Object, new Mock<IMigrationManifestEntryEditor>().Object);

                Assert.True(filter.ShouldMigrate(item));
            }
        }
    }
}
