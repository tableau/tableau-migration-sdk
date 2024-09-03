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

using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit
{
    public class NameOfTests
    {
        public class Build
        {
            public class FromObjectExpression
            {
                [Fact]
                public void Builds()
                {
                    var response = new SignInResponse();

                    var actual = NameOf.Build(() => response);

                    var expected = "response";

                    Assert.Equal(expected, actual);
                }

                [Fact]
                public void BuildsChain()
                {
                    var response = new SignInResponse();

                    var actual = NameOf.Build(() => response.Item!.Site!.ContentUrl);

                    var expected = "response.Item.Site.ContentUrl";

                    Assert.Equal(expected, actual);
                }

                [Fact]
                public void BuildsChainForValueType()
                {
                    var response = new SignInResponse();

                    var actual = NameOf.Build(() => response.Item!.Site!.Id);

                    var expected = "response.Item.Site.Id";

                    Assert.Equal(expected, actual);
                }

                [Fact]
                public void DoesNotThrowOnNonMemberExpression()
                {
                    Assert.Equal(string.Empty, NameOf.Build(() => 1 + 1));
                }
            }

            public class FromPropertyExpression
            {
                [Fact]
                public void BuildsChain()
                {
                    var response = new SignInResponse();

                    var actual = NameOf.Build<SignInResponse>(r => r.Item!.Site!.ContentUrl);

                    var expected = "Item.Site.ContentUrl";

                    Assert.Equal(expected, actual);
                }

                [Fact]
                public void BuildsChainForValueType()
                {
                    var response = new SignInResponse();

                    var actual = NameOf.Build<SignInResponse>(r => r.Item!.Site!.Id);

                    var expected = "Item.Site.Id";

                    Assert.Equal(expected, actual);
                }

                [Fact]
                public void DoesNotThrowOnNonMemberExpression()
                {
                    Assert.Equal(string.Empty, NameOf.Build<SignInResponse>(r => 1 + 1));
                }
            }
        }
    }
}
