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

using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Models
{
    public class SignInRequestTests
    {
        public class SignInRequestTest : AutoFixtureTestBase
        { }

        public class Ctor : SignInRequestTest
        {
            [Fact]
            public void Sets_values()
            {
                var tokenName = Create<string>();
                var token = Create<string>();
                var contentUrl = Create<string>();

                var request = new SignInRequest(tokenName, token, contentUrl);

                Assert.NotNull(request.Credentials?.Site);

                Assert.Equal(tokenName, request.Credentials.PersonalAccessTokenName);
                Assert.Equal(token, request.Credentials.PersonalAccessTokenSecret);
                Assert.Equal(contentUrl, request.Credentials.Site.ContentUrl);
            }
        }
    }
}
