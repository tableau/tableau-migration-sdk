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

using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public class RetrieveKeychainRequestTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var request = AutoFixture.Create<RetrieveKeychainRequest>();

                Assert.NotNull(request);

                var serialized = Serializer.SerializeToXml(request);

                Assert.NotNull(serialized);
                var expected = $@"
<tsRequest>
    <destinationSiteUrlNamespace>{request.DestinationSiteUrlNamespace}</destinationSiteUrlNamespace>
    <destinationSiteLuid>{request.DestinationSiteLuid}</destinationSiteLuid>
    <destinationServerUrl>{request.DestinationServerUrl}</destinationServerUrl>
</tsRequest>
";

                AssertXmlEqual(expected, serialized);
            }
        }
    }
}
