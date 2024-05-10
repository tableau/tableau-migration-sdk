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

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class ServerInfoResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expected = Create<ServerInfoResponse>();

                Assert.NotNull(expected.Item);
                Assert.NotNull(expected.Item.RestApiVersion);
                Assert.NotNull(expected.Item.ProductVersion);

                var xml = $@"
<tsResponse xmlns=""http://tableau.com/api""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xsi:schemaLocation=""http://tableau.com/api
	http://tableau.com/api/ts-api-3.4.xsd"">
		<serverInfo>
			<productVersion build=""{expected.Item.ProductVersion.Build}"">{expected.Item.ProductVersion.Version}</productVersion>
            <restApiVersion>{expected.Item.RestApiVersion.Version}</restApiVersion>
		</serverInfo>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<ServerInfoResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Item);
                Assert.NotNull(deserialized.Item.RestApiVersion);
                Assert.NotNull(deserialized.Item.ProductVersion);

                Assert.Equal(expected.Item.RestApiVersion.Version, deserialized.Item.RestApiVersion.Version);
                Assert.Equal(expected.Item.ProductVersion.Version, deserialized.Item.ProductVersion.Version);
                Assert.Equal(expected.Item.ProductVersion.Build, deserialized.Item.ProductVersion.Build);
            }
        }
    }
}
