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

using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class SignInRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var request = AutoFixture.Create<SignInRequest>();

                Assert.NotNull(request.Credentials);
                Assert.NotNull(request.Credentials.Site);

                var serialized = Serializer.SerializeToXml(request);

                Assert.NotNull(serialized);

                var expected = $@"
<tsRequest>
    <credentials personalAccessTokenName=""{request.Credentials.PersonalAccessTokenName}"" personalAccessTokenSecret=""{request.Credentials.PersonalAccessTokenSecret}"">
	    <site contentUrl=""{request.Credentials.Site.ContentUrl}"" />
    </credentials>
</tsRequest>";

                AssertXmlEqual(expected, serialized);
            }
        }
    }
}