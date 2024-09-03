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
    public class CustomViewsResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedCustomViews = AutoFixture.Build<CustomViewsResponse.CustomViewResponseType>()
                    .CreateMany(2)
                    .ToImmutableList();

                var cv1 = expectedCustomViews[0];
                var cv2 = expectedCustomViews[1];

                var xml = $@"<?xml version='1.0' encoding='UTF-8'?>
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_22.xsd"">
    <pagination pageNumber=""1"" pageSize=""100"" totalAvailable=""{expectedCustomViews.Count}""/>
    <customViews>
        <customView id=""{cv1.Id}"" name=""{cv1.Name}"" createdAt=""{cv1.CreatedAt}"" updatedAt=""{cv1.UpdatedAt}"" lastAccessedAt=""{cv1.LastAccessedAt}"" shared=""{cv1.Shared.ToString().ToLower()}"">
            <view id=""{cv1.View!.Id}"" name=""{cv1.View.Name}""/>
            <workbook id=""{cv1.Workbook!.Id}"" name=""{cv1.Workbook.Name}""/>
            <owner id=""{cv1.Owner!.Id}"" name=""{cv1.Owner.Name}""/>
        </customView>
        <customView id=""{cv2.Id}"" name=""{cv2.Name}"" createdAt=""{cv2.CreatedAt}"" updatedAt=""{cv2.UpdatedAt}"" lastAccessedAt=""{cv2.LastAccessedAt}"" shared=""{cv2.Shared.ToString().ToLower()}"">
            <view id=""{cv2.View!.Id}"" name=""{cv2.View.Name}""/>
            <workbook id=""{cv2.Workbook!.Id}"" name=""{cv2.Workbook.Name}""/>
            <owner id=""{cv2.Owner!.Id}"" name=""{cv2.Owner.Name}""/>
        </customView>
    </customViews>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<CustomViewsResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);
                Assert.NotEmpty(deserialized.Items);

                Assert.Equal(expectedCustomViews.Count, deserialized.Items.Length);

                for (var i = 0; i != deserialized.Items.Length; i++)
                {
                    AssertCustomView(expectedCustomViews[i], deserialized.Items[i]);
                }

                static void AssertCustomView(CustomViewsResponse.CustomViewResponseType expected, CustomViewsResponse.CustomViewResponseType actual)
                {
                    Assert.Equal(expected.Id, actual.Id);
                    Assert.Equal(expected.Name, actual.Name);
                    Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                    Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
                    Assert.Equal(expected.LastAccessedAt, actual.LastAccessedAt);
                    Assert.Equal(expected.Shared, actual.Shared);

                    Assert.NotNull(actual.View);
                    Assert.NotNull(expected.View);
                    Assert.Equal(expected.View.Id, actual.View.Id);
                    Assert.Equal(expected.View.Name, actual.View.Name);

                    Assert.NotNull(actual.Workbook);
                    Assert.NotNull(expected.Workbook);
                    Assert.Equal(expected.Workbook.Id, actual.Workbook.Id);
                    Assert.Equal(expected.Workbook.Name, actual.Workbook.Name);

                    Assert.NotNull(actual.Owner);
                    Assert.NotNull(expected.Owner);
                    Assert.Equal(expected.Owner.Id, actual.Owner.Id);
                    Assert.Equal(expected.Owner.Name, actual.Owner.Name);
                }
            }
        }
    }
}
