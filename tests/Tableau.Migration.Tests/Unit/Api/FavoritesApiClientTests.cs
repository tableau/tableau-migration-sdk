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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Tableau.Migration.Paging;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api
{
    public sealed class FavoritesApiClientTests
    {
        public abstract class FavoritesApiClientTest : ApiClientTestBase<IFavoritesApiClient>
        {
            public FavoritesApiClientTest()
            {
                IContentReference CreateIdRef(Guid id)
                {
                    var mockRef = Create<Mock<IContentReference>>();
                    mockRef.SetupGet(x => x.Id).Returns(id);
                    return mockRef.Object;
                }

                MockDataSourceFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken cancel) => CreateIdRef(id));

                MockProjectFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken cancel) => CreateIdRef(id));

                MockWorkbookFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken cancel) => CreateIdRef(id));
                
                MockCollectionFinder.Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), Cancel))
                    .ReturnsAsync((Guid id, CancellationToken cancel) => CreateIdRef(id));

            }
        }

        #region - GetFavoritesForUserAsync -

        public sealed class GetFavoritesForUserAsync : FavoritesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                SetupExceptionResponse<FavoritesResponse>();

                var user = Create<IContentReference>();
                var result = await ApiClient.GetFavoritesForUserAsync(user, 1, 100, Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                SetupErrorResponse<FavoritesResponse>();

                var user = Create<IContentReference>();
                var result = await ApiClient.GetFavoritesForUserAsync(user, 1, 100, Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }

            [Fact]
            public async Task SuccessAsync()
            {
                SetupSuccessResponse<FavoritesResponse>(r =>
                {
                    var items = new List<FavoritesResponse.FavoriteType>();

                    foreach(var contentType in Enum.GetValues<FavoriteContentType>())
                    {
                        if (contentType is FavoriteContentType.Unknown)
                            continue;

                        items.Add(contentType switch
                        {
                            FavoriteContentType.DataSource => new FavoritesResponse.FavoriteType { Label = Create<string>(), DataSource = Create<FavoritesResponse.FavoriteType.DataSourceType>() },
                            FavoriteContentType.Flow => new FavoritesResponse.FavoriteType { Label = Create<string>(), Flow = Create<FavoritesResponse.FavoriteType.FlowType>() },
                            FavoriteContentType.Project => new FavoritesResponse.FavoriteType { Label = Create<string>(), Project = Create<FavoritesResponse.FavoriteType.ProjectType>() },
                            FavoriteContentType.View => new FavoritesResponse.FavoriteType { Label = Create<string>(), View = Create<FavoritesResponse.FavoriteType.ViewType>() },
                            FavoriteContentType.Workbook => new FavoritesResponse.FavoriteType { Label = Create<string>(), Workbook = Create<FavoritesResponse.FavoriteType.WorkbookType>() },
                            FavoriteContentType.Collection => new FavoritesResponse.FavoriteType { Label = Create<string>(), Collection = Create<FavoritesResponse.FavoriteType.CollectionType>() },
                            _ => throw new Exception("Content type not supported in GetFavoritesForUserAsync.SuccessAsync. Add support to test.")
                        });
                    }

                    r.Items = items.ToArray();
                });
                
                var user = Create<IContentReference>();
                var result = await ApiClient.GetFavoritesForUserAsync(user, 1, 100, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }

            [Fact]
            public async Task UnknownContentTypeAsync()
            {
                SetupSuccessResponse<FavoritesResponse>(r =>
                {
                    r.Items = [new FavoritesResponse.FavoriteType { Label = Create<string>() }];
                });

                var user = Create<IContentReference>();
                var result = await ApiClient.GetFavoritesForUserAsync(user, 1, 100, Cancel);

                result.AssertSuccess();

                var favorites= result.Value;
                Assert.NotNull(favorites);
                Assert.Empty(favorites);
                MockLogger.VerifyWarnings(Times.Once());

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }
        }

        #endregion

        #region - GetPagerForUser -
        
        public sealed class GetPagerForUser : FavoritesApiClientTest
        {
            [Fact]
            public void GetsPagerForUser()
            {
                var user = Create<IContentReference>();
                var pager = ApiClient.GetPagerForUser(user, 10);

                Assert.IsType<CallbackPager<IFavorite>>(pager);
            }
        }

        #endregion

        #region - AddFavoriteAsync -

        public sealed class AddFavoriteAsync : FavoritesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                SetupExceptionResponse<FavoritesResponse>();

                var user = Create<IContentReference>();
                var result = await ApiClient.AddFavoriteAsync(user, "label", FavoriteContentType.Workbook, Guid.NewGuid(), Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                SetupErrorResponse<FavoritesResponse>();

                var user = Create<IContentReference>();
                var result = await ApiClient.AddFavoriteAsync(user, "label", FavoriteContentType.Workbook, Guid.NewGuid(), Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }

            [Theory]
            [EnumData<FavoriteContentType>]
            public async Task SuccessAsync(FavoriteContentType contentType)
            {
                // Unknown content type is not supposed to succeed.
                if(contentType == FavoriteContentType.Unknown)
                {
                    return;
                }

                SetupSuccessResponse<FavoritesResponse>();

                var user = Create<IContentReference>();
                var result = await ApiClient.AddFavoriteAsync(user, "label", contentType, Guid.NewGuid(), Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{user.Id}");
            }
        }

        #endregion

        #region - DeleteFavoriteAsync -

        public sealed class DeleteFavoriteAsync : FavoritesApiClientTest
        {
            [Fact]
            public async Task ErrorAsync()
            {
                SetupExceptionResponse();

                var userId = Guid.NewGuid();
                var contentId = Guid.NewGuid();
                var result = await ApiClient.DeleteFavoriteAsync(userId, FavoriteContentType.Workbook, contentId, Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{userId}/workbooks/{contentId}");
            }

            [Fact]
            public async Task FailureResponseAsync()
            {
                SetupErrorResponse();

                var userId = Guid.NewGuid();
                var contentId = Guid.NewGuid();
                var result = await ApiClient.DeleteFavoriteAsync(userId, FavoriteContentType.Workbook, contentId, Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{userId}/workbooks/{contentId}");
            }

            [Theory]
            [EnumData<FavoriteContentType>]
            public async Task SuccessAsync(FavoriteContentType contentType)
            {
                SetupSuccessResponse();

                var userId = Guid.NewGuid();
                var contentId = Guid.NewGuid();

                if (contentType is FavoriteContentType.Unknown)
                {
                    await Assert.ThrowsAsync<ArgumentException>(() => ApiClient.DeleteFavoriteAsync(userId, contentType, contentId, Cancel));
                }
                else
                {
                    var result = await ApiClient.DeleteFavoriteAsync(userId, contentType, contentId, Cancel);

                    result.AssertSuccess();

                    var expectedContentTypeUrlSegment = contentType.ToString().ToLower() + "s";

                    var request = MockHttpClient.AssertSingleRequest();
                    request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{userId}/{expectedContentTypeUrlSegment}/{contentId}");
                }
            }
        }

        #endregion

        #region - PublishAsync -

        public sealed class PublishAsync : FavoritesApiClientTest
        {
            [Fact]
            public async Task AddFailsAsync()
            {
                SetupErrorResponse<FavoritesResponse>();

                var mockFavorite = Create<Mock<IFavorite>>();
                mockFavorite.SetupGet(x => x.ContentType).Returns(FavoriteContentType.Workbook);

                var result = await ApiClient.PublishAsync(mockFavorite.Object, Cancel);

                result.AssertFailure();

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{mockFavorite.Object.User.Id}");
            }

            [Fact]
            public async Task SuccessReturnsFavoriteAsync()
            {
                var mockFavorite = Create<Mock<IFavorite>>();
                mockFavorite.SetupGet(x => x.ContentType).Returns(FavoriteContentType.Workbook);
                
                var favorite = mockFavorite.Object;

                SetupSuccessResponse<FavoritesResponse>(c =>
                {
                    var matchItem = new FavoritesResponse.FavoriteType
                    {
                        Label = favorite.Label,
                        Workbook = new() { Id = favorite.Content.Id }
                    };

                    c.Items = c.Items.Append(matchItem).ToArray();
                });
                
                var result = await ApiClient.PublishAsync(favorite, Cancel);

                result.AssertSuccess();
                Assert.NotNull(result.Value);

                Assert.NotSame(favorite, result.Value);

                Assert.Same(favorite.User, result.Value.User);
                Assert.Equal(favorite.Label, result.Value.Label);
                Assert.Equal(favorite.ContentType, result.Value.ContentType);
                Assert.Equal(favorite.Content.Id, result.Value.Content.Id);

                var request = MockHttpClient.AssertSingleRequest();
                request.AssertRelativeUri($"/api/{TableauServerVersion.RestApiVersion}/sites/{SiteId}/favorites/{favorite.User.Id}");
            }
        }

        #endregion

        #region - GetPager -

        public sealed class GetPager : FavoritesApiClientTest
        {
            [Fact]
            public void GetsFavoritesPager()
            {
                var p = ApiClient.GetPager(100);
                Assert.IsType<FavoritesApiListPager>(p);
            }
        }

        #endregion
    }
}
