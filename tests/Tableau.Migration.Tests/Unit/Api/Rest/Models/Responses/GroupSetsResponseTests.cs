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

using System.Collections.Immutable;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class GroupSetsResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedGroupSets = AutoFixture.Build<GroupSetsResponse.GroupSetType>()
                    .CreateMany(4)
                    .ToImmutableList();

                var gs1 = expectedGroupSets[0];
                var gs2 = expectedGroupSets[1];
                var gs3 = expectedGroupSets[2];
                var gs4 = expectedGroupSets[3];

                var xml = $@"<?xml version='1.0' encoding='UTF-8'?>
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_25.xsd"">
    <pagination pageNumber=""1"" pageSize=""4"" totalAvailable=""4""/>
    <groupSets>
        <groupSet id=""{gs1.Id}"" name=""{gs1.Name}"" groupCount=""{gs1.GroupCount}""/>
        <groupSet id=""{gs2.Id}"" name=""{gs2.Name}"" groupCount=""{gs2.GroupCount}""/>
        <groupSet id=""{gs3.Id}"" name=""{gs3.Name}"" groupCount=""{gs3.GroupCount}""/>
        <groupSet id=""{gs4.Id}"" name=""{gs4.Name}"" groupCount=""{gs4.GroupCount}""/>
    </groupSets>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<GroupSetsResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);
                Assert.NotEmpty(deserialized.Items);
                Assert.Equal(expectedGroupSets.Count, deserialized.Items.Length);

                for (var i = 0; i != deserialized.Items.Length; i++)
                {
                    AssertGroupSet(expectedGroupSets[i], deserialized.Items[i]);
                }

                static void AssertGroupSet(GroupSetsResponse.GroupSetType expected, GroupSetsResponse.GroupSetType actual)
                {
                    Assert.Equal(expected.Id, actual.Id);
                    Assert.Equal(expected.Name, actual.Name);
                    Assert.Equal(expected.GroupCount, actual.GroupCount);
                }
            }
        }
    }
}
