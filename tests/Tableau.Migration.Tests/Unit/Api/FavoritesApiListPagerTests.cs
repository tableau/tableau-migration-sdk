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
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Content.Search;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public sealed class FavoritesApiListPagerTests
    {
        public abstract class FavoritesApiListPagerTest : AutoFixtureTestBase
        {
            protected Mock<IContentReferenceFinder<IUser>> MockUserFinder { get; }
            protected Mock<IFavoritesApiClient> MockFavoritesApiClient { get; }

            internal FavoritesApiListPager Pager { get; }

            protected List<IContentReference> UserReferences { get; }
            protected Dictionary<Guid, List<IFavorite>> UserFavorites { get; }

            protected int FavoriteBatchSize { get; } = 10;

            public FavoritesApiListPagerTest()
            {
                UserReferences = CreateMany<IContentReference>().ToList();
                UserFavorites = new();
                foreach(var user in UserReferences)
                {
                    UserFavorites[user.Id] = CreateMany<IFavorite>(2 * FavoriteBatchSize).ToList();
                }

                MockUserFinder = Freeze<Mock<IContentReferenceFinder<IUser>>>();
                MockUserFinder.Setup(x => x.FindAllAsync(Cancel))
                    .ReturnsAsync(() => UserReferences.ToImmutableArray());

                var mockFinderFactory = Freeze<Mock<IContentReferenceFinderFactory>>();
                mockFinderFactory.Setup(x => x.ForContentType<IUser>()).Returns(MockUserFinder.Object);

                MockFavoritesApiClient = Freeze<Mock<IFavoritesApiClient>>();
                MockFavoritesApiClient.Setup(x => x.GetPagerForUser(It.IsAny<IContentReference>(), It.IsAny<int>()))
                    .Returns((IContentReference user, int pageSize) => new MemoryPager<IFavorite>(UserFavorites[user.Id], pageSize));

                var favoritesConfig = Freeze<ContentTypesOptions>();
                favoritesConfig.BatchSize = FavoriteBatchSize;

                var mockConfigReader = Freeze<Mock<IConfigReader>>();
                mockConfigReader.Setup(x => x.Get<IFavorite>()).Returns(favoritesConfig);

                Pager = Create<FavoritesApiListPager>();
            }
        }

        #region - LoadItemsAsync -

        public sealed class LoadItemsAsync : FavoritesApiListPagerTest
        {
            [Fact]
            public async Task PagesAllFavoritesForAllUsersAsync()
            {
                var allFavorites = await ((IPager<IFavorite>)Pager).GetAllPagesAsync(Cancel);

                allFavorites.AssertSuccess();
                Assert.Equal(UserFavorites.Values.SelectMany(v => v).ToImmutableArray(), allFavorites.Value);

                MockUserFinder.Verify(x => x.FindAllAsync(Cancel), Times.Once);
                foreach(var user in UserReferences)
                {
                    MockFavoritesApiClient.Verify(x => x.GetPagerForUser(user, FavoriteBatchSize), Times.Once);
                }
            }
        }

        #endregion
    }
}
