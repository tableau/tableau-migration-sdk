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

using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class WorkbookResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var (TestXML, ExpectedResult) = GetTestData();
                var deserialized = Serializer.DeserializeFromXml<WorkbookResponse>(TestXML);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var workbook = deserialized.Item;
                Assert.NotNull(workbook);

                AssertWorkbook(ExpectedResult, workbook);
                AssertViews(ExpectedResult.Views.ToList(), workbook.Views.ToList());
                AssertProject(ExpectedResult.Project, workbook.Project);
                AssertOwner(ExpectedResult.Owner, workbook.Owner);
                AssertTags(ExpectedResult.Tags!, workbook.Tags!);
            }

            private static void AssertWorkbook(
                WorkbookResponse.WorkbookType? expected,
                WorkbookResponse.WorkbookType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Description, actual.Description);
                Assert.Equal(expected.ContentUrl, actual.ContentUrl);
                Assert.Equal(expected.WebpageUrl, actual.WebpageUrl);
                Assert.Equal(expected.ShowTabs, actual.ShowTabs);
                Assert.Equal(expected.Size, actual.Size);
                Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
                Assert.Equal(expected.DefaultViewId, actual.DefaultViewId);
                Assert.NotNull(actual.DataAccelerationConfig);
                Assert.NotNull(expected.DataAccelerationConfig);
                Assert.Equal(
                    expected.DataAccelerationConfig.AccelerationEnabled,
                    actual.DataAccelerationConfig.AccelerationEnabled);
            }

            private static void AssertViews(
                List<WorkbookResponse.WorkbookType.ViewReferenceType> expected,
                List<WorkbookResponse.WorkbookType.ViewReferenceType> actual)
            {
                Assert.Equal(expected.Count, actual.Count);
                Assert.All(actual, view => Assert.NotEqual(Guid.Empty, view.Id));
                Assert.All(actual, view => Assert.False(string.IsNullOrEmpty(view.ContentUrl)));
                Assert.All(actual, view => Assert.NotEmpty(view.Tags));
            }

            private static void AssertProject(
                WorkbookResponse.WorkbookType.ProjectType? expected,
                WorkbookResponse.WorkbookType.ProjectType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
            }

            private static void AssertOwner(
                WorkbookResponse.WorkbookType.OwnerType? expected,
                WorkbookResponse.WorkbookType.OwnerType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
            }

            private static void AssertTags(
                WorkbookResponse.WorkbookType.TagType[] expected,
                WorkbookResponse.WorkbookType.TagType[] actual)
            {
                Assert.Equal(expected.Length, actual.Length);
                Assert.All(actual, tag => Assert.Contains(tag, expected, ITagTypeComparer.Instance));
            }

            #region - Test Data -

            private (string TestXML, WorkbookResponse.WorkbookType ExpectedResult) GetTestData()
            {
                var viewTags = CreateMany<WorkbookResponse.WorkbookType.ViewReferenceType.ViewTagType>(2).ToArray();
                var views = AutoFixture
                    .Build<WorkbookResponse.WorkbookType.ViewReferenceType>()
                    .With(wb => wb.Tags, viewTags)
                    .CreateMany(2)
                    .ToArray();

                var wbTags = CreateMany<WorkbookResponse.WorkbookType.TagType>(2).ToArray();

                var wb = AutoFixture
                    .Build<WorkbookResponse.WorkbookType>()
                    .With(wb => wb.Tags, wbTags)
                    .With(wb => wb.Views, views)
                    .Create();

                Assert.NotNull(wb);
                Assert.NotNull(wb.Project);
                Assert.NotNull(wb.Location);
                Assert.NotNull(wb.Owner);
                Assert.NotNull(wb.DataAccelerationConfig);

                var view1 = views[0];
                var view2 = views[1];

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_18.xsd"">
    <workbook id=""{wb.Id}"" name=""{wb.Name}"" description=""{wb.Description}"" contentUrl=""{wb.ContentUrl}"" webpageUrl=""{wb.WebpageUrl}"" showTabs=""{wb.ShowTabs.ToString().ToLower()}"" size=""{wb.Size}"" createdAt=""{wb.CreatedAt}"" updatedAt=""{wb.UpdatedAt}"" encryptExtracts=""{wb.EncryptExtracts.ToString().ToLower()}"" defaultViewId=""{wb.DefaultViewId}"">
        <project id=""{wb.Project.Id}"" name=""{wb.Project.Name}""/>
        <location id=""{wb.Location.Id}"" type=""{wb.Location.Type}"" name=""{wb.Location.Name}""/>
        <owner id=""{wb.Owner.Id}"" name=""{Create<string>()}""/>
        <tags>
            <tag label=""{wb.Tags![0].Label}""/>
            <tag label=""{wb.Tags[1].Label}""/>
        </tags>
        <views>
            <view id=""{view1.Id}"" name=""Overview"" contentUrl=""{view1.ContentUrl}"" createdAt=""2023-08-28T17:55:20Z"" updatedAt=""2023-08-28T17:55:20Z"" viewUrlName=""Overview"">
                <tags>
                    <tag label=""{view1.Tags[0].Label}""/>
                    <tag label=""{view1.Tags[1].Label}""/>
                </tags>
            </view>
            <view id=""{view2.Id}"" name=""Product"" contentUrl=""{view2.ContentUrl}"" createdAt=""2023-08-28T17:55:20Z"" updatedAt=""2023-08-28T17:55:20Z"" viewUrlName=""Product"">
                <tags>
                    <tag label=""{view2.Tags[0].Label}""/>
                    <tag label=""{view2.Tags[1].Label}""/>
                </tags>
            </view>
        </views>
        <dataAccelerationConfig accelerationEnabled=""{wb.DataAccelerationConfig.AccelerationEnabled.ToString().ToLower()}""/>
    </workbook>
</tsResponse>";

                return (testXml, wb);
            }
            #endregion
        }
    }
}

