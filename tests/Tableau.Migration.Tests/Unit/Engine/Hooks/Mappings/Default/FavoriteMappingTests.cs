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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Hooks.Mappings;
using Tableau.Migration.Engine.Hooks.Mappings.Default;
using Tableau.Migration.Engine.Manifest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Mappings.Default
{
    public sealed class FavoriteMappingTests
    {
        public abstract class FavoriteMappingTest : AutoFixtureTestBase
        {
            protected readonly Dictionary<ContentLocation, IMigrationManifestEntryEditor> UserManifestItems;
            protected readonly Dictionary<ContentLocation, IMigrationManifestEntryEditor> ContentManifestItems;

            protected readonly FavoriteMapping Mapping;

            public FavoriteMappingTest()
            {
                UserManifestItems = new();
                var mockUserManifestPartition = Create<Mock<IMigrationManifestContentTypePartitionEditor>>();
                mockUserManifestPartition.SetupGet(x => x.BySourceLocation).Returns(() => UserManifestItems);

                ContentManifestItems = new();
                var mockContentManifestPartition = Create<Mock<IMigrationManifestContentTypePartitionEditor>>();
                mockContentManifestPartition.SetupGet(x => x.BySourceLocation).Returns(() => ContentManifestItems);

                var mockEntryEditor = Freeze<Mock<IMigrationManifestEntryCollectionEditor>>();
                mockEntryEditor.Setup(x => x.GetOrCreatePartition(It.IsAny<Type>())).Returns(mockContentManifestPartition.Object);
                mockEntryEditor.Setup(x => x.GetOrCreatePartition<IUser>()).Returns(mockUserManifestPartition.Object);

                Mapping = Create<FavoriteMapping>();
            }

            protected ContentMappingContext<IFavorite> CreateMappingContext(FavoriteContentType contentType = FavoriteContentType.DataSource)
            {
                var mockFavorite = Create<Mock<IFavorite>>();
                mockFavorite.SetupGet(x => x.ContentType).Returns(contentType);

                return new ContentMappingContext<IFavorite>(mockFavorite.Object, mockFavorite.Object.Location);
            }

            protected void AssertMappedLocation(ContentMappingContext<IFavorite>? result, ContentLocation expectedUserLocation, ContentLocation expectedContentLocation)
            {
                Assert.NotNull(result);

                var expectedLocation = Favorite.BuildLocation(expectedUserLocation, result.ContentItem.ContentType, expectedContentLocation);

                Assert.Equal(expectedLocation, result.MappedLocation);
            }
        }

        public sealed class MapAsync : FavoriteMappingTest
        {
            [Fact]
            public async Task ManifestEntriesNotFoundAsync()
            {
                var ctx = CreateMappingContext();

                var result = await Mapping.MapAsync(ctx, Cancel);

                AssertMappedLocation(result, ctx.ContentItem.User.Location, ctx.ContentItem.Content.Location);
            }

            [Fact]
            public async Task MapsUserAndContentAsync()
            {
                var ctx = CreateMappingContext();

                var userManifest = Create<IMigrationManifestEntryEditor>();
                UserManifestItems[ctx.ContentItem.User.Location] = userManifest;

                var contentManifest = Create<IMigrationManifestEntryEditor>();
                ContentManifestItems[ctx.ContentItem.Content.Location] = contentManifest;

                var result = await Mapping.MapAsync(ctx, Cancel);

                AssertMappedLocation(result, userManifest.MappedLocation, contentManifest.MappedLocation);
            }
        }
    }
}
