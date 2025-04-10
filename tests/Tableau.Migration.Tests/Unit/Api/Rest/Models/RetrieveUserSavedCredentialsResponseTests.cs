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

using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class RetrieveUserSavedCredentialsResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expected = Create<RetrieveKeychainResponse>();

                Assert.NotNull(expected.EncryptedKeychainList);
                Assert.NotNull(expected.AssociatedUserLuidList);

                var xml = $@"
<tsResponse xmlns=""http://tableau.com/api""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xsi:schemaLocation=""http://tableau.com/api
	http://tableau.com/api/ts-api-3.4.xsd"">
		<encryptedKeychainList>
            <encryptedKeychain>{expected.EncryptedKeychainList[0]}</encryptedKeychain>
            <encryptedKeychain>{expected.EncryptedKeychainList[1]}</encryptedKeychain>
            <encryptedKeychain>{expected.EncryptedKeychainList[2]}</encryptedKeychain>
		</encryptedKeychainList>
        <associatedUserLuidList>
            <associatedUserLuid>{expected.AssociatedUserLuidList[0]}</associatedUserLuid>
            <associatedUserLuid>{expected.AssociatedUserLuidList[1]}</associatedUserLuid>
            <associatedUserLuid>{expected.AssociatedUserLuidList[2]}</associatedUserLuid>
		</associatedUserLuidList>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<RetrieveKeychainResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                
                Assert.Equal(expected.EncryptedKeychainList, deserialized.EncryptedKeychainList);
                Assert.Equal(expected.AssociatedUserLuidList, deserialized.AssociatedUserLuidList);
            }
        }
    }
}
