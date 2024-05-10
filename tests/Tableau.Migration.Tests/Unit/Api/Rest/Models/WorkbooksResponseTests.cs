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
using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models
{
    public class WorkbooksResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var expectedWorkbooks = AutoFixture.Build<WorkbooksResponse.WorkbookType>()
                    .With(wb => wb.Tags, CreateMany<WorkbooksResponse.WorkbookType.TagType>(2).ToArray())
                    .CreateMany(2)
                    .ToImmutableList();

                var wb1 = expectedWorkbooks[0];
                var wb2 = expectedWorkbooks[1];

                var xml = $@"
<tsResponse>
    <pagination pageNumber=""1"" pageSize=""100"" totalAvailable=""{expectedWorkbooks.Count}""/>
    <workbooks>
        <workbook id=""{wb1.Id}"" name=""{wb1.Name}"" description=""{wb1.Description}"" contentUrl=""{wb1.ContentUrl}"" webpageUrl=""{wb1.WebpageUrl}"" showTabs=""{wb1.ShowTabs.ToString().ToLower()}"" size=""{wb1.Size}"" createdAt=""{wb1.CreatedAt}"" updatedAt=""{wb1.UpdatedAt}"" encryptExtracts=""false"" defaultViewId=""026da6c4-9ee6-46ec-80be-182565f2b3bb"">
            <project id=""{wb1.Project!.Id}"" name=""{wb1.Project.Name}""/>
            <location id=""{wb1.Location!.Id}"" type=""{wb1.Location.Type}"" name=""{wb1.Location.Name}""/>
            <owner id=""{wb1.Owner!.Id}""/>
            <tags>
                <tag label=""{wb1.Tags![0].Label}""/>
                <tag label=""{wb1.Tags[1].Label}""/>
            </tags>
            <dataAccelerationConfig accelerationEnabled=""false""/>
        </workbook>
        <workbook id=""{wb2.Id}"" name=""{wb2.Name}"" description=""{wb2.Description}"" contentUrl=""{wb2.ContentUrl}"" webpageUrl=""{wb2.WebpageUrl}"" showTabs=""{wb2.ShowTabs.ToString().ToLower()}"" size=""{wb2.Size}"" createdAt=""{wb2.CreatedAt}"" updatedAt=""{wb2.UpdatedAt}"" encryptExtracts=""false"" defaultViewId=""d37a1b57-4039-4932-a086-ef571dcccbf4"">
            <project id=""{wb2.Project!.Id}"" name=""{wb2.Project.Name}""/>
            <location id=""{wb2.Location!.Id}"" type=""{wb2.Location.Type}"" name=""{wb2.Location.Name}""/>
            <owner id=""{wb2.Owner!.Id}""/>
            <tags>
                <tag label=""{wb2.Tags![0].Label}""/>
                <tag label=""{wb2.Tags[1].Label}""/>
            </tags>
            <dataAccelerationConfig accelerationEnabled=""false""/>
        </workbook>
    </workbooks>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<WorkbooksResponse>(xml);

                Assert.NotNull(deserialized);

                Assert.Null(deserialized.Error);
                Assert.NotNull(deserialized.Pagination);
                Assert.NotEmpty(deserialized.Items);

                Assert.Equal(expectedWorkbooks.Count, deserialized.Items.Length);

                for (var i = 0; i != deserialized.Items.Length; i++)
                {
                    AssertWorkbook(expectedWorkbooks[i], deserialized.Items[i]);
                }

                static void AssertWorkbook(WorkbooksResponse.WorkbookType expected, WorkbooksResponse.WorkbookType actual)
                {
                    Assert.Equal(expected.Id, actual.Id);
                    Assert.Equal(expected.Name, actual.Name);
                    Assert.Equal(expected.Description, actual.Description);
                    Assert.Equal(expected.ContentUrl, actual.ContentUrl);
                    Assert.Equal(expected.WebpageUrl, actual.WebpageUrl);
                    Assert.Equal(expected.ShowTabs, actual.ShowTabs);
                    Assert.Equal(expected.Size, actual.Size);
                    Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                    Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);

                    Assert.NotNull(actual.Project);
                    Assert.NotNull(expected.Project);
                    Assert.Equal(expected.Project.Id, actual.Project.Id);
                    Assert.Equal(expected.Project.Name, actual.Project.Name);

                    Assert.NotNull(actual.Location);
                    Assert.NotNull(expected.Location);
                    Assert.Equal(expected.Location.Id, actual.Location.Id);
                    Assert.Equal(expected.Location.Name, actual.Location.Name);
                    Assert.Equal(expected.Location.Type, actual.Location.Type);

                    Assert.NotNull(actual.Owner);
                    Assert.NotNull(expected.Owner);
                    Assert.Equal(expected.Owner.Id, actual.Owner.Id);

                    Assert.NotNull(actual.Tags);
                    Assert.NotNull(expected.Tags);
                    Assert.Equal(2, actual.Tags.Length);

                    for (var i = 0; i != actual.Tags.Length; i++)
                        Assert.Equal(expected.Tags[i].Label, actual.Tags[i].Label);
                }
            }
        }
    }
}
