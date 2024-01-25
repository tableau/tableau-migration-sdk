using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Manifest
{
    public class MigrationManifestFactoryTests
    {
        private interface ITestContent : IContentReference
        { }

        public class Create : AutoFixtureTestBase
        {
            private readonly Mock<ISharedResourcesLocalizer> _mockLocalizer;
            private readonly Mock<ILoggerFactory> _mockLoggerFactory;
            private readonly MigrationManifestFactory _factory;

            private readonly Mock<IMigrationInput> _mockInput;
            private readonly Guid _planId;

            private readonly Guid _migrationId;

            public Create()
            {
                _mockLocalizer = Freeze<Mock<ISharedResourcesLocalizer>>();
                _mockLoggerFactory = Freeze<Mock<ILoggerFactory>>();
                _factory = Create<MigrationManifestFactory>();

                _mockInput = Create<Mock<IMigrationInput>>();

                _migrationId = Guid.NewGuid();

                _planId = Guid.NewGuid();
                _mockInput.SetupGet(x => x.Plan.PlanId).Returns(_planId);
                _mockInput.SetupGet(x => x.MigrationId).Returns(_migrationId);
            }

            [Fact]
            public void InitializesEmptyManifestWithInput()
            {
                var manifest = _factory.Create(_mockInput.Object, _migrationId);

                Assert.Equal(_mockInput.Object.Plan.PlanId, manifest.PlanId);
                Assert.Equal(_migrationId, manifest.MigrationId);

                Assert.Empty(manifest.Entries);
            }

            [Fact]
            public void InitializesEmptyManifestWithoutInput()
            {
                var manifest = _factory.Create(_planId, _migrationId);

                Assert.Equal(_planId, manifest.PlanId);
                Assert.Equal(_migrationId, manifest.MigrationId);

                Assert.Empty(manifest.Entries);
            }

            [Fact]
            public void CreatesInstancesWithInput()
            {
                var manifest1 = _factory.Create(_mockInput.Object, _migrationId);
                var manifest2 = _factory.Create(_mockInput.Object, _migrationId);

                Assert.NotSame(manifest1, manifest2);
            }

            [Fact]
            public void CreatesInstancesWithoutInput()
            {
                var manifest1 = _factory.Create(_planId, _migrationId);
                var manifest2 = _factory.Create(_planId, _migrationId);

                Assert.NotSame(manifest1, manifest2);
            }

            //TODO (W-14313275): Previous manifest is not used
            //until it can be fully tested/made working.
            [Fact(Skip = "Previous manifest is not used until it can be fully tested/made working.")]
            public void CopiesFromPreviousManifest()
            {
                var previousEntries = CreateMany<IMigrationManifestEntry>().ToImmutableArray();

                var previousManifest = Create<MigrationManifest>();
                previousManifest.Entries
                    .GetOrCreatePartition<ITestContent>()
                    .CreateEntries(previousEntries);

                _mockInput.SetupGet(x => x.PreviousManifest).Returns(previousManifest);

                var manifest = _factory.Create(_mockInput.Object, _migrationId);

                Assert.Equal(previousManifest.Entries.Count(), manifest.Entries.Count());
            }
        }
    }
}
