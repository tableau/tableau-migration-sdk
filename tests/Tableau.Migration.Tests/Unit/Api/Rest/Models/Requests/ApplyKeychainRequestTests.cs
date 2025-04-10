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
    public sealed class ApplyKeychainRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var options = Create<IApplyKeychainOptions>();

                var request = new ApplyKeychainRequest(options);

                Assert.Equal(options.EncryptedKeychains, request.EncryptedKeychains);

                Assert.NotNull(request.AssociatedUserLuidMapping);
                Assert.Equal(options.KeychainUserMapping.Count(), request.AssociatedUserLuidMapping.Count());
                foreach (var mapping in options.KeychainUserMapping)
                {
                    Assert.Contains(request.AssociatedUserLuidMapping, i => i.SourceSiteUserLuid == mapping.SourceUserId && i.DestinationSiteUserLuid == mapping.DestinationUserId);
                }
            }
        }

        public sealed class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var options = Create<IApplyKeychainOptions>();
                var request = new ApplyKeychainRequest(options);

                var serialized = Serializer.SerializeToXml(request);

                var expected = $@"
<tsRequest>
  <encryptedKeychainList>
    {string.Join("", options.EncryptedKeychains.Select(k => $"<encryptedKeychain>{k}</encryptedKeychain>"))}
  </encryptedKeychainList>
  <associatedUserLuidMapping>
    {string.Join("", options.KeychainUserMapping.Select(m => $@"<userLuidPair sourceSiteUserLuid=""{m.SourceUserId}"" destinationSiteUserLuid=""{m.DestinationUserId}"" />"))}
  </associatedUserLuidMapping>
</tsRequest>";
                AssertXmlEqual(expected, serialized);

            }

            [Fact]
            public void SerializesWithNoUserMapping()
            {
                var options = new ApplyKeychainOptions(CreateMany<string>().ToArray(), []);
                var request = new ApplyKeychainRequest(options);

                var serialized = Serializer.SerializeToXml(request);

                var expected = $@"
<tsRequest>
  <encryptedKeychainList>
    {string.Join("", options.EncryptedKeychains.Select(k => $"<encryptedKeychain>{k}</encryptedKeychain>"))}
  </encryptedKeychainList>
</tsRequest>";
                AssertXmlEqual(expected, serialized);

            }
        }
    }
}
