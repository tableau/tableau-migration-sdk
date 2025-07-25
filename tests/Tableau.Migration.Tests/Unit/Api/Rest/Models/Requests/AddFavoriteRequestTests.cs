﻿//
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
using Tableau.Migration.Api.Rest;
using Tableau.Migration.Api.Rest.Models.Requests;
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public sealed class AddFavoriteRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void DefaultCtor()
            {
                var r = new AddFavoriteRequest();
            }

            [Theory]
            [EnumData<FavoriteContentType>]
            public void CreateByContentType(FavoriteContentType contentType)
            {
                var label = Create<string>();
                var id = Guid.NewGuid();
                
                if (contentType is FavoriteContentType.Unknown)
                {
                    Assert.Throws<ArgumentException>(() => new AddFavoriteRequest(label, contentType, id));
                }
                else
                {
                    var r = new AddFavoriteRequest(label, contentType, id);

                    Assert.NotNull(r.Favorite);
                    Assert.Equal(label, r.Favorite.Label);

                    void AssertContentElement(IRestIdentifiable? contentElement)
                    {
                        Assert.NotNull(contentElement);
                        Assert.Equal(id, contentElement.Id);
                    }

                    switch (contentType)
                    {
                        case FavoriteContentType.DataSource:
                            AssertContentElement(r.Favorite.DataSource);
                            break;
                        case FavoriteContentType.Flow:
                            AssertContentElement(r.Favorite.Flow);
                            break;
                        case FavoriteContentType.Project:
                            AssertContentElement(r.Favorite.Project);
                            break;
                        case FavoriteContentType.View:
                            AssertContentElement(r.Favorite.View);
                            break;
                        case FavoriteContentType.Workbook:
                            AssertContentElement(r.Favorite.Workbook);
                            break;
                        case FavoriteContentType.Collection:
                            AssertContentElement(r.Favorite.Collection);
                            break;
                        default:
                            throw new ArgumentException("Cannot test AddFavoriteRequest with unsupported content type. Add assertion to AddFavoriteRequestTests.Ctor.CreateByContentType to test.");
                    }
                }
            }
        }
    }
}
