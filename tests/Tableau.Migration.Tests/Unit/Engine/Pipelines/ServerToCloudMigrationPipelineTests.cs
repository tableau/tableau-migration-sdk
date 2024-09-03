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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Moq;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Schedules;
using Tableau.Migration.Content.Schedules.Server;
using Tableau.Migration.Engine.Actions;
using Tableau.Migration.Engine.Migrators.Batch;
using Tableau.Migration.Engine.Pipelines;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Pipelines
{
    public class ServerToCloudMigrationPipelineTests
    {
        private static Type GetContentType(object o)
                => o.GetType().GenericTypeArguments.First();

        private static Type GetPublishType(object o)
        {
            var t = o.GetType();
            if (t.GenericTypeArguments.Length == 1)
                return t.GenericTypeArguments[0];

            return t.GenericTypeArguments[1];
        }

        public class ContentTypes : MigrationPipelineTestBase<ServerToCloudMigrationPipeline>
        {
            public static IEnumerable<object[]> ContentTypesData
                => ServerToCloudMigrationPipeline.ContentTypes.Select(t => new[] { t });

            private object GetBatchMigrator(Type contentType)
            {
                return typeof(ServerToCloudMigrationPipeline)
                    .GetMethod(nameof(ServerToCloudMigrationPipeline.GetBatchMigrator))
                    !.MakeGenericMethod(contentType)
                    .Invoke(Pipeline, null)!;
            }

            [Theory]
            [MemberData(nameof(ContentTypesData))]
            public void MatchesPipelineMigrationActions(MigrationPipelineContentType type)
            {
                var actions = Pipeline.BuildActions();
                var migrateActions = actions.Where(a => a is not PreflightAction).ToImmutableArray();

                Assert.True(migrateActions.Any(a => type.ContentType == GetContentType(a)),
                    $"Each item in {nameof(ServerToCloudMigrationPipeline)}.{nameof(ContentTypes)} should have a migration action defined in {nameof(BuildPipeline)}.");
                var matchingAction = migrateActions.Single(a => type.ContentType == GetContentType(a));

                var contentTypeIndex = ServerToCloudMigrationPipeline.ContentTypes.IndexOf(type);
                Assert.Equal(contentTypeIndex, migrateActions.IndexOf(matchingAction));
            }

            [Theory]
            [MemberData(nameof(ContentTypesData))]
            public void HasCorrectPublisher(MigrationPipelineContentType type)
            {
                var migrator = GetBatchMigrator(type.ContentType);
                var migratorPublishType = GetPublishType(migrator);

                Assert.True(type.PublishType == migratorPublishType,
                    $"The publish type defined for content type {type.ContentType.Name} should be {type.PublishType.Name}, but the migrator uses {migratorPublishType.Name}");
            }
        }

        public class BuildPipeline : MigrationPipelineTestBase<ServerToCloudMigrationPipeline>
        {
            [Fact]
            public void BuildsPipeline()
            {
                var actions = Pipeline.BuildActions();

                Assert.Equal(8, actions.Length);
                Assert.IsType<PreflightAction>(actions[0]);
                Assert.IsType<MigrateContentAction<IUser>>(actions[1]);
                Assert.IsType<MigrateContentAction<IGroup>>(actions[2]);
                Assert.IsType<MigrateContentAction<IProject>>(actions[3]);
                Assert.IsType<MigrateContentAction<IDataSource>>(actions[4]);
                Assert.IsType<MigrateContentAction<IWorkbook>>(actions[5]);
                Assert.IsType<MigrateContentAction<IServerExtractRefreshTask>>(actions[6]);
                Assert.IsType<MigrateContentAction<ICustomView>>(actions[7]);
            }

            [Fact]
            public void MatchesContentTypeDefinitions()
            {
                var actions = Pipeline.BuildActions();
                var migrateActions = actions.Where(a => a is not PreflightAction).ToImmutableArray();

                Assert.All(migrateActions, (a, migrationActionIndex) =>
                {
                    Assert.True(ServerToCloudMigrationPipeline.ContentTypes.Any(t => t.ContentType == GetContentType(a)),
                    $"Each migration action should have a definition in {nameof(ServerToCloudMigrationPipeline)}.{nameof(ContentTypes)}.");

                    var matchingType = ServerToCloudMigrationPipeline.ContentTypes.Single(t => t.ContentType == GetContentType(a));

                    var contentTypeIndex = ServerToCloudMigrationPipeline.ContentTypes.IndexOf(matchingType);
                    Assert.Equal(migrationActionIndex, contentTypeIndex);
                });
            }
        }

        public class GetBatchMigrator : MigrationPipelineTestBase<ServerToCloudMigrationPipeline>
        {
            [Fact]
            public void CreatesDefaultBatchMigrator()
            {
                var migrator = Pipeline.GetBatchMigrator<TestContentType>();

                Assert.IsType<ItemPublishContentBatchMigrator<TestContentType>>(migrator);
                MockServices.Verify(x => x.GetService(typeof(ItemPublishContentBatchMigrator<TestContentType>)), Times.Once);
            }

            [Fact]
            public void CreatesDefaultUserBatchMigrator()
            {
                var migrator = Pipeline.GetBatchMigrator<IUser>();

                Assert.IsType<ItemPublishContentBatchMigrator<IUser>>(migrator);
                MockServices.Verify(x => x.GetService(typeof(ItemPublishContentBatchMigrator<IUser>)), Times.Once);
            }
        }

        public class GetUserBatchMigrator : MigrationPipelineTestBase<ServerToCloudMigrationPipeline>
        {
            protected override ServerToCloudMigrationPipeline CreatePipeline()
            {
                var config = new ContentTypesOptions
                {
                    BatchPublishingEnabled = true
                };

                var mockConfigReader = Freeze<Mock<IConfigReader>>();

                mockConfigReader.Setup(x => x.Get<IUser>())
                    .Returns(config);

                return base.CreatePipeline();
            }

            [Fact]
            public void CreatesUserBatchMigrator()
            {
                var migrator = Pipeline.GetBatchMigrator<IUser>();

                Assert.IsType<BulkPublishContentBatchMigrator<IUser>>(migrator);
                MockServices.Verify(x => x.GetService(typeof(BulkPublishContentBatchMigrator<IUser>)), Times.Once);
            }
        }
    }
}
