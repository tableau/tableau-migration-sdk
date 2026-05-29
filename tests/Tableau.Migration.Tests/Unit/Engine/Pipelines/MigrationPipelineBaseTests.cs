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

using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules.Cloud;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Conversion;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Caching;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Migrators;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;
using Tableau.Migration.Engine.Preparation;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class MigrationPipelineBaseTests
    {
        #region - Verify First Party Pipelines -

        [Fact]
        public void Verify_AllPipelinesHaveExpectedNumberOfActions()
        {
            MigrationPipelineTestBase<ServerToServerMigrationPipeline> ServerToServerPipeline = new();
            MigrationPipelineTestBase<ServerToCloudMigrationPipeline> ServerToCloudPipeline = new();
            MigrationPipelineTestBase<CloudToCloudMigrationPipeline> CloudToCloudPipeline = new();

            var serverToServerActions = ServerToServerPipeline.Pipeline.BuildActions();
            var serverToCloudActions = ServerToCloudPipeline.Pipeline.BuildActions();
            var cloudToCloudActions = CloudToCloudPipeline.Pipeline.BuildActions();

            Assert.NotEmpty(serverToServerActions);
            Assert.NotEmpty(serverToCloudActions);
            Assert.NotEmpty(cloudToCloudActions);

            Assert.Equal(serverToServerActions.Length, serverToCloudActions.Length);
            Assert.Equal(serverToCloudActions.Length, cloudToCloudActions.Length);
        }

        [Fact]
        public void Verify_AllPipelinesHaveExpectedNumberOfContentTypes()
        {
            Assert.NotEmpty(ServerToServerMigrationPipeline.ContentTypes);
            Assert.NotEmpty(ServerToCloudMigrationPipeline.ContentTypes);
            Assert.NotEmpty(CloudToCloudMigrationPipeline.ContentTypes);

            Assert.Equal(ServerToServerMigrationPipeline.ContentTypes.Length, ServerToCloudMigrationPipeline.ContentTypes.Length);
            Assert.Equal(ServerToCloudMigrationPipeline.ContentTypes.Length, CloudToCloudMigrationPipeline.ContentTypes.Length);
        }

        #endregion

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

        #region - GetContentLoader -

        public sealed class GetContentLoader : MigrationPipelineBaseTest
        {
            [Fact]
            public void GetsPlanService()
            {
                var loader = Freeze<IMigrationContentLoader<TestContentType>>();

                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.Services.GetService<IMigrationContentLoader<TestContentType>>(MockServices.Object))
                    .Returns(loader);                

                var result = Pipeline.GetContentLoader<TestContentType>();

                Assert.Same(loader, result);
                mockPlan.Verify(x => x.Services.GetService<IMigrationContentLoader<TestContentType>>(MockServices.Object), Times.Once);
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
                var preparer = Pipeline.GetItemPreparer<TestContentType, TestContentType, TestContentType>();

                Assert.IsType<SourceContentItemPreparer<TestContentType>>(preparer);
                MockServices.Verify(x => x.GetService(typeof(SourceContentItemPreparer<TestContentType>)), Times.Once);
            }

            [Fact]
            public void CreatesNonPullSourceItemPreparer()
            {
                var preparer = Pipeline.GetItemPreparer<TestContentType, TestContentType, TestPublishType>();

                Assert.IsType<SourceContentItemPreparer<TestContentType, TestPublishType>>(preparer);
                MockServices.Verify(x => x.GetService(typeof(SourceContentItemPreparer<TestContentType, TestPublishType>)), Times.Once);
            }

            [Fact]
            public void CreatesEndpointItemPreparer()
            {
                var preparer = Pipeline.GetItemPreparer<TestContentType, OtherTestContentType, OtherTestContentType>();

                Assert.IsType<EndpointContentItemPreparer<TestContentType, OtherTestContentType, OtherTestContentType>>(preparer);
                MockServices.Verify(x => x.GetService(typeof(EndpointContentItemPreparer<TestContentType, OtherTestContentType, OtherTestContentType>)), Times.Once);
            }

            [Fact]
            public void CreatesExtractRefreshTaskServerToCloudPreparer()
            {
                var preparer = Pipeline.GetItemPreparer<IServerExtractRefreshTask, ICloudExtractRefreshTask, ICloudExtractRefreshTask>();

                Assert.IsType<ExtractRefreshTaskServerToCloudPreparer>(preparer);
                MockServices.Verify(x => x.GetService(typeof(ExtractRefreshTaskServerToCloudPreparer)), Times.Once);
            }

            [Fact]
            public void CreatesFlowRunTaskServerToCloudPreparer()
            {
                var preparer = Pipeline.GetItemPreparer<IServerFlowRunTask, ICloudFlowRunTask, ICloudFlowRunTask>();

                Assert.IsType<FlowRunTaskServerToCloudPreparer>(preparer);
                MockServices.Verify(x => x.GetService(typeof(FlowRunTaskServerToCloudPreparer)), Times.Once);
            }
        }

        #endregion

        #region - GetItemConverter -

        public class GetItemConverter : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesIdentityConverter()
            {
                var converter = Pipeline.GetItemConverter<TestContentType, TestContentType>();

                Assert.IsType<DirectContentItemConverter<TestContentType, TestContentType>>(converter);
                MockServices.Verify(x => x.GetService(typeof(DirectContentItemConverter<TestContentType, TestContentType>)), Times.Once);
            }
        }

        #endregion

        #region - CreateSourceCache -

        public class CreateSourceCache : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesSourceTypedDestinationCache()
            {
                var cache = Pipeline.CreateSourceCache<IUser>();

                Assert.IsType<SourceCache<IUser>>(cache);
                MockServices.Verify(x => x.GetService(typeof(SourceCache<IUser>)), Times.Once);
            }
        }

        #endregion

        #region - CreateDestinationCache -

        public class CreateDestinationCache : MigrationPipelineBaseTest
        {
            [Fact]
            public void CreatesDefaultTypedDestinationCache()
            {
                var cache = Pipeline.CreateDestinationCache<IUser>();

                Assert.IsType<DestinationCache<IUser>>(cache);
                MockServices.Verify(x => x.GetService(typeof(DestinationCache<IUser>)), Times.Once);
            }

            [Fact]
            public void CreatesSpecializedProjectCache()
            {
                var cache = Pipeline.CreateDestinationCache<IProject>();

                Assert.IsType<DestinationProjectCache>(cache);
                MockServices.Verify(x => x.GetService(typeof(DestinationProjectCache)), Times.Once);
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

                Assert.IsType<DestinationProjectCache>(cache);
                MockServices.Verify(x => x.GetService(typeof(DestinationProjectCache)), Times.Once);
            }
        }

        #endregion

        #region - CreateSourceContentReferenceFinder -

        public sealed class CreateSourceContentReferenceFinder : MigrationPipelineBaseTest
        {
            [Fact]
            public void GetsPlanService()
            {
                var finder = Freeze<ISourceContentReferenceFinder<TestContentType>>();

                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.Services.GetService<ISourceContentReferenceFinder<TestContentType>>(MockServices.Object))
                    .Returns(finder);

                var result = Pipeline.CreateSourceContentReferenceFinder<TestContentType>();

                Assert.Same(finder, result);
                mockPlan.Verify(x => x.Services.GetService<ISourceContentReferenceFinder<TestContentType>>(MockServices.Object), Times.Once);
            }
        }

        #endregion

        #region - CreateDestinationContentReferenceFinder -

        public sealed class CreateDestinationContentReferenceFinder : MigrationPipelineBaseTest
        {
            [Fact]
            public void GetsPlanService()
            {
                var finder = Freeze<IDestinationContentReferenceFinder<TestContentType>>();

                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.Services.GetService<IDestinationContentReferenceFinder<TestContentType>>(MockServices.Object))
                    .Returns(finder);

                var result = Pipeline.CreateDestinationContentReferenceFinder<TestContentType>();

                Assert.Same(finder, result);
                mockPlan.Verify(x => x.Services.GetService<IDestinationContentReferenceFinder<TestContentType>>(MockServices.Object), Times.Once);
            }
        }

        #endregion

        #region - CreateSourceCacheLoadStrategy -

        public sealed class CreateSourceCacheLoadStrategy : MigrationPipelineBaseTest
        {
            [Fact]
            public void GetsPlanService()
            {
                var strategy = Freeze<IContentReferenceCacheLoadStrategy<TestContentType>>();
                var mockStrategyProvider = Freeze<Mock<IContentReferenceCacheLoadStrategyProvider<TestContentType>>>();
                mockStrategyProvider.Setup(x => x.GetSourceCacheLoadStrategy()).Returns(strategy);

                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.Services.GetService<IContentReferenceCacheLoadStrategyProvider<TestContentType>>(MockServices.Object))
                    .Returns(mockStrategyProvider.Object);

                var result = Pipeline.CreateSourceCacheLoadStrategy<TestContentType>();

                Assert.Same(strategy, result);
                mockPlan.Verify(x => x.Services.GetService<IContentReferenceCacheLoadStrategyProvider<TestContentType>>(MockServices.Object), Times.Once);
                mockStrategyProvider.Verify(x => x.GetSourceCacheLoadStrategy(), Times.Once);
            }
        }

        #endregion

        #region - CreateDestinationCacheLoadStrategy -

        public sealed class CreateDestinationCacheLoadStrategy : MigrationPipelineBaseTest
        {
            [Fact]
            public void GetsPlanService()
            {
                var strategy = Freeze<IContentReferenceCacheLoadStrategy<TestContentType>>();
                var mockStrategyProvider = Freeze<Mock<IContentReferenceCacheLoadStrategyProvider<TestContentType>>>();
                mockStrategyProvider.Setup(x => x.GetDestinationCacheLoadStrategy()).Returns(strategy);

                var mockPlan = Freeze<Mock<IMigrationPlan>>();
                mockPlan.Setup(x => x.Services.GetService<IContentReferenceCacheLoadStrategyProvider<TestContentType>>(MockServices.Object))
                    .Returns(mockStrategyProvider.Object);

                var result = Pipeline.CreateDestinationCacheLoadStrategy<TestContentType>();

                Assert.Same(strategy, result);
                mockPlan.Verify(x => x.Services.GetService<IContentReferenceCacheLoadStrategyProvider<TestContentType>>(MockServices.Object), Times.Once);
                mockStrategyProvider.Verify(x => x.GetDestinationCacheLoadStrategy(), Times.Once);
            }
        }

        #endregion
    }
}
