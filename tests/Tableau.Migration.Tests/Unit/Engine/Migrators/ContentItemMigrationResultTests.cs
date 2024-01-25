using System;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Engine.Migrators;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Migrators
{
    public class ContentItemMigrationResultTests
    {
        public class Succeeded : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultNextBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var r = ContentItemMigrationResult<TestContentType>.Succeeded(entry);

                Assert.True(r.Success);
                Assert.True(r.ContinueBatch);
                Assert.Empty(r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }

            [Fact]
            public void ExplicitContinueBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var r = ContentItemMigrationResult<TestContentType>.Succeeded(entry, false);

                Assert.True(r.Success);
                Assert.False(r.ContinueBatch);
                Assert.Empty(r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }
        }

        public class Failed : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultContinueBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var errors = new[] { new Exception() };
                var r = ContentItemMigrationResult<TestContentType>.Failed(entry, errors);

                Assert.False(r.Success);
                Assert.True(r.ContinueBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }

            [Fact]
            public void ExplicitContinueBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var errors = new[] { new Exception() };
                var r = ContentItemMigrationResult<TestContentType>.Failed(entry, errors, false);

                Assert.False(r.Success);
                Assert.False(r.ContinueBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }
        }

        public class Canceled : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultContinueBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var errors = new[] { new Exception() };
                var r = ContentItemMigrationResult<TestContentType>.Canceled(entry, errors);

                Assert.False(r.Success);
                Assert.True(r.IsCanceled);
                Assert.True(r.ContinueBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }

            [Fact]
            public void ExplicitContinueBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var errors = new[] { new Exception() };
                var r = ContentItemMigrationResult<TestContentType>.Canceled(entry, errors, false);

                Assert.False(r.Success);
                Assert.True(r.IsCanceled);
                Assert.False(r.ContinueBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }
        }

        public class FromResult : AutoFixtureTestBase
        {
            [Fact]
            public void CopiesSuccess()
            {
                var resultToCopy = Result.Succeeded();
                var entry = Create<IMigrationManifestEntry>();
                var r = ContentItemMigrationResult<TestContentType>.FromResult(resultToCopy, entry);

                r.AssertSuccess();
                Assert.True(r.ContinueBatch);
                Assert.Same(entry, r.ManifestEntry);
            }

            [Fact]
            public void DefaultContinueBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var errors = new[] { new Exception() };
                var resultToCopy = Result.Failed(errors);
                var r = ContentItemMigrationResult<TestContentType>.FromResult(resultToCopy, entry);

                r.AssertFailure();
                Assert.True(r.ContinueBatch);
                Assert.Equal(errors, r.Errors);
                Assert.Same(entry, r.ManifestEntry);
            }

            [Fact]
            public void ExplicitContinueBatch()
            {
                var resultToCopy = Result.Succeeded();
                var entry = Create<IMigrationManifestEntry>();
                var r = ContentItemMigrationResult<TestContentType>.FromResult(resultToCopy, entry, false);

                r.AssertSuccess();
                Assert.False(r.ContinueBatch);
                Assert.Same(entry, r.ManifestEntry);
            }
        }

        public class ForContinueBatch : AutoFixtureTestBase
        {
            [Fact]
            public void ModifiedNextBatch()
            {
                var entry = Create<IMigrationManifestEntry>();
                var r1 = ContentItemMigrationResult<TestContentType>.Succeeded(entry);
                var r2 = r1.ForContinueBatch(false);

                Assert.Equal(r1.Success, r2.Success);
                Assert.Equal(r1.Errors, r2.Errors);
                Assert.Same(r1.ManifestEntry, r2.ManifestEntry);
                Assert.NotEqual(r1.ContinueBatch, r2.ContinueBatch);
            }
        }
    }
}
