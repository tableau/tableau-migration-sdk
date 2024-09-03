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

using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class CustomViewResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedCustomView = AutoFixture.Build<CustomViewResponse.CustomViewType>()
                    .Create();

                var xml = $@"<?xml version='1.0' encoding='UTF-8'?>
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_22.xsd"">
    <customView id=""{expectedCustomView.Id}"" name=""{expectedCustomView.Name}"" createdAt=""{expectedCustomView.CreatedAt}"" updatedAt=""{expectedCustomView.UpdatedAt}"" lastAccessedAt=""{expectedCustomView.LastAccessedAt}"" shared=""{expectedCustomView.Shared.ToString().ToLower()}"">
        <view id=""{expectedCustomView.View!.Id}"" name=""{expectedCustomView.View.Name}""/>
        <workbook id=""{expectedCustomView.Workbook!.Id}"" name=""{expectedCustomView.Workbook.Name}""/>
        <owner id=""{expectedCustomView.Owner!.Id}"" name=""{expectedCustomView.Owner.Name}""/>
    </customView>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<CustomViewResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Item);

                AssertCustomView(expectedCustomView, deserialized.Item);
                
                static void AssertCustomView(CustomViewResponse.CustomViewType expected, CustomViewResponse.CustomViewType actual)
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
