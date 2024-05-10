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
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class PermissionsItemPostPublishHookTests
    {
        public class PermissionsContentType : TestContentType, IPermissionsContent
        { }

        public class ResultContentType : TestContentType, IPermissionsContent, IContainerContent
        {
            public IContentReference Container { get; set; } = null!;
        }

        public abstract class PermissionsItemPostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
            protected readonly Mock<ISourceEndpoint> MockSourceEndpoint = new();
            protected readonly Mock<IDestinationEndpoint> MockDestinationEndpoint = new();
            protected readonly Mock<IPermissionsTransformer> MockPermissionsTransformer = new();
            protected readonly Mock<ILockedProjectCache> MockProjectCache = new();

            protected readonly PermissionsItemPostPublishHook<PermissionsContentType, ResultContentType> Hook;

            public PermissionsItemPostPublishHookTest()
            {
                MockMigration.SetupGet(m => m.Source).Returns(MockSourceEndpoint.Object);
                MockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);

                MockMigration.Setup(m => m.Pipeline.GetDestinationLockedProjectCache())
                    .Returns(MockProjectCache.Object);

                Hook = new(MockMigration.Object, MockPermissionsTransformer.Object);
            }
        }

        public class ExecuteAsync : PermissionsItemPostPublishHookTest
        {
            [Fact]
            public async Task Returns_when_source_permissions_fails()
            {
                var context = Create<ContentItemPostPublishContext<PermissionsContentType, ResultContentType>>();

                MockSourceEndpoint
                    .Setup(e => e.GetPermissionsAsync<PermissionsContentType>(It.IsAny<IContentReference>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IPermissions>.Failed(new Exception()));

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockPermissionsTransformer.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task Transforms_permissions()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var sourceItem = Create<PermissionsContentType>();
                var destinationItem = Create<ResultContentType>();

                var context = new ContentItemPostPublishContext<PermissionsContentType, ResultContentType>(manifestEntry, sourceItem, destinationItem);

                var sourcePermissions = AutoFixture.Build<Mock<IPermissions>>()
                    .Do(m => m.SetupGet(i => i.ParentId).Returns(sourceItem.Id))
                    .Create()
                    .Object;

                var sourceGrantees = sourcePermissions.GranteeCapabilities.ToImmutableArray();

                MockSourceEndpoint
                    .Setup(e => e.GetPermissionsAsync<PermissionsContentType>(It.IsAny<IContentReference>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IPermissions>.Succeeded(sourcePermissions));

                var destinationGrantees = CreateMany<IGranteeCapability>(5).ToImmutableArray();

                MockPermissionsTransformer.Setup(t => t.ExecuteAsync(sourcePermissions.GranteeCapabilities.ToImmutableArray(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(destinationGrantees);

                MockDestinationEndpoint
                    .Setup(e => e.UpdatePermissionsAsync<PermissionsContentType>(
                        destinationItem,
                        It.Is<IPermissions>(p => p.GranteeCapabilities.SequenceEqual(destinationGrantees) && p.ParentId == destinationItem.Id),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IPermissions>.Succeeded(Create<IPermissions>()));

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockPermissionsTransformer.Verify(t => t.ExecuteAsync(sourceGrantees, Cancel), Times.Once);

                MockDestinationEndpoint.VerifyAll();
            }

            [Fact]
            public async Task Does_not_run_when_parent_locked()
            {
                var context = Create<ContentItemPostPublishContext<PermissionsContentType, ResultContentType>>();

                Assert.NotNull(context.DestinationItem.Container);
                MockProjectCache.Setup(x => x.IsProjectLockedAsync(context.DestinationItem.Container.Id, Cancel, true))
                    .ReturnsAsync(true);

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockPermissionsTransformer.VerifyNoOtherCalls();
            }
        }
    }
}
