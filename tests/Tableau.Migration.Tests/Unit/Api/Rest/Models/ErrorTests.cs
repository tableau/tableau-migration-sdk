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
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class ErrorTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedError = Create<Error>();

                var xml = $@"
<tsResponse>
    <error code=""{expectedError.Code}"">
        <summary>{expectedError.Summary}</summary>
        <detail>{expectedError.Detail}</detail>
    </error>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<TestTableauServerResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.NotNull(deserialized.Error);

                Assert.Equal(expectedError.Code, deserialized.Error.Code);
                Assert.Equal(expectedError.Summary, deserialized.Error.Summary);
                Assert.Equal(expectedError.Detail, deserialized.Error.Detail);
            }
        }

        public class Equals_Override
        {
            [Theory]
            [InlineData("code", "summary", "detail")]
            public void True(string code, string summary, string detail)
            {
                var error1 = new Error { Code = code, Summary = summary, Detail = detail };
                var error2 = new Error { Code = code, Summary = summary, Detail = detail };

                Assert.Equal(error1, error2);
            }

            [Theory]
            [InlineData("code", "summary", "detail", "CODE", "SUMMARY", "DETAIL")]
            [InlineData("code", "summary", "detail", "code-different", "summary", "detail")]
            [InlineData("code", "summary", "detail", "code", "summary-different", "detail")]
            [InlineData("code", "summary", "detail", "code", "summary", "detail-different")]
            public void False(string code1, string summary1, string detail1, string code2, string summary2, string detail2)
            {
                var error1 = new Error { Code = code1, Summary = summary1, Detail = detail1 };
                var error2 = new Error { Code = code2, Summary = summary2, Detail = detail2 };

                Assert.NotEqual(error1, error2);
            }
        }

        public class GetHashCode_Override
        {
            [Theory]
            [InlineData("code", "summary", "detail")]
            public void Uses_values(string code, string summary, string detail)
            {
                var hashCode = HashCode.Combine(code, summary, detail);

                var error = new Error { Code = code, Summary = summary, Detail = detail };

                Assert.Equal(hashCode, error.GetHashCode());
            }

            [Theory]
            [InlineData("code", "summary", "detail")]
            public void Case_sensitive(string code, string summary, string detail)
            {
                var error1 = new Error { Code = code, Summary = summary, Detail = detail };
                var error2 = new Error { Code = code.ToUpper(), Summary = summary.ToUpper(), Detail = detail.ToUpper() };

                Assert.NotEqual(error1.GetHashCode(), error2.GetHashCode());
            }
        }
    }
}
