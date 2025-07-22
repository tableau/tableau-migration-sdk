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
using System.Linq;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class FavoriteTests : AutoFixtureTestBase
    {
        public class Ctor() : FavoriteTests
        {

            [Theory]
            [EnumData<FavoriteContentType>()]
            public void Success(FavoriteContentType contentType)
            {
                // Arrange
                var user = Create<IContentReference>();
                var label = Create<string>();
                var content = Create<IContentReference>();

                // Act
                var result = new Favorite(user, contentType, content, label);

                // Assert
                Assert.Equal(label, result.Label);
                Assert.Equal(Guid.Empty, result.Id);
                Assert.Empty(result.ContentUrl);
            }

            [Theory]
            [EnumData<FavoriteContentType>()]
            public void Id_empty(FavoriteContentType contentType)
            {
                // Arrange
                var user = Create<IContentReference>();
                var label = Create<string>();
                var content = Create<IContentReference>();

                // Act
                var result = new Favorite(user, contentType, content, label);

                // Assert               
                Assert.Equal(Guid.Empty, result.Id);

            }

            [Theory]
            [EnumData<FavoriteContentType>()]
            public void ContentUrl_empty(FavoriteContentType contentType)
            {
                // Arrange
                var user = Create<IContentReference>();
                var label = Create<string>();
                var content = Create<IContentReference>();

                // Act
                var result = new Favorite(user, contentType, content, label);

                // Assert               
                Assert.Equal(string.Empty, result.ContentUrl);

            }

            [Theory]
            [EnumData<FavoriteContentType>()]
            public void Location_correct(FavoriteContentType contentType)
            {
                // Arrange
                var user = Create<IContentReference>();
                var label = Create<string>();
                var content = Create<IContentReference>();
                var location = Favorite.BuildLocation(user, contentType, content);

                // Act
                var result = new Favorite(user, contentType, content, label);

                // Assert               
                Assert.Equal(location, result.Location);
            }

            [Theory]
            [EnumData<FavoriteContentType>()]
            public void Name_equals_label(FavoriteContentType contentType)
            {
                // Arrange
                var user = Create<IContentReference>();
                var label = Create<string>();
                var content = Create<IContentReference>();

                // Act
                var result = new Favorite(user, contentType, content, label);

                // Assert               
                Assert.Equal(label, result.Name);
            }


            [Theory]
            [EnumData<FavoriteContentType>()]
            public void User_correct(FavoriteContentType contentType)
            {
                // Arrange
                var user = Create<IContentReference>();
                var label = Create<string>();
                var content = Create<IContentReference>();

                // Act
                var result = new Favorite(user, contentType, content, label);

                // Assert               
                Assert.Equal(user, result.User);
            }

        }

        public class BuildLocation : FavoriteTests
        {
            [Theory]
            [EnumData<FavoriteContentType>()]
            public void Success(FavoriteContentType contentType)
            {
                //Arrange
                var user = Create<IContentReference>();
                var content = Create<IContentReference>();

                // Act
                var result = Favorite.BuildLocation(user, contentType, content);

                // Assert
                var expectedSegments = user.Location.PathSegments
                    .Concat(["favorites", contentType.ToString()])
                    .Concat(content.Location.PathSegments);

                Assert.Equal(expectedSegments, result.PathSegments);
            }
        }

        public class EqualsTests : FavoriteTests
        {
            [Fact]
            public void Success()
            {
                // Arrange
                var fav = Create<Favorite>();

                // Assert               
                Assert.Equal(fav.Location.ToString(), fav.Location.ToString());
                Assert.True(fav.Equals(fav));
            }

            [Fact]
            public void False_on_different_location()
            {
                // Arrange
                var fav1 = Create<Favorite>();
                var fav2 = Create<Favorite>();


                // Assert               
                Assert.NotEqual(fav1.Location.ToString(), fav2.Location.ToString());
                Assert.False(fav1.Equals(fav2));
            }

            [Fact]
            public void False_on_null_location()
            {
                // Arrange
                var fav = Create<Favorite>();

                // Assert                               
                Assert.False(fav.Equals(null));
            }
        }
    }
}