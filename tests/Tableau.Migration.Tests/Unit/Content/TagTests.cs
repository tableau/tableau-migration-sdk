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

namespace Tableau.Migration.Tests.Unit.Content
{
    public class TagTests
    {
        public class FromTagType : AutoFixtureTestBase
        {
            protected ITagType CreateTestResponse()
            {
                return Create<ITagType>();
            }

            private static void AssertSuccess(ITagType? response, ITag result)
            {
                if (response == null)
                {
                    return;
                }

                Assert.NotNull(result);
                Assert.Equal(response.Label, result.Label);
            }

            [Fact]
            public void Success()
            {
                var response = CreateTestResponse();

                var result = new Tag(response);
                FromTagType.AssertSuccess(response, result);
            }


            [Fact]
            public void InvalidName()
            {
                var response = CreateTestResponse();
                response.Label = null;

                Assert.Throws<ArgumentException>(() => new Tag(response));
            }
        }
    }
}

