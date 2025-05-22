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

using System.Text;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class RetrieveKeychainResponseTests
    {
        public class DeSerialization : SerializationTestBase
        {
            [Fact]
            public void DeSerializes()
            {
                var expectedResponse = Create<RetrieveKeychainResponse>();

                Assert.NotNull(expectedResponse.AssociatedUserLuidList);

                var userLuidElements = new StringBuilder();

                foreach (var userId in expectedResponse.AssociatedUserLuidList)
                {
                    userLuidElements.AppendLine($"<associatedUserLuid>{userId.ToString()}</associatedUserLuid>");
                }

                Assert.NotNull(expectedResponse.EncryptedKeychainList);

                var keyChainElements = new StringBuilder();

                foreach (var keychain in expectedResponse.EncryptedKeychainList)
                {
                    keyChainElements.AppendLine($"<encryptedKeychain>{keychain}</encryptedKeychain>");
                }

                var xml = $@"
<tsResponse>
    <associatedUserLuidList>
        {userLuidElements}
    </associatedUserLuidList>
    <encryptedKeychainList>
        {keyChainElements}
    </encryptedKeychainList>
</tsResponse>
";
                var deserialized = Serializer.DeserializeFromXml<RetrieveKeychainResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.AssociatedUserLuidList);
                Assert.Equal(expectedResponse.AssociatedUserLuidList.Length, deserialized.AssociatedUserLuidList.Length);
                Assert.All(
                    expectedResponse.AssociatedUserLuidList,
                    (responseUserLuid) => Assert.Contains(responseUserLuid, deserialized.AssociatedUserLuidList));

                Assert.NotNull(deserialized.EncryptedKeychainList);
                Assert.Equal(expectedResponse.EncryptedKeychainList.Length, deserialized.EncryptedKeychainList.Length);
                Assert.All(
                    expectedResponse.EncryptedKeychainList,
                    (responseKeychain) => Assert.Contains(responseKeychain, deserialized.EncryptedKeychainList));
            }

            [Fact]
            public void DeSerializes_with_empty_users()
            {
                var expectedResponse = AutoFixture
                    .Build<RetrieveKeychainResponse>()
                    .With(r => r.AssociatedUserLuidList, () => [])
                    .Create();

                Assert.NotNull(expectedResponse.AssociatedUserLuidList);
                Assert.Empty(expectedResponse.AssociatedUserLuidList);



                Assert.NotNull(expectedResponse.EncryptedKeychainList);

                var keyChainElements = new StringBuilder();

                foreach (var keychain in expectedResponse.EncryptedKeychainList)
                {
                    keyChainElements.AppendLine($"<encryptedKeychain>{keychain}</encryptedKeychain>");
                }

                var xml = $@"
<tsResponse>
    <associatedUserLuidList></associatedUserLuidList>
    <encryptedKeychainList>
        {keyChainElements}
    </encryptedKeychainList>
</tsResponse>
";
                var deserialized = Serializer.DeserializeFromXml<RetrieveKeychainResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.AssociatedUserLuidList);
                Assert.Empty(deserialized.AssociatedUserLuidList);

                Assert.NotNull(deserialized.EncryptedKeychainList);
                Assert.Equal(expectedResponse.EncryptedKeychainList.Length, deserialized.EncryptedKeychainList.Length);
                Assert.All(
                    expectedResponse.EncryptedKeychainList,
                    (responseKeychain) => Assert.Contains(responseKeychain, deserialized.EncryptedKeychainList));
            }
        }

    }
}
