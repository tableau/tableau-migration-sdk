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

using System.Collections.Generic;
using Tableau.Migration.Net;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net
{
    public class QueryStringBuilderTests
    {
        public abstract class QueryStringBuilderTest : AutoFixtureTestBase
        {
            internal readonly QueryStringBuilder Builder = new();
        }

        public class AddOrUpdate : QueryStringBuilderTest
        {
            [Theory]
            [InlineData("aaa", "bbb", "aaa=bbb")]
            [InlineData("aaa bbb", "ccc", "aaa+bbb=ccc")]
            [InlineData("aaa & bbb", "ccc", "aaa+%26+bbb=ccc")]
            [InlineData("aaa & bbb", "ccc ddd", "aaa+%26+bbb=ccc+ddd")]
            [InlineData("aaa & bbb", "ccc & ddd", "aaa+%26+bbb=ccc+%26+ddd")]
            [InlineData("aaa\nbbb", "ccc\nddd", "aaa%0Abbb=ccc%0Addd")]
            [InlineData(" \t\r\n\0~!@#$%^&*()_+{}|:\"<>?`[]\\;',./=", " \t\r\n\0~!@#$%^&*()_+{}|:\"<>?`[]\\;',./=", "+%09%0D%0A%00%7E!%40%23%24%25%5E%26*()_%2B%7B%7D%7C%3A%22%3C%3E%3F%60%5B%5D%5C%3B%27%2C.%2F%3D=+%09%0D%0A%00%7E!%40%23%24%25%5E%26*()_%2B%7B%7D%7C%3A%22%3C%3E%3F%60%5B%5D%5C%3B%27%2C.%2F%3D")]
            public void Encodes(string key, string value, string expected)
            {
                // key, value overload
                {
                    Builder.AddOrUpdate(key, value);

                    var query = Builder.Build();

                    Assert.Equal(expected, query);
                }

                // IDictionary overload
                {
                    Builder.AddOrUpdate(new Dictionary<string, string>
                    {
                        [key] = value
                    });

                    var query = Builder.Build();

                    Assert.Equal(expected, query);
                }
            }
        }
    }
}
