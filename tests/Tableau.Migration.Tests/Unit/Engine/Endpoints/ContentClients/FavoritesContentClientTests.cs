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
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Tableau.Migration.Api;
using Tableau.Migration.Config;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.ContentClients;
using Tableau.Migration.Paging;
using Tableau.Migration.Resources;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.ContentClients
{
    public class FavoritesContentClientTests
    {
        private readonly Mock<IFavoritesApiClient> _favoritesApiClientMock;
        private readonly Mock<IConfigReader> _configReaderMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly Mock<ISharedResourcesLocalizer> _localizerMock;
        private readonly FavoritesContentClient _favoritesContentClient;

        public FavoritesContentClientTests()
        {
            _favoritesApiClientMock = new Mock<IFavoritesApiClient>();
            _configReaderMock = new Mock<IConfigReader>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _localizerMock = new Mock<ISharedResourcesLocalizer>();

            _favoritesContentClient = new FavoritesContentClient(
                _favoritesApiClientMock.Object,
                _configReaderMock.Object,
                _loggerFactoryMock.Object,
                _localizerMock.Object
            );
        }

        [Fact]
        public async Task GetAllByUser_ShouldReturnFavorites_WhenPagerReturnsResults()
        {
            // Arrange
            var user = Mock.Of<IContentReference>();
            var favorites = ImmutableList.Create(Mock.Of<IFavorite>());
            var pagerMock = new Mock<IPager<IFavorite>>();
            pagerMock
                .Setup(p => p.GetAllPagesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IImmutableList<IFavorite>>.Succeeded(favorites));

            _favoritesApiClientMock
                .Setup(api => api.GetPagerForUser(user, It.IsAny<int>()))
                .Returns(pagerMock.Object);

            _configReaderMock
                .Setup(config => config.Get<IUser>())
                .Returns(new ContentTypesOptions { BatchSize = 10 });

            // Act
            var result = await _favoritesContentClient.GetAllByUserAsync(user, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(favorites, result.Value);
        }

        [Fact]
        public async Task GetAllByUser_ShouldReturnFailure_WhenPagerFails()
        {
            // Arrange
            var user = Mock.Of<IContentReference>();
            var pagerMock = new Mock<IPager<IFavorite>>();
            pagerMock
                .Setup(p => p.GetAllPagesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IImmutableList<IFavorite>>.Failed(new Exception("Pager error")));

            _favoritesApiClientMock
                .Setup(api => api.GetPagerForUser(user, It.IsAny<int>()))
                .Returns(pagerMock.Object);

            _configReaderMock
                .Setup(config => config.Get<IUser>())
                .Returns(new ContentTypesOptions { BatchSize = 10 });

            // Act
            var result = await _favoritesContentClient.GetAllByUserAsync(user, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Errors);
        }

        [Fact]
        public async Task DeleteFavoriteForUserId_ShouldReturnSuccess_WhenApiCallSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var favorite = Mock.Of<IFavorite>(f => f.ContentType == FavoriteContentType.Workbook && f.Content.Id == Guid.NewGuid());

            _favoritesApiClientMock
                .Setup(api => api.DeleteFavoriteAsync(userId, favorite.ContentType, favorite.Content.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Succeeded());

            // Act
            var result = await _favoritesContentClient.DeleteFavoriteForUserIdAsync(userId, favorite, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task DeleteFavoriteForUserId_ShouldReturnFailure_WhenApiCallFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var favorite = Mock.Of<IFavorite>(f => f.ContentType == FavoriteContentType.Workbook && f.Content.Id == Guid.NewGuid());

            _favoritesApiClientMock
                .Setup(api => api.DeleteFavoriteAsync(userId, favorite.ContentType, favorite.Content.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failed(new Exception("Delete error")));

            // Act
            var result = await _favoritesContentClient.DeleteFavoriteForUserIdAsync(userId, favorite, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.NotNull(result.Errors);
        }
    }
}
