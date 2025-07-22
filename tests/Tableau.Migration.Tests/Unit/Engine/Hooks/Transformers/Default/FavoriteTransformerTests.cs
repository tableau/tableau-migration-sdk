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
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Tableau.Migration.Engine.Hooks.Transformers.Default;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Hooks.Transformers.Default
{
    public sealed class FavoriteTransformerTests
    {
        public abstract class FavoriteTransformerTest : AutoFixtureTestBase
        {
            protected readonly Mock<IMappedUserTransformer> MockUserTransformer;
            protected readonly Mock<IDestinationViewReferenceFinder> MockViewFinder;
            protected readonly Mock<IDestinationContentReferenceFinderFactory> MockFinderFactory;

            internal readonly FavoriteTransformer Transformer;

            protected FavoriteContentType ContentType { get; set; }
            protected IFavorite Favorite { get; set; }

            protected IContentReference? DestinationUser { get; set; }
            protected IContentReference? DestinationContent { get; set; }

            public FavoriteTransformerTest()
            {
                ContentType = FavoriteContentType.Workbook;

                var mockFavorite = Create<Mock<IFavorite>>();
                mockFavorite.Setup(x => x.ContentType).Returns(() => ContentType);

                Favorite = mockFavorite.Object;

                DestinationUser = Create<IContentReference>();
                DestinationContent = Create<IContentReference>();

                MockUserTransformer = Freeze<Mock<IMappedUserTransformer>>();
                MockUserTransformer.Setup(x => x.ExecuteAsync(Favorite.User, Cancel))
                    .ReturnsAsync(() => DestinationUser);

                MockViewFinder = Freeze<Mock<IDestinationViewReferenceFinder>>();
                MockViewFinder.Setup(x => x.FindBySourceIdAsync(Favorite.Content.Id, Cancel))
                    .ReturnsAsync(() =>
                    {
                        if(DestinationContent is null)
                        {
                            return Result<IContentReference>.Failed(CreateMany<Exception>());
                        }
                        else
                        {
                            return Result<IContentReference>.Succeeded(DestinationContent);
                        }
                    });

                var mockFinder = Freeze<Mock<IDestinationContentReferenceFinder>>();
                mockFinder.Setup(x => x.FindBySourceLocationAsync(Favorite.Content.Location, Cancel))
                    .ReturnsAsync(() => DestinationContent);

                MockFinderFactory = Freeze<Mock<IDestinationContentReferenceFinderFactory>>();
                MockFinderFactory.Setup(x => x.ForFavoriteDestinationContentType(It.IsAny<FavoriteContentType>()))
                    .Returns(mockFinder.Object);

                Transformer = Create<FavoriteTransformer>();
            }
        }

        public sealed class TransformAsync : FavoriteTransformerTest
        {
            [Fact]
            public async Task UserUpdateFailureThrowsAsync()
            {
                DestinationUser = null;

                await Assert.ThrowsAsync<Exception>(() => Transformer.ExecuteAsync(Favorite, Cancel));
            }

            [Fact]
            public async Task UpdatesViewFavoriteAsync()
            {
                ContentType = FavoriteContentType.View;

                var result = await Transformer.ExecuteAsync(Favorite, Cancel);

                Assert.NotNull(result);
                Assert.Same(Favorite, result);
                Assert.Same(DestinationUser, result.User);
                Assert.Same(DestinationContent, result.Content);
            }

            [Fact]
            public async Task ViewUpdateFailureThrowsAsync()
            {
                ContentType = FavoriteContentType.View;

                DestinationContent = null;

                await Assert.ThrowsAsync<AggregateException>(() => Transformer.ExecuteAsync(Favorite, Cancel));
            }

            [Fact]
            public async Task UpdatesNonViewFavoriteAsync()
            {
                var result = await Transformer.ExecuteAsync(Favorite, Cancel);

                Assert.NotNull(result);
                Assert.Same(Favorite, result);
                Assert.Same(DestinationUser, result.User);
                Assert.Same(DestinationContent, result.Content);
            }

            [Fact]
            public async Task NonViewUpdateFailureThrowsAsync()
            {
                DestinationContent = null;

                await Assert.ThrowsAsync<Exception>(() => Transformer.ExecuteAsync(Favorite, Cancel));
            }
        }
    }
}
