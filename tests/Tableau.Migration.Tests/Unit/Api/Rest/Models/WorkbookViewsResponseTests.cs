//
//  Copyright (c) 2026, Salesforce, Inc.
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
    public class WorkbookViewsResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void DeserializesWithTags()
            {
                var expectedViews = AutoFixture.Build<WorkbookViewsResponse.ViewType>()
                    .With(v => v.Tags, [.. CreateMany<WorkbookViewsResponse.ViewType.TagType>(2)])
                    .CreateMany(2)
                    .ToImmutableList();

                var view1 = expectedViews[0];
                var view2 = expectedViews[1];

                var xml = $@"
<tsResponse>
    <views>
        <view id=""{view1.Id}"" name=""{view1.Name}"" contentUrl=""{view1.ContentUrl}"" viewUrlName=""{view1.ViewUrlName}"" createdAt=""{view1.CreatedAt}"" updatedAt=""{view1.UpdatedAt}"">
            <tags>
                <tag label=""{view1.Tags![0].Label}""/>
                <tag label=""{view1.Tags[1].Label}""/>
            </tags>
        </view>
        <view id=""{view2.Id}"" name=""{view2.Name}"" contentUrl=""{view2.ContentUrl}"" viewUrlName=""{view2.ViewUrlName}"" createdAt=""{view2.CreatedAt}"" updatedAt=""{view2.UpdatedAt}"">
            <tags>
                <tag label=""{view2.Tags![0].Label}""/>
                <tag label=""{view2.Tags[1].Label}""/>
            </tags>
        </view>
    </views>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<WorkbookViewsResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotEmpty(deserialized.Items);
                Assert.Equal(expectedViews.Count, deserialized.Items.Length);

                for (var i = 0; i != deserialized.Items.Length; i++)
                {
                    AssertView(expectedViews[i], deserialized.Items[i]);
                }
            }

            [Fact]
            public void DeserializesWithEmptyTags()
            {
                var expectedViews = AutoFixture.Build<WorkbookViewsResponse.ViewType>()
                    .With(v => v.Tags, [])
                    .CreateMany(2)
                    .ToImmutableList();

                var view1 = expectedViews[0];
                var view2 = expectedViews[1];

                var xml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
    <views>
        <view id=""{view1.Id}"" name=""{view1.Name}"" contentUrl=""{view1.ContentUrl}"" viewUrlName=""{view1.ViewUrlName}"" createdAt=""{view1.CreatedAt}"" updatedAt=""{view1.UpdatedAt}"">
            <tags/>
        </view>
        <view id=""{view2.Id}"" name=""{view2.Name}"" contentUrl=""{view2.ContentUrl}"" viewUrlName=""{view2.ViewUrlName}"" createdAt=""{view2.CreatedAt}"" updatedAt=""{view2.UpdatedAt}"">
            <tags/>
        </view>
    </views>
</tsResponse>";

                var deserialized = Serializer.DeserializeFromXml<WorkbookViewsResponse>(xml);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                Assert.NotEmpty(deserialized.Items);
                Assert.Equal(expectedViews.Count, deserialized.Items.Length);

                for (var i = 0; i != deserialized.Items.Length; i++)
                {
                    AssertViewWithEmptyTags(expectedViews[i], deserialized.Items[i]);
                }
            }

            private static void AssertView(WorkbookViewsResponse.ViewType expected, WorkbookViewsResponse.ViewType actual)
            {
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.ContentUrl, actual.ContentUrl);
                Assert.Equal(expected.ViewUrlName, actual.ViewUrlName);
                Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);

                Assert.NotNull(actual.Tags);
                Assert.NotNull(expected.Tags);
                Assert.Equal(expected.Tags.Length, actual.Tags.Length);

                for (var i = 0; i != actual.Tags.Length; i++)
                    Assert.Equal(expected.Tags[i].Label, actual.Tags[i].Label);
            }

            private static void AssertViewWithEmptyTags(WorkbookViewsResponse.ViewType expected, WorkbookViewsResponse.ViewType actual)
            {
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.ContentUrl, actual.ContentUrl);
                Assert.Equal(expected.ViewUrlName, actual.ViewUrlName);
                Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);

                Assert.NotNull(actual.Tags);
                Assert.Empty(actual.Tags);
            }
        }
    }
}