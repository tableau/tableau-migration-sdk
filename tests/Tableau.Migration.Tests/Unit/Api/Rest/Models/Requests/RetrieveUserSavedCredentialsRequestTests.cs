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

using System.Linq;
using Tableau.Migration.Api.Models;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public sealed class RetrieveUserSavedCredentialsRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IDestinationSiteInfo>();

                var request = new RetrieveUserSavedCredentialsRequest(options);

                Assert.Equal(options.ContentUrl, request.DestinationSiteUrlNamespace);
                Assert.Equal(options.SiteId, request.DestinationSiteLuid);
                Assert.Equal(options.SiteUrl, request.DestinationServerUrl);
            }
        }

        public sealed class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var options = Create<IDestinationSiteInfo>();
                var request = new RetrieveUserSavedCredentialsRequest(options);

                var serialized = Serializer.SerializeToXml(request);

                var expected = $@"
<tsRequest>
  <destinationSiteUrlNamespace>{options.ContentUrl}</destinationSiteUrlNamespace>
  <destinationSiteLuid>{options.SiteId}</destinationSiteLuid>
  <destinationServerUrl>{options.SiteUrl}</destinationServerUrl>
</tsRequest>";
                AssertXmlEqual(expected, serialized);

            }
        }
    }
}