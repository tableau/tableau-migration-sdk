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
using Tableau.Migration.Content.Search;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content.Search
{
    public sealed class IContentReferenceFinderFactoryTests
    {
        public sealed class ForFavoriteContentType : AutoFixtureTestBase
        {
            [Theory]
            [EnumData<FavoriteContentType>]
            public void GetsFinderForContentType(FavoriteContentType contentType)
            {
                var mockFactory = new Mock<IContentReferenceFinderFactory>();
                mockFactory.CallBase = true;
                var factory = mockFactory.Object;

                switch (contentType)
                {
                    case FavoriteContentType.DataSource:
                        var dsFinder = factory.ForFavoriteContentType(contentType);
                        mockFactory.Verify(x => x.ForContentType<IDataSource>(), Times.Once);
                        break;

                    case FavoriteContentType.Flow:
                        var flowFinder = factory.ForFavoriteContentType(contentType);
                        mockFactory.Verify(x => x.ForContentType<IFlow>(), Times.Once);
                        break;

                    case FavoriteContentType.Project:
                        var projFinder = factory.ForFavoriteContentType(contentType);
                        mockFactory.Verify(x => x.ForContentType<IProject>(), Times.Once);
                        break;

                    case FavoriteContentType.Workbook:
                        var wbFinder = factory.ForFavoriteContentType(contentType);
                        mockFactory.Verify(x => x.ForContentType<IWorkbook>(), Times.Once);
                        break;

                    case FavoriteContentType.Collection:
                        var collFinder = factory.ForFavoriteContentType(contentType);
                        mockFactory.Verify(x => x.ForContentType<ITableauCollection>(), Times.Once);
                        break;

                    case FavoriteContentType.Unknown:
                    case FavoriteContentType.View:
                        Assert.Throws<NotSupportedException>(() => factory.ForFavoriteContentType(contentType));
                        break;
                    default:
                        throw new ArgumentException($"Content type {contentType} not supported by ForFavoriteContentType.GetsFinderForContentType. Add support to test.");
                }
            }
        }
    }
}