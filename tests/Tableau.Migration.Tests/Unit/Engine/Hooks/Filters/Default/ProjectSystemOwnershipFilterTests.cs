using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public class ProjectSystemOwnershipFilterTests
    {
        public class ShouldMigrate : AutoFixtureTestBase
        {
            private readonly SystemOwnershipFilter<IProject> _filter;

            public ShouldMigrate()
            {
                _filter = new();
            }

            [Fact]
            public void False_when_External_Assets_Default_Project_translation()
            {
                var mockProject = Create<Mock<IProject>>();
                mockProject.SetupGet(g => g.Owner.Location).Returns(Constants.SystemUserLocation);

                var item = new ContentMigrationItem<IProject>(mockProject.Object, new Mock<IMigrationManifestEntryEditor>().Object);

                Assert.False(_filter.ShouldMigrate(item));
            }

            [Fact]
            public void True_when_not_External_Assets_Default_Project_translation()
            {
                var proj = Create<IProject>();

                var item = new ContentMigrationItem<IProject>(proj, new Mock<IMigrationManifestEntryEditor>().Object);

                Assert.True(_filter.ShouldMigrate(item));
            }
        }
    }
}
