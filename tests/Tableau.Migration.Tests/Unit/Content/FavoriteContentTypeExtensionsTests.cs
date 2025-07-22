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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Content
{
    public class FavoriteContentTypeExtensionsTests
    {
        public class ToMigrationContentType
        {
            [Theory]
            [InlineData(FavoriteContentType.DataSource, typeof(IDataSource))]
            [InlineData(FavoriteContentType.Flow, typeof(IFlow))]
            [InlineData(FavoriteContentType.Project, typeof(IProject))]
            [InlineData(FavoriteContentType.View, typeof(IView))]
            [InlineData(FavoriteContentType.Workbook, typeof(IWorkbook))]
            public void ReturnsCorrectType(FavoriteContentType contentType, Type expectedType)
            {
                Assert.Equal(expectedType, contentType.ToMigrationContentType());
            }

            [Fact]
            public void ThrowsOnUnsupportedType()
            {
                Assert.Throws<ArgumentException>(() => FavoriteContentType.Unknown.ToMigrationContentType());
            }
        }
        public class IsMigrationSupported
        {
            [Theory]
            [InlineData(FavoriteContentType.Unknown, false)]
            [InlineData(FavoriteContentType.Project, true)]
            [InlineData(FavoriteContentType.Workbook, true)]
            [InlineData(FavoriteContentType.View, true)]
            [InlineData(FavoriteContentType.DataSource, true)]
            [InlineData(FavoriteContentType.Flow, false)]
            [InlineData(FavoriteContentType.Collection, false)]
            public void ReturnsSupportStatus(FavoriteContentType contentType, bool expectedResult)
            {
                var result = contentType.IsMigrationSupported();
                Assert.Equal(result, expectedResult);
            }
        }
    }
}