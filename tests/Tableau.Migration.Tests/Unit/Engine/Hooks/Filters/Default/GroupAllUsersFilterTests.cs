using System.Collections.Immutable;
using System.Linq;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class GroupAllUsersFilterTests
    {
        public abstract class GroupAllUsersFilterTest : OptionsHookTestBase<GroupAllUsersFilterOptions>
        { }

        public class Ctor : GroupAllUsersFilterTest
        {
            [Fact]
            public void Initializes()
            {
                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object);

                var translations = filter.GetFieldValue("_allUsersTranslations") as IImmutableList<string>;

                Assert.NotNull(translations);

                Assert.Contains(translations, t => AllUsersTranslations.GetAll().Count(n => t == n) == 1);
            }

            [Fact]
            public void Initializes_with_options()
            {
                var extraTranslations = CreateMany<string>(5);
                Options = new GroupAllUsersFilterOptions(extraTranslations);

                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object);

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

                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object);

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
                var filter = new GroupAllUsersFilter(MockOptionsProvider.Object);

                var mockGroup = new Mock<IGroup>();
                mockGroup.SetupGet(g => g.Name).Returns(Create<string>());

                var item = new ContentMigrationItem<IGroup>(mockGroup.Object, new Mock<IMigrationManifestEntryEditor>().Object);

                Assert.True(filter.ShouldMigrate(item));
            }
        }
    }
}
