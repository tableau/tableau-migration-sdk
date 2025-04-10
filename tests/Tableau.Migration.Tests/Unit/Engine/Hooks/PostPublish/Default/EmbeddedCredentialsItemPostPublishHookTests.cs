//
//  Copyright (c) 2025, Salesforce, Inc.
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
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.PostPublish;
using Tableau.Migration.Engine.Hooks.PostPublish.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.PostPublish.Default
{
    public class EmbeddedCredentialsItemPostPublishHookTests
    {
        public interface IEmbeddedCredentialsContentType : IContentReference, IRequiresEmbeddedCredentialMigration, IConnectionsContent
        { }
        public interface ITestContentType : IWithEmbeddedCredentials
        { }
        public class EmbeddedCredentialsContentType : TestContentType, IEmbeddedCredentialsContentType, ITestContentType
        {
            public EmbeddedCredentialsContentType(IImmutableList<IConnection> connections)
            {
                Connections = connections;
            }

            public IImmutableList<IConnection> Connections { get; set; } = ImmutableList.Create<IConnection>();
        }

        public class EmbeddedCredentialsItemPostPublishHookTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMigration> MockMigration = new();
            protected readonly Mock<IDestinationEndpoint> MockDestinationEndpoint = new();
            protected readonly Mock<ISourceEndpoint> MockSourceEndpoint = new();
            protected readonly Mock<IDestinationContentReferenceFinder<IUser>> MockUserContentFinder = new();
            protected readonly Mock<IUserSavedCredentialsCache> MockUserSavedCredentialsCache = new();
            protected readonly Mock<ILogger<EmbeddedCredentialsItemPostPublishHook<IEmbeddedCredentialsContentType, ITestContentType>>> MockLogger = new();
            protected readonly MockSharedResourcesLocalizer MockLocalizer = new();
            protected readonly Mock<IMigrationCapabilities> MockMigrationCapabilities = new();
            protected readonly EmbeddedCredentialsItemPostPublishHook<IEmbeddedCredentialsContentType, ITestContentType> Hook;

            public EmbeddedCredentialsItemPostPublishHookTest()
            {
                MockMigration.SetupGet(m => m.Destination).Returns(MockDestinationEndpoint.Object);
                MockMigration.SetupGet(m => m.Source).Returns(MockSourceEndpoint.Object);
                MockMigration.SetupGet(m => m.Plan).Returns(Create<IMigrationPlan>());
                MockMigration.SetupGet(m => m.Plan.Destination).Returns(Create<ITableauApiEndpointConfiguration>());
                MockDestinationEndpoint.Setup(m => m.GetSessionAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult((IResult<IServerSession>)Result<IServerSession>.Succeeded(Create<IServerSession>())));

                var mockDestinationFinderFactory = new Mock<IDestinationContentReferenceFinderFactory>();
                mockDestinationFinderFactory.Setup(dff => dff.ForDestinationContentType<IUser>()).Returns(MockUserContentFinder.Object);

                MockUserContentFinder.Setup(ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(Create<IContentReference?>()));

                var mockLoggerFactory = new Mock<ILoggerFactory>();
                mockLoggerFactory
                    .Setup(lf => lf.CreateLogger(It.IsAny<string>()))
                    .Returns(MockLogger.Object);
                MockMigrationCapabilities.SetupGet(mc => mc.EmbeddedCredentialsDisabled).Returns(false);

                Hook = new(
                    MockMigration.Object,
                    mockDestinationFinderFactory.Object,
                    MockUserSavedCredentialsCache.Object,
                    mockLoggerFactory.Object,
                    MockLocalizer.Object,
                    MockMigrationCapabilities.Object);
            }

            protected ContentItemPostPublishContext<IEmbeddedCredentialsContentType, ITestContentType> CreateContext(
                IEmbeddedCredentialsContentType sourceItem,
                EmbeddedCredentialsContentType destinationItem)
            {
                var manifestEntry = new MigrationManifestEntry(
                    Create<IMigrationManifestEntryBuilder>(),
                    new ContentReferenceStub(sourceItem))
                    .SetMigrated();

                Assert.Equal(MigrationManifestEntryStatus.Migrated, manifestEntry.Status);
                Assert.Empty(manifestEntry.Errors);


                var context = new ContentItemPostPublishContext<IEmbeddedCredentialsContentType, ITestContentType>(
                    manifestEntry,
                    sourceItem,
                    destinationItem);

                Assert.NotNull(context);

                return context;
            }

            protected IEmbeddedCredentialsContentType CreateSourceItem(
                bool embedPassword = false,
                bool useOAuthManagedKeychain = false)
            {
                var sourceConnections = AutoFixture
                    .Build<Connection>()
                    .With(c => c.EmbedPassword, () => embedPassword)
                    .With(c => c.UseOAuthManagedKeychain, () => useOAuthManagedKeychain)
                    .CreateMany()
                    .Select(c => (IConnection)c)
                    .ToImmutableList();

                Assert.NotNull(sourceConnections);

                var sourceItem = (IEmbeddedCredentialsContentType)new EmbeddedCredentialsContentType(sourceConnections);
                return sourceItem;
            }

            protected void SetupApplyKeychainAsync()
            {
                MockDestinationEndpoint.Setup(
                    m => m.ApplyKeychainsAsync<IEmbeddedCredentialsContentType>(
                        It.IsAny<Guid>(),
                        It.IsAny<IApplyKeychainOptions>(),
                        It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult((IResult)Result.Succeeded()));
            }

            protected void SetupRetrieveKeychainAsync(
                IEnumerable<string> encryptedKeychains,
                IEnumerable<Guid>? associatedUserIds = null)
            {
                var response = new RetrieveKeychainResponse(encryptedKeychains, associatedUserIds);
                SetupRetrieveKeychainAsync(new EmbeddedCredentialKeychainResult(response));
            }

            protected void SetupRetrieveKeychainAsync() => SetupRetrieveKeychainAsync(Create<IEmbeddedCredentialKeychainResult>());

            protected void SetupRetrieveUserSavedCredentialsAsync()
                => SetupRetrieveUserSavedCredentialsAsync(Create<IEmbeddedCredentialKeychainResult>());

            private void SetupRetrieveKeychainAsync(IEmbeddedCredentialKeychainResult keychainResult)
            {
                MockSourceEndpoint.Setup(m => m.RetrieveKeychainsAsync<IEmbeddedCredentialsContentType>(
                    It.IsAny<Guid>(),
                    It.IsAny<IDestinationSiteInfo>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(
                    Task.FromResult(
                        (IResult<IEmbeddedCredentialKeychainResult>)Result<IEmbeddedCredentialKeychainResult>
                        .Succeeded(keychainResult)));
            }

            private void SetupRetrieveUserSavedCredentialsAsync(IEmbeddedCredentialKeychainResult keychainResult)
                => MockSourceEndpoint.Setup(m => m.RetrieveUserSavedCredentialsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<IDestinationSiteInfo>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult((IResult<IEmbeddedCredentialKeychainResult>)Result<IEmbeddedCredentialKeychainResult>.Succeeded(keychainResult)));

            protected void SetupUploadUserSavedCredentialsAsync()
                => MockDestinationEndpoint.Setup(m => m.UploadUserSavedCredentialsAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((IResult)Result.Succeeded()));
            protected static void AssertSuccess(ContentItemPostPublishContext<IEmbeddedCredentialsContentType, ITestContentType> context, ContentItemPostPublishContext<IEmbeddedCredentialsContentType, ITestContentType>? result)
            {
                Assert.Same(context, result);
                Assert.NotNull(result);
                Assert.Equal(MigrationManifestEntryStatus.Migrated, result.ManifestEntry.Status);
                Assert.Empty(result.ManifestEntry.Errors);
            }
        }

        public class ExecuteAsync : EmbeddedCredentialsItemPostPublishHookTest
        {
            [Fact]
            public async Task Skips_when_no_embedded_creds()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();

                var sourceItem = CreateSourceItem(embedPassword: false);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());

                SetupRetrieveKeychainAsync();
                SetupApplyKeychainAsync();

                var result = await Hook.ExecuteAsync(context, Cancel);

                AssertSuccess(context, result);

                MockDestinationEndpoint.Verify(de => de.GetSessionAsync(It.IsAny<CancellationToken>()), Times.Never);
                MockUserContentFinder.Verify(ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task Skips_when_cabaility_disabled()
            {
                var manifestEntry = Create<IMigrationManifestEntryEditor>();

                var sourceItem = CreateSourceItem(embedPassword: true);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());

                MockMigrationCapabilities.SetupGet(mc => mc.EmbeddedCredentialsDisabled).Returns(true);
                SetupRetrieveKeychainAsync(CreateMany<string>());
                SetupApplyKeychainAsync();

                var result = await Hook.ExecuteAsync(context, Cancel);

                Assert.True(context.PublishedItem.HasEmbeddedPassword);
                AssertSuccess(context, result);

                MockDestinationEndpoint.Verify(de => de.GetSessionAsync(It.IsAny<CancellationToken>()), Times.Never);
                MockUserContentFinder.Verify(ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task Migrates_non_OAuth_embedded_creds()
            {
                var sourceItem = CreateSourceItem(embedPassword: true);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());

                SetupRetrieveKeychainAsync(CreateMany<string>());
                SetupApplyKeychainAsync();


                var result = await Hook.ExecuteAsync(context, Cancel);

                AssertSuccess(context, result);

                MockDestinationEndpoint.VerifyAll();
                MockUserContentFinder.Verify(ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            }

            [Fact]
            public async Task Warns_on_managed_OAuth_embedded_creds()
            {
                var sourceItem = CreateSourceItem(embedPassword: true, useOAuthManagedKeychain: true);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());

                var associatedUserIds = CreateMany<Guid>();
                SetupRetrieveKeychainAsync(CreateMany<string>(), associatedUserIds);
                SetupApplyKeychainAsync();
                SetupRetrieveUserSavedCredentialsAsync();
                SetupUploadUserSavedCredentialsAsync();

                var result = await Hook.ExecuteAsync(context, Cancel);

                AssertSuccess(context, result);

                MockDestinationEndpoint.VerifyAll();
                MockLogger.VerifyWarnings(Times.Once);

                MockUserContentFinder.Verify(
                    ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(associatedUserIds.Count()));
            }

            [Fact]
            public async Task Migrates_OAuth_embedded_creds()
            {
                var sourceItem = CreateSourceItem(embedPassword: true);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());
                var associatedUserIds = CreateMany<Guid>();

                SetupRetrieveKeychainAsync(CreateMany<string>(), associatedUserIds);
                SetupApplyKeychainAsync();
                SetupRetrieveUserSavedCredentialsAsync();
                SetupUploadUserSavedCredentialsAsync();

                var result = await Hook.ExecuteAsync(context, Cancel);

                AssertSuccess(context, result);

                MockDestinationEndpoint.VerifyAll();
                var userCount = associatedUserIds.Count();

                MockUserContentFinder.Verify(
                    ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(userCount));
                MockDestinationEndpoint.Verify(
                    mde => mde.UploadUserSavedCredentialsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(userCount));

                MockSourceEndpoint.Verify(
                    mse => mse.RetrieveUserSavedCredentialsAsync(It.IsAny<Guid>(), It.IsAny<IDestinationSiteInfo>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(userCount));
            }


            [Fact]
            public async Task Migrates_OAuth_embedded_creds_using_cache()
            {
                var sourceItem = CreateSourceItem(embedPassword: true);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());
                var associatedUserIds = CreateMany<Guid>();

                SetupRetrieveKeychainAsync(CreateMany<string>(), associatedUserIds);
                SetupApplyKeychainAsync();
                SetupRetrieveUserSavedCredentialsAsync();
                SetupUploadUserSavedCredentialsAsync();

                foreach (var item in associatedUserIds)
                {
                    MockUserSavedCredentialsCache.Setup(x => x.Get(item)).Returns(Create<IEmbeddedCredentialKeychainResult>());
                }
                var result = await Hook.ExecuteAsync(context, Cancel);

                AssertSuccess(context, result);

                MockDestinationEndpoint.VerifyAll();
                var userCount = associatedUserIds.Count();

                MockUserContentFinder.Verify(
                    ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(userCount));
                MockDestinationEndpoint.Verify(
                    mde => mde.UploadUserSavedCredentialsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(userCount));

                MockSourceEndpoint.Verify(
                    mse => mse.RetrieveUserSavedCredentialsAsync(It.IsAny<Guid>(), It.IsAny<IDestinationSiteInfo>(), It.IsAny<CancellationToken>()),
                    Times.Never);

                MockUserSavedCredentialsCache.Verify(
                    musc => musc.AddOrUpdate(It.IsAny<Guid>(), It.IsAny<IEmbeddedCredentialKeychainResult>())
                    , Times.Never);

            }

            [Fact]
            public async Task Migrates_OAuth_embedded_creds_and_updates_cache()
            {
                var sourceItem = CreateSourceItem(embedPassword: true);

                var context = CreateContext(sourceItem, Create<EmbeddedCredentialsContentType>());
                var associatedUserIds = CreateMany<Guid>();

                SetupRetrieveKeychainAsync(CreateMany<string>(), associatedUserIds);
                SetupApplyKeychainAsync();
                SetupRetrieveUserSavedCredentialsAsync();
                SetupUploadUserSavedCredentialsAsync();

                var result = await Hook.ExecuteAsync(context, Cancel);

                AssertSuccess(context, result);

                MockDestinationEndpoint.VerifyAll();
                var userCount = associatedUserIds.Count();

                MockUserContentFinder.Verify(
                    ucf => ucf.FindBySourceIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(userCount));
                MockDestinationEndpoint.Verify(
                    mde => mde.UploadUserSavedCredentialsAsync(It.IsAny<Guid>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(userCount));

                MockSourceEndpoint.Verify(
                    mse => mse.RetrieveUserSavedCredentialsAsync(It.IsAny<Guid>(), It.IsAny<IDestinationSiteInfo>(), It.IsAny<CancellationToken>()),
                    Times.Exactly(userCount));

                MockUserSavedCredentialsCache.Verify(
                  musc => musc.Get(It.IsAny<Guid>()),
                  Times.Exactly(userCount));

                MockUserSavedCredentialsCache.Verify(
                    muscc => muscc.AddOrUpdate(It.IsAny<Guid>(), It.IsAny<IEmbeddedCredentialKeychainResult>()),
                    Times.Exactly(userCount));

            }
        }
    }
}
