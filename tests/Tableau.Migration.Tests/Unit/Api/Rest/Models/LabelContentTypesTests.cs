//
//  Copyright (c) 2024, Salesforce, Inc.
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
using Tableau.Migration.Content;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class LabelContentTypesTests
    {
        public class FromContentType
        {
            [Theory]
            [InlineData(typeof(IDataSource), LabelContentTypes.DataSource)]
            [InlineData(typeof(IPublishableDataSource), LabelContentTypes.DataSource)]
            public void Parses(Type inputContentType, string expectedResult)
                => Assert.Equal(expectedResult, LabelContentTypes.FromContentType(inputContentType));

            [Theory]
            [InlineData(typeof(IProject))]
            public void Throws_exception_when_unsupported(Type inputContentType)
                => Assert.Throws<NotSupportedException>(() => LabelContentTypes.FromContentType(inputContentType));
        }
    }
}
