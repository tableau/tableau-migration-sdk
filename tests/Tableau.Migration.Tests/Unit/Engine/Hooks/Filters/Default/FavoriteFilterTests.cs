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
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Engine.Hooks.Filters.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Filters.Default
{
    public sealed class FavoriteFilterTests
    {
        public abstract class FavoriteFilterTest : AutoFixtureTestBase
        {
            protected Dictionary<ContentLocation, IMigrationManifestEntryEditor> UserManifestEntries = new();
            protected Dictionary<ContentLocation, IMigrationManifestEntryEditor> ContentReferenceManifestEntries = new();
            protected Dictionary<Guid, IView> Views = new();

            protected readonly FavoriteFilter Filter;

            protected FavoriteFilterTest()
            {
                var mockUserEntries = Create<Mock<IMigrationManifestContentTypePartitionEditor>>();
                mockUserEntries.Setup(e => e.BySourceLocation).Returns(UserManifestEntries);

                var mockContentReferenceEntries = Create<Mock<IMigrationManifestContentTypePartitionEditor>>();
                mockContentReferenceEntries.Setup(e => e.BySourceLocation).Returns(ContentReferenceManifestEntries);

                var mockManifest = Freeze<Mock<IMigrationManifestEditor>>();
                mockManifest.Setup(m => m.Entries.GetOrCreatePartition<IUser>()).Returns(mockUserEntries.Object);
                mockManifest.Setup(m => m.Entries.GetOrCreatePartition(It.IsAny<Type>())).Returns(mockContentReferenceEntries.Object);
                mockManifest.Setup(m => m.Entries.GetOrCreatePartition<IWorkbook>()).Returns(mockContentReferenceEntries.Object);

                var mockViewsContentClient = Freeze<Mock<IViewsContentClient>>();
                mockViewsContentClient.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken cancel) =>
                    {
                        if (Views.TryGetValue(id, out var view))
                        {
                            return Result<IView>.Succeeded(view);
                        }
                        else
                        {
                            return Result<IView>.Failed(new Exception());
                        }
                    });

                Filter = Create<FavoriteFilter>();
            }

            protected ContentMigrationItem<IFavorite> SetupFavorite(FavoriteContentType contentType = FavoriteContentType.Workbook,
                bool addUserEntry = true, MigrationManifestEntryStatus userEntryStatus = MigrationManifestEntryStatus.Migrated,
                bool addContentReferenceEntry = true, MigrationManifestEntryStatus contentReferenceEntryStatus = MigrationManifestEntryStatus.Migrated)
            {
                var mockFavorite = Create<Mock<IFavorite>>();
                mockFavorite.Setup(f => f.ContentType).Returns(contentType);
                var favorite = mockFavorite.Object;

                if (addUserEntry)
                {
                    var mockUserEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                    mockUserEntry.Setup(m => m.Status).Returns(userEntryStatus);
                    UserManifestEntries[mockFavorite.Object.User.Location] = mockUserEntry.Object;
                }

                if (addContentReferenceEntry)
                {
                    var manifestLocation = favorite.Content.Location;

                    if (contentType is FavoriteContentType.View)
                    {
                        var mockView = Create<Mock<IView>>();
                        mockView.SetupGet(x => x.Id).Returns(favorite.Content.Id);
                        var view = mockView.Object;

                        Views[favorite.Content.Id] = view;
                        manifestLocation = view.ParentWorkbook.Location;
                    }

                    var mockContentReferenceEntry = Create<Mock<IMigrationManifestEntryEditor>>();
                    mockContentReferenceEntry.Setup(m => m.Status).Returns(contentReferenceEntryStatus);
                    ContentReferenceManifestEntries[manifestLocation] = mockContentReferenceEntry.Object;
                }

                return new ContentMigrationItem<IFavorite>(favorite, Create<IMigrationManifestEntryEditor>());
            }
        }

        public sealed class ShouldMigrateAsync : FavoriteFilterTest
        {
            [Theory]
            [InlineData(FavoriteContentType.Flow)]
            [InlineData(FavoriteContentType.Collection)]
            [InlineData(FavoriteContentType.Unknown)]
            public async Task UnsupportedFavoriteContentTypeAsync(FavoriteContentType contentType)
            {
                var favorite = SetupFavorite(contentType);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task UserNotFoundAsync()
            {
                var favorite = SetupFavorite(addUserEntry: false);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task UserSkippedAsync()
            {
                var favorite = SetupFavorite(userEntryStatus: MigrationManifestEntryStatus.Skipped);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task ContentReferenceNotFoundAsync()
            {
                var favorite = SetupFavorite(addContentReferenceEntry: false);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task ContentReferenceSkippedAsync()
            {
                var favorite = SetupFavorite(contentReferenceEntryStatus: MigrationManifestEntryStatus.Skipped);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task UserAndContentMigratedAsync()
            {
                var favorite = SetupFavorite();

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.True(result);
            }

            [Fact]
            public async Task ViewNotFoundAsync()
            {
                var favorite = SetupFavorite(contentType: FavoriteContentType.View, addContentReferenceEntry: true);
                Views.Clear();

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task ViewNoWorkbookEntryAsync()
            {
                var favorite = SetupFavorite(contentType: FavoriteContentType.View, addContentReferenceEntry: true);
                ContentReferenceManifestEntries.Clear();

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task ViewWorkbookSkippedAsync()
            {
                var favorite = SetupFavorite(contentType: FavoriteContentType.View, contentReferenceEntryStatus: MigrationManifestEntryStatus.Skipped);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.False(result);
            }

            [Fact]
            public async Task UserAndViewMigratedAsync()
            {
                var favorite = SetupFavorite(contentType: FavoriteContentType.View);

                var result = await Filter.ShouldMigrateAsync(favorite, Cancel);

                Assert.True(result);
            }
        }

    }
}