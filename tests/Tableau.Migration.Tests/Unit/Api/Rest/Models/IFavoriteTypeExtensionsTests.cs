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
using Tableau.Migration.Api.Rest.Models;
using Tableau.Migration.Api.Rest.Models.Responses;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public sealed class IFavoriteTypeExtensionsTests
    {
        public class IFavoriteTypeExtensionsTest : AutoFixtureTestBase
        {
            protected IFavoriteType SetupFavorite(FavoriteContentType contentType, Guid? contentId = null)
            {
                contentId ??= Guid.NewGuid();

                var result = new FavoritesResponse.FavoriteType();

                switch(contentType)
                {
                    case FavoriteContentType.DataSource:
                        result.DataSource = new() { Id = contentId.Value };
                        break;
                    case FavoriteContentType.Flow:
                        result.Flow = new() { Id = contentId.Value };
                        break;
                    case FavoriteContentType.Project:
                        result.Project = new() { Id = contentId.Value };
                        break;
                    case FavoriteContentType.View:
                        result.View = new() { Id = contentId.Value };
                        break;
                    case FavoriteContentType.Workbook:
                        result.Workbook = new() { Id = contentId.Value };
                        break;
                    case FavoriteContentType.Collection:
                        result.Collection = new() { Id = contentId.Value };
                        break;
                    case FavoriteContentType.Unknown:
                        break;
                    default:
                        throw new ArgumentException("Invalid content type for test, add support to IFavoriteTypeExtensionsTest.SetupFavorite to test.");
                };

                return result;
            }
        }

        public sealed class GetContentType : IFavoriteTypeExtensionsTest
        {
            [Theory]
            [EnumData<FavoriteContentType>]
            public void GetsContentType(FavoriteContentType contentType)
            {
                var favorite = SetupFavorite(contentType);

                Assert.Equal(contentType, favorite.GetContentType());
            }
        }

        public sealed class GetContentId : IFavoriteTypeExtensionsTest
        {
            [Theory]
            [EnumData<FavoriteContentType>]
            public void GetsContentId(FavoriteContentType contentType)
            {
                var id = Guid.NewGuid();
                var favorite = SetupFavorite(contentType, id);

                if (contentType is FavoriteContentType.Unknown)
                {
                    Assert.Equal(Guid.Empty, favorite.GetContentId());
                }
                else
                {
                    Assert.Equal(id, favorite.GetContentId());
                }
            }
        }
    }
}
