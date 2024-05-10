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
    public class SiteResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expected = Create<SiteResponse>();

                Assert.NotNull(expected.Item);

                var xml = $@"
<tsResponse>
    <site id=""{expected.Item.Id}"" name=""{expected.Item.Name}"" contentUrl=""{expected.Item.ContentUrl}"" />
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<SiteResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.NotNull(deserialized.Item);

                Assert.Equal(expected.Item.Id, deserialized.Item.Id);
                Assert.Equal(expected.Item.Name, deserialized.Item.Name);
                Assert.Equal(expected.Item.ContentUrl, deserialized.Item.ContentUrl);
            }
        }
    }
}