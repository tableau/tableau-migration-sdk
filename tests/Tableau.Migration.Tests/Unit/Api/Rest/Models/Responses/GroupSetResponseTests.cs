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
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class GroupSetResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedGroupSet = AutoFixture.Build<GroupSetResponse.GroupSetType>()
                    .Create();

                var expectedGroups = AutoFixture.Build<GroupSetResponse.GroupSetType.GroupType>()
                    .CreateMany(3)
                    .ToArray();

                expectedGroupSet.Groups = expectedGroups;
                expectedGroupSet.GroupCount = expectedGroups.Length;

                var xml = $@"<?xml version='1.0' encoding='UTF-8'?>
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_25.xsd"">
    <groupSet id=""{expectedGroupSet.Id}"" name=""{expectedGroupSet.Name}"" groupCount=""{expectedGroupSet.GroupCount}"">
        <group id=""{expectedGroups[0].Id}"" name=""{expectedGroups[0].Name}"" />
        <group id=""{expectedGroups[1].Id}"" name=""{expectedGroups[1].Name}"" />
        <group id=""{expectedGroups[2].Id}"" name=""{expectedGroups[2].Name}"" />
    </groupSet>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<GroupSetResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Item);

                // Assert GroupSet properties
                Assert.Equal(expectedGroupSet.Id, deserialized.Item.Id);
                Assert.Equal(expectedGroupSet.Name, deserialized.Item.Name);
                Assert.Equal(expectedGroupSet.GroupCount, deserialized.Item.GroupCount);

                // Assert Groups
                Assert.NotNull(deserialized.Item.Groups);
                Assert.Equal(expectedGroups.Length, deserialized.Item.Groups.Length);

                for (var i = 0; i < expectedGroups.Length; i++)
                {
                    Assert.Equal(expectedGroups[i].Id, deserialized.Item.Groups[i].Id);
                    Assert.Equal(expectedGroups[i].Name, deserialized.Item.Groups[i].Name);
                }
            }
        }
    }
}