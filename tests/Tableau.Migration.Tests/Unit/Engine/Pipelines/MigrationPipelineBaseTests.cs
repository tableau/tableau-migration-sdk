using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineBaseTests
    {
        public class MigrationPipelineBaseTest : MigrationPipelineTestBase<TestPipeline>
        { }

        #region - BuildActions -

        public class BuildActions : MigrationPipelineBaseTest
        {
            [Fact]
            public void BuildsPipeline()
            {
                var actions = Pipeline.BuildActions();
                Assert.Equal(1, Pipeline.BuildPipelineCalls);
            }
        }

        #endregion

        #region - CreateAction -

        public class CreateAction : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesAction()
            {
                var result1 = Pipeline.CreateAction<TestAction>();
                var result2 = Pipeline.CreateAction<TestAction>();

                Assert.NotSame(result1, result2);

                MockServices.Verify(x => x.GetService(typeof(TestAction)), Times.Exactly(2 + Pipeline.BuildActions().Length));
            }
        }

        #endregion

        #region - CreateMigrationContentAction -

        public class CreateMigrateContentAction : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesAction()
            {
                var result1 = Pipeline.CreateMigrateContentAction<TestContentType>();
                var result2 = Pipeline.CreateMigrateContentAction<TestContentType>();

                Assert.NotSame(result1, result2);
                Assert.IsType<MigrateContentAction<TestContentType>>(result1);
                Assert.IsType<MigrateContentAction<TestContentType>>(result2);

                MockServices.Verify(x => x.GetService(typeof(MigrateContentAction<TestContentType>)), Times.Exactly(2));
            }
        }

        #endregion

        #region - GetMigrator -

        public class GetMigrator : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesDefaultMigrator()
            {
                var migrator = Pipeline.GetMigrator<TestContentType>();

                Assert.IsType<ContentMigrator<TestContentType>>(migrator);
                MockServices.Verify(x => x.GetService(typeof(ContentMigrator<TestContentType>)), Times.Once);
            }
        }

        #endregion

        #region - GetBatchMigrator -

        public class GetBatchMigrator : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesDefaultBatchMigrator()
            {
                var migrator = Pipeline.GetBatchMigrator<TestContentType>();

                Assert.IsType<ItemPublishContentBatchMigrator<TestContentType>>(migrator);
                MockServices.Verify(x => x.GetService(typeof(ItemPublishContentBatchMigrator<TestContentType>)), Times.Once);
            }
        }

        #endregion

        #region - GetItemPreparer -

        public class GetItemPreparer : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesDefaultSourceItemPreparer()
            {
                var migrator = Pipeline.GetItemPreparer<TestContentType, TestContentType>();

                Assert.IsType<SourceContentItemPreparer<TestContentType>>(migrator);
                MockServices.Verify(x => x.GetService(typeof(SourceContentItemPreparer<TestContentType>)), Times.Once);
            }
        }

        #endregion

        #region - CreateDestinationCache -

        public class CreateDestinationCache : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesDefaultTypedDestinationCache()
            {
                var cache = Pipeline.CreateDestinationCache<TestContentType>();

                Assert.IsType<BulkDestinationCache<TestContentType>>(cache);
                MockServices.Verify(x => x.GetService(typeof(BulkDestinationCache<TestContentType>)), Times.Once);
            }

            [Fact]
            public void CreatesSpecializedProjectCache()
            {
                var cache = Pipeline.CreateDestinationCache<IProject>();

                Assert.IsType<BulkDestinationProjectCache>(cache);
                MockServices.Verify(x => x.GetService(typeof(BulkDestinationProjectCache)), Times.Once);
            }
        }

        #endregion

        #region - CreateDestinationFinder -

        public class CreateDestinationFinder : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesLocationFinder()
            {
                var cache = Pipeline.CreateDestinationFinder<TestContentType>();

                Assert.IsType<ManifestDestinationContentReferenceFinder<TestContentType>>(cache);
                MockServices.Verify(x => x.GetService(typeof(ManifestDestinationContentReferenceFinder<TestContentType>)), Times.Once);
            }
        }

        #endregion

        #region - GetDestinationLockedProjectCache -

        public class GetDestinationLockedProjectCache : MigrationPipelineBaseTest
        {
            [Fact]
            public void GetsDestinationProjectCache()
            {
                var cache = Pipeline.GetDestinationLockedProjectCache();

                Assert.IsType<BulkDestinationProjectCache>(cache);
                MockServices.Verify(x => x.GetService(typeof(BulkDestinationProjectCache)), Times.Once);
            }
        }

        #endregion
    }
}
