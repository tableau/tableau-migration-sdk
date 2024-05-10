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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class PermissionPostPublishHookBaseTests
    {
        #region - Test Classes -

        public class TestPermissionPostPublishHook<TPublish, TResult> : PermissionPostPublishHookBase<TPublish, TResult>
        {
            public TestPermissionPostPublishHook(IMigration migration)
                : base(migration)
            { }

            public override Task<ContentItemPostPublishContext<TPublish, TResult>?> ExecuteAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
            {
                return ctx.ToTask();
            }

            public Guid? PublicGetDestinationProjectId(TResult result) 
                => GetDestinationProjectId(result);

            public async Task<bool> PublicParentProjectLockedAsync(ContentItemPostPublishContext<TPublish, TResult> ctx, CancellationToken cancel)
                => await ParentProjectLockedAsync(ctx, cancel);
        }

        public class PermissionPostPublishHookBaseTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration;
            protected readonly Mock<ILockedProjectCache> MockProjectCache;

            public PermissionPostPublishHookBaseTest()
            {
                MockMigration = Freeze<Mock<IMigration>>();

                MockProjectCache = Create<Mock<ILockedProjectCache>>();
                MockMigration.Setup(x => x.Pipeline.GetDestinationLockedProjectCache())
                    .Returns(MockProjectCache.Object);
            }
        }

        #endregion

        #region - GetDestinationContainerId -

        public class GetDestinationContainerId : PermissionPostPublishHookBaseTest
        {
            [Fact]
            public void ContainerContent()
            {
                var hook = Create<TestPermissionPostPublishHook<IPublishableWorkbook, IWorkbook>>();

                var wb = Create<IWorkbook>();

                var result = hook.PublicGetDestinationProjectId(wb);

                Assert.Equal(result, ((IContainerContent)wb).Container.Id);
            }

            [Fact]
            public void MappableContainerContent()
            {
                var hook = Create<TestPermissionPostPublishHook<IProject, IProject>>();

                var proj = Create<IProject>();

                var result = hook.PublicGetDestinationProjectId(proj);

                Assert.Equal(result, proj.Container?.Id);
            }

            [Fact]
            public void NotContainerType()
            {
                var hook = Create<TestPermissionPostPublishHook<TestContentType, TestContentType>>();

                var proj = Create<TestContentType>();

                var result = hook.PublicGetDestinationProjectId(proj);

                Assert.Null(result);
            }
        }

        #endregion

        #region - PublicParentProjectLockedAsync

        public class PublicParentProjectLockedAsync : PermissionPostPublishHookBaseTest
        {
            protected ContentItemPostPublishContext<TPublish, TResult> BuildContext<TPublish, TResult>(TResult result)
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var publishItem = Create<TPublish>();

                return new(manifestEntry, publishItem, result);
            }

            protected ContentItemPostPublishContext<TResult, TResult> BuildContext<TResult>(TResult result)
                => BuildContext<TResult, TResult>(result);

            [Fact]
            public async Task NoParentProjectAsync()
            {
                var mockProj = Create<Mock<IProject>>();
                mockProj.SetupGet(x => x.Container).Returns((IContentReference?)null);

                var hook = Create<TestPermissionPostPublishHook<IProject, IProject>>();

                var ctx = BuildContext(mockProj.Object);
                var result = await hook.PublicParentProjectLockedAsync(ctx, Cancel);

                Assert.False(result);

                MockProjectCache.Verify(x => x.IsProjectLockedAsync(It.IsAny<Guid>(), Cancel, It.IsAny<bool>()), Times.Never);
            }

            [Fact]
            public async Task ParentProjectLockedAsync()
            {
                var wb = Create<IWorkbook>();
                var projId = ((IContainerContent)wb).Container.Id;

                MockProjectCache.Setup(x => x.IsProjectLockedAsync(projId, Cancel, true))
                    .ReturnsAsync(true);

                var hook = Create<TestPermissionPostPublishHook<IPublishableWorkbook, IWorkbook>>();

                var ctx = BuildContext<IPublishableWorkbook, IWorkbook>(wb);
                var result = await hook.PublicParentProjectLockedAsync(ctx, Cancel);

                Assert.True(result);

                MockProjectCache.Verify(x => x.IsProjectLockedAsync(projId, Cancel, true), Times.Once);
            }

            [Fact]
            public async Task ProjectIgnoresWithoutNestedAsync()
            {
                var proj = Create<IProject>();
                
                Assert.NotNull(proj.Container);
                var parentId = proj.Container.Id;

                MockProjectCache.Setup(x => x.IsProjectLockedAsync(parentId, Cancel, false))
                    .ReturnsAsync(true);

                var hook = Create<TestPermissionPostPublishHook<IProject, IProject>>();

                var ctx = BuildContext<IProject, IProject>(proj);
                var result = await hook.PublicParentProjectLockedAsync(ctx, Cancel);

                Assert.True(result);

                MockProjectCache.Verify(x => x.IsProjectLockedAsync(parentId, Cancel, false), Times.Once);
            }
        }

        #endregion
    }
}
