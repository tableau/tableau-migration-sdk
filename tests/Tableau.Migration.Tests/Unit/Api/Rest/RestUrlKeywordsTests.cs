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

using System.Collections.Generic;
using Tableau.Migration.Api.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest
{
    public class RestUrlKeywordsTests
    {
        private static HashSet<string> GetAll() => TypeExtensions.GetAllPublicStringValues(typeof(RestUrlKeywords));

        [Fact]
        public void No_null_or_empty_values()
        {
            Assert.All(GetAll(), v => Assert.False(string.IsNullOrEmpty(v)));
        }

        [Fact]
        public void All_values_are_valid_url_segments()
        {
            // URL segments should not contain spaces, special characters
            Assert.All(GetAll(), v =>
            {
                Assert.Matches("^[a-zA-Z0-9-]+$", v);
                Assert.DoesNotContain(" ", v);
                Assert.DoesNotContain("_", v);
            });
        }
    }
}