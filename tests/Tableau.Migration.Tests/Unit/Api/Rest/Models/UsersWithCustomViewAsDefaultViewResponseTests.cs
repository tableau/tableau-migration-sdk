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

using System.Collections.Immutable;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class UsersWithCustomViewAsDefaultViewResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedUsers = AutoFixture.Build<UsersWithCustomViewAsDefaultViewResponse.UserType>()
                    .CreateMany(2)
                    .ToImmutableList();

                var user1 = expectedUsers[0];
                var user2 = expectedUsers[1];

                var xml = $@"<?xml version='1.0' encoding='UTF-8'?>
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_22.xsd"">
    <pagination pageNumber=""1"" pageSize=""100"" totalAvailable=""{expectedUsers.Count}""/>
    <users>
        <user id=""{user1.Id}""/>
        <user id=""{user2.Id}""/>
    </users>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<UsersWithCustomViewAsDefaultViewResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);
                Assert.NotEmpty(deserialized.Items);

                Assert.Equal(expectedUsers.Count, deserialized.Items.Length);

                for (var i = 0; i != deserialized.Items.Length; i++)
                {
                    AssertUser(expectedUsers[i], deserialized.Items[i]);
                }

                static void AssertUser(UsersWithCustomViewAsDefaultViewResponse.UserType expected, UsersWithCustomViewAsDefaultViewResponse.UserType actual)
                {
                    Assert.Equal(expected.Id, actual.Id);
                }
            }
        }
    }
}
