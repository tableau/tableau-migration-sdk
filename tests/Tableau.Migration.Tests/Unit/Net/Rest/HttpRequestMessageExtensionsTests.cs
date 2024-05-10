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

using System.Net.Http;
using Tableau.Migration.Net.Rest;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Net.Rest
{
    public class HttpRequestMessageExtensionsTests
    {
        public abstract class HttpRequestMessageExtensionsTest : AutoFixtureTestBase
        { }

        public class SetRestAuthenticationToken : HttpRequestMessageExtensionsTest
        {
            [Fact]
            public void Overwrites_existing_token()
            {
                var oldToken = Create<string>();
                var newToken = Create<string>();

                var request = new HttpRequestMessage(HttpMethod.Get, TestConstants.LocalhostUri);

                request.Headers.TryAddWithoutValidation(RestHeaders.AuthenticationToken, oldToken);

                request.AssertHeaderExists(RestHeaders.AuthenticationToken);

                request.SetRestAuthenticationToken(newToken);

                request.AssertSingleHeaderValue(RestHeaders.AuthenticationToken, newToken);
            }

            [Fact]
            public void Sets_token()
            {
                var token = Create<string>();

                var request = new HttpRequestMessage(HttpMethod.Get, TestConstants.LocalhostUri);

                request.SetRestAuthenticationToken(token);

                request.AssertSingleHeaderValue(RestHeaders.AuthenticationToken, token);
            }
        }
    }
}
