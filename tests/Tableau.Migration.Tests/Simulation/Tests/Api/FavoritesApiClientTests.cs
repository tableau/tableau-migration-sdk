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
using System.Threading.Tasks;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Simulation.Tests.Api
{
    public sealed class FavoritesApiClientTests
    {
        public abstract class FavoriteApiClientTest : ApiClientTestBase
        {
            public FavoriteApiClientTest()
            { }
        }

        #region - GetFavoritesForUserAsync -

        public sealed class GetFavoritesForUserAsync : FavoriteApiClientTest
        {
            [Fact]
            public async Task GetsFavoritesForUserAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Api.Data.Users.PickRandom();

                foreach (var contentType in Enum.GetValues<FavoriteContentType>())
                {
                    if (contentType is FavoriteContentType.Unknown)
                        continue;

                    switch (contentType)
                    {
                        case FavoriteContentType.DataSource:
                            var ds = Api.Data.CreateDataSource(AutoFixture);
                            Api.Data.AddUserFavorite(user.Id, FavoriteContentType.DataSource, ds.Id, ds.Name!);
                            break;
                        case FavoriteContentType.Flow:
                            var f = Api.Data.CreateFlow(AutoFixture);
                            Api.Data.AddUserFavorite(user.Id, FavoriteContentType.Flow, f.Id, f.Name!);
                            break;
                        case FavoriteContentType.Project:
                            var proj = Api.Data.CreateProject(AutoFixture);
                            Api.Data.AddUserFavorite(user.Id, FavoriteContentType.Project, proj.Id, proj.Name!);
                            break;
                        case FavoriteContentType.View:
                            var viewWorkbook = Api.Data.CreateWorkbook(AutoFixture);
                            var view = viewWorkbook.Views.PickRandom();
                            Api.Data.AddUserFavorite(user.Id, FavoriteContentType.View, view.Id, view.Name!);
                            break;
                        case FavoriteContentType.Workbook:
                            var wb = Api.Data.CreateWorkbook(AutoFixture);
                            Api.Data.AddUserFavorite(user.Id, FavoriteContentType.Workbook, wb.Id, wb.Name!);
                            break;
                        case FavoriteContentType.Collection:
                            // TODO: The collections feature is not fully developed.
                            // The simulation data used here is merely a stub.
                            // It should be fully fleshed out in collections feature work.
                            var coll = Api.Data.CreateCollection(AutoFixture);
                            Api.Data.AddUserFavorite(user.Id, FavoriteContentType.Collection, coll.Id, coll.Name!);
                            break;
                        default:
                            throw new Exception($"Content type {contentType} not supported in GetFavoritesForUserAsync.GetsFavoritesForUserAsync simulation test. Add support to test.");
                    }
                }

                var result = await sitesClient.Favorites.GetFavoritesForUserAsync(new User(user), 1, 100, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.NotEmpty(result.Value);
            }

            [Fact]
            public async Task EmptyAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = new User(Api.Data.Users.PickRandom());
                var result = await sitesClient.Favorites.GetFavoritesForUserAsync(user, 1, 100, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);
                Assert.Empty(result.Value);
            }

            [Fact]
            public async Task ErrorAsync()
            {
                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var badUser = Create<IContentReference>();
                var result = await sitesClient.Favorites.GetFavoritesForUserAsync(badUser, 1, 100, Cancel);

                result.AssertFailure();
            }
        }

        #endregion

        #region - DeleteFavoriteAsync -

        public sealed class DeleteFavoriteAsync : FavoriteApiClientTest
        {
            [Theory]
            [EnumData<FavoriteContentType>]
            public async Task DeletesFavoriteAsync(FavoriteContentType contentType)
            {
                if (contentType is FavoriteContentType.Unknown)
                    return;

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Api.Data.Users.PickRandom();
                var contentId = Guid.NewGuid();

                Api.Data.AddUserFavorite(user.Id, contentType, contentId, Create<string>());

                var result = await sitesClient.Favorites.DeleteFavoriteAsync(user.Id, contentType, contentId, Cancel);

                result.AssertSuccess();

                Assert.Empty(Api.Data.UserFavorites[user.Id]);
            }
        }

        #endregion

        #region - PublishAsync -

        public sealed class PublishAsync : FavoriteApiClientTest
        {
            [Theory]
            [EnumData<FavoriteContentType>]
            public async Task PublishesFavoriteAsync(FavoriteContentType contentType)
            {
                if (contentType is FavoriteContentType.Unknown)
                    return;

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Api.Data.Users.PickRandom();

                IContentReference contentRef;
                switch (contentType)
                {
                    case FavoriteContentType.DataSource:
                        var ds = Api.Data.CreateDataSource(AutoFixture);
                        contentRef = new ContentReferenceStub(ds.Id, string.Empty, new(CreateMany<string>()));
                        break;
                    case FavoriteContentType.Flow:
                        var f = Api.Data.CreateFlow(AutoFixture);
                        contentRef = new ContentReferenceStub(f.Id, string.Empty, new(CreateMany<string>()));
                        break;
                    case FavoriteContentType.Project:
                        var p = Api.Data.CreateProject(AutoFixture);
                        contentRef = new ContentReferenceStub(p.Id, string.Empty, new(CreateMany<string>()));
                        break;
                    case FavoriteContentType.View:
                        var viewWorkbook = Api.Data.CreateWorkbook(AutoFixture);
                        contentRef = new ContentReferenceStub(viewWorkbook.Views.PickRandom().Id, string.Empty, new(CreateMany<string>()));
                        break;
                    case FavoriteContentType.Workbook:
                        var wb = Api.Data.CreateWorkbook(AutoFixture);
                        contentRef = new ContentReferenceStub(wb.Id, string.Empty, new(CreateMany<string>()));
                        break;
                    case FavoriteContentType.Collection:
                        var coll = Api.Data.CreateCollection(AutoFixture);
                        contentRef = new ContentReferenceStub(coll.Id, string.Empty, new(CreateMany<string>()));
                        break;
                    default:
                        throw new ArgumentException("Content type not supported in PublishAsync.PublishesFavoriteAsync simulation test. Add support to test.");
                }

                var favorite = new Favorite(new User(user), contentType, contentRef, "test");

                var result = await sitesClient.Favorites.PublishAsync(favorite, Cancel);

                result.AssertSuccess();

                Assert.NotEmpty(Api.Data.UserFavorites[user.Id]);
            }

            [Theory]
            [EnumData<FavoriteContentType>]
            public async Task PublishInvalidAsync(FavoriteContentType contentType)
            {
                if (contentType is FavoriteContentType.Unknown)
                    return;

                await using var sitesClient = await GetSitesClientAsync(Cancel);

                var user = Api.Data.Users.PickRandom();
                var favorite = new Favorite(new User(user), contentType, Create<IContentReference>(), "test");

                var result = await sitesClient.Favorites.PublishAsync(favorite, Cancel);

                result.AssertFailure();

                Assert.Empty(Api.Data.UserFavorites[user.Id]);
            }
        }

        #endregion
    }
}
