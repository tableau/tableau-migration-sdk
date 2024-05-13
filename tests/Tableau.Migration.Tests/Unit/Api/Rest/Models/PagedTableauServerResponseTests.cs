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

using Tableau.Migration.Api.Rest.Models;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class PagedTableauServerResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes_error()
            {
                var expectedError = Create<Error>();

                var xml = $@"
<tsResponse>
    <error code=""{expectedError.Code}"">
        <summary>{expectedError.Summary}</summary>
        <detail>{expectedError.Detail}</detail>
    </error>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestPagedTableauServerResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.NotNull(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);

                Assert.Equal(expectedError.Code, deserialized.Error.Code);
                Assert.Equal(expectedError.Summary, deserialized.Error.Summary);
                Assert.Equal(expectedError.Detail, deserialized.Error.Detail);
            }

            [Fact]
            public void Deserializes_pagination()
            {
                var expectedPagination = Create<Pagination>();

                var xml = $@"
<tsResponse>
    <pagination pageNumber=""{expectedPagination.PageNumber}"" pageSize=""{expectedPagination.PageSize}"" totalAvailable=""{expectedPagination.TotalAvailable}""/>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestPagedTableauServerResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);

                Assert.Equal(expectedPagination.PageNumber, deserialized.Pagination.PageNumber);
                Assert.Equal(expectedPagination.PageSize, deserialized.Pagination.PageSize);
                Assert.Equal(expectedPagination.TotalAvailable, deserialized.Pagination.TotalAvailable);
            }
        }
    }
}