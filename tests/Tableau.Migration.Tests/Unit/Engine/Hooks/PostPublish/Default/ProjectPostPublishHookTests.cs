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

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Permissions;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Tableau.Migration.Engine.Manifest;
using Tableau.Migration.Tests.Content.Permissions;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class ProjectPostPublishHookTests
    {
        public class ProjectPostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
            protected readonly Mock<ISourceApiEndpoint> MockSourceEndpoint = new();
            protected readonly Mock<IProjectsApiClient> MockSourceProjectsClient = new();
            protected readonly Mock<IDestinationApiEndpoint> MockDestinationEndpoint = new();
            protected readonly Mock<IProjectsApiClient> MockDestinationProjectsClient = new();
            protected readonly Mock<IPermissionsTransformer> MockPermissionsTransformer = new();
            protected readonly Mock<ILockedProjectCache> MockProjectCache = new();

            protected readonly ProjectPostPublishHook Hook;

            public ProjectPostPublishHookTest()
            {
                MockSourceEndpoint.SetupGet(e => e.SiteApi.Projects).Returns(MockSourceProjectsClient.Object);
                MockDestinationEndpoint.SetupGet(e => e.SiteApi.Projects).Returns(MockDestinationProjectsClient.Object);

                MockMigration.SetupGet(m => m.Source).Returns(MockSourceEndpoint.Object);
                MockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);

                MockMigration.Setup(x => x.Pipeline.GetDestinationLockedProjectCache())
                    .Returns(MockProjectCache.Object);

                Hook = CreateHook();
            }

            protected ProjectPostPublishHook CreateHook()
                => new(MockMigration.Object, MockPermissionsTransformer.Object);
        }

        public class Ctor : ProjectPostPublishHookTest
        {
            [Fact]
            public void Not_enabled_when_non_Api_endpoints()
            {
                MockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceEndpoint>().Object);
                MockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationEndpoint>().Object);

                var hook = CreateHook();

                Assert.False(hook.IsEnabled);
            }

            [Fact]
            public void Enabled_when_Api_endpoints()
            {
                MockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceApiEndpoint>().Object);
                MockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationApiEndpoint>().Object);

                var hook = CreateHook();

                Assert.True(hook.IsEnabled);
            }
        }

        public class ExecuteAsync : ProjectPostPublishHookTest
        {
            [Fact]
            public async Task SucceedsAsync()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();
                var sourceItem = Create<IProject>();
                var destinationItem = Create<IProject>();

                var context = new ContentItemPostPublishContext<IProject, IProject>(manifestEntry, sourceItem, destinationItem);

                var existingDefaultPermissions = ImmutableDictionary.CreateBuilder<string, IPermissions>();
                var transformedDefaultPermissions = ImmutableDictionary.CreateBuilder<string, IPermissions>();

                foreach (var contentTypeUrlSegment in DefaultPermissionsContentTypeUrlSegments.GetAll())
                {
                    existingDefaultPermissions.Add(contentTypeUrlSegment, Create<IPermissions>());
                    var transformedPermissions = Create<IPermissions>();
                    transformedDefaultPermissions.Add(contentTypeUrlSegment, transformedPermissions);

                    MockPermissionsTransformer.Setup(t => t.ExecuteAsync(existingDefaultPermissions[contentTypeUrlSegment].GranteeCapabilities.ToImmutableArray(), Cancel))
                        .ReturnsAsync(transformedPermissions.GranteeCapabilities.ToImmutableArray());
                }

                var existingDefaultPermissionsResult = Result<IImmutableDictionary<string, IPermissions>>.Succeeded(existingDefaultPermissions.ToImmutable());

                MockSourceProjectsClient
                    .Setup(c => c.GetAllDefaultPermissionsAsync(sourceItem.Id, Cancel))
                    .ReturnsAsync(existingDefaultPermissionsResult);

                MockDestinationProjectsClient
                    .Setup(e => e.UpdateAllDefaultPermissionsAsync(
                        destinationItem.Id,
                        It.Is<IReadOnlyDictionary<string, IPermissions>>(p => PermissionsEqual(transformedDefaultPermissions.ToImmutable(), p)),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Result<IImmutableDictionary<string, IPermissions>>.Succeeded(Create<IImmutableDictionary<string, IPermissions>>()));

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockProjectCache.Verify(x => x.UpdateLockedProjectCache(context.DestinationItem), Times.Once);

                MockSourceProjectsClient.VerifyAll();
                MockDestinationProjectsClient.VerifyAll();
            }

            [Fact]
            public async Task Does_not_run_when_not_enabled()
            {
                MockMigration.SetupGet(m => m.Source).Returns(new Mock<ISourceEndpoint>().Object);
                MockMigration.SetupGet(m => m.Destination).Returns(new Mock<IDestinationEndpoint>().Object);

                var context = Create<ContentItemPostPublishContext<IProject, IProject>>();

                var hook = CreateHook();

                var result = await hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockProjectCache.Verify(x => x.UpdateLockedProjectCache(context.DestinationItem), Times.Once);
                MockPermissionsTransformer.VerifyNoOtherCalls();
            }

            [Fact]
            public async Task Does_not_run_when_parent_locked()
            {
                var context = Create<ContentItemPostPublishContext<IProject, IProject>>();

                Assert.NotNull(context.DestinationItem.Container);
                MockProjectCache.Setup(x => x.IsProjectLockedAsync(context.DestinationItem.Container.Id, Cancel, false))
                    .ReturnsAsync(true);

                var hook = CreateHook();

                var result = await hook.ExecuteAsync(context, Cancel);

                Assert.Same(context, result);

                MockProjectCache.Verify(x => x.UpdateLockedProjectCache(context.DestinationItem), Times.Once);
                MockPermissionsTransformer.VerifyNoOtherCalls();
            }

            private static bool PermissionsEqual(IImmutableDictionary<string, IPermissions> expected, IReadOnlyDictionary<string, IPermissions> actual)
            {
                if (!expected.Keys.SequenceEqual(actual.Keys, k => k))
                    return false;

                foreach (var key in expected.Keys)
                {
                    if (!IGranteeCapabilityComparer.Instance.Equals(expected[key].GranteeCapabilities, actual[key].GranteeCapabilities))
                        return false;
                }

                return true;
            }
        }
    }
}
