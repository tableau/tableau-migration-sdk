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

using System;
using System.Collections.Generic;
using System.Linq;
using Tableau.Migration.Api.Rest.Models.Requests;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Requests
{
    public sealed class UploadUserSavedCredentialsRequestTests
    {
        public sealed class Ctor : AutoFixtureTestBase
        {
            [Fact]
            public void Initializes()
            {
                var encryptedKeychains = Create<IEnumerable<string>>();

                var request = new UploadUserSavedCredentialsRequest(encryptedKeychains);

                Assert.Equal(encryptedKeychains, request.EncryptedKeychains);
            }
        }

        public sealed class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes()
            {
                var encryptedKeychains = Create<IEnumerable<string>>();

                var request = new UploadUserSavedCredentialsRequest(encryptedKeychains);

                Assert.NotNull(request);

                var serialized = Serializer.SerializeToXml(request);

                Assert.NotNull(serialized);
                var expected = $@"
<tsRequest>
    <encryptedKeychainList>
        {string.Join("", (request.EncryptedKeychains ?? Array.Empty<string>()).Select(k => $"<encryptedKeychain>{k}</encryptedKeychain>"))}
    </encryptedKeychainList>
</tsRequest>
";

                AssertXmlEqual(expected, serialized);
            }
        }
    }
}