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
using Moq;
using Tableau.Migration.Content;
using Tableau.Migration.Engine.Endpoints.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Engine.Endpoints.Search
{
    public sealed class IDestinationContentReferenceFinderFactoryTests
    {
        public sealed class ForFavoriteDestinationContentType : AutoFixtureTestBase
        {
            [Theory]
            [EnumData<FavoriteContentType>]
            public void BuildsForContentType(FavoriteContentType favoriteContentType)
            {
                var mockFactory = new Mock<IDestinationContentReferenceFinderFactory>();
                mockFactory.CallBase = true;

                void TestFinderCreated<TContent>()
                    where TContent: class, IContentReference
                {
                    var result = mockFactory.Object.ForFavoriteDestinationContentType(favoriteContentType);
                    mockFactory.Verify(x => x.ForDestinationContentType<TContent>(), Times.Once);
                }

                switch (favoriteContentType)
                {
                    case FavoriteContentType.DataSource:
                        TestFinderCreated<IDataSource>();
                        break;
                    case FavoriteContentType.Flow:
                        TestFinderCreated<IFlow>();
                        break;
                    case FavoriteContentType.Project:
                        TestFinderCreated<IProject>();
                        break;
                    case FavoriteContentType.Workbook:
                        TestFinderCreated<IWorkbook>();
                        break;
                    case FavoriteContentType.View:
                    case FavoriteContentType.Unknown:
                    case FavoriteContentType.Collection:
                        Assert.Throws<NotSupportedException>(() => mockFactory.Object.ForFavoriteDestinationContentType(favoriteContentType));
                        break;
                    default:
                        throw new ArgumentException($"Favorite content type not supported for test. Add test to {nameof(BuildsForContentType)}.");
                }
            }
        }
    }
}
