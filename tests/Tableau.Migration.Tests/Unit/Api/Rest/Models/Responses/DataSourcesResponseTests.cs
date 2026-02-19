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

using System.Linq;
using AutoFixture;
using Tableau.Migration.Api.Rest.Models.Responses;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses
{
    public class DataSourcesResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var (TestXML, ExpectedResult) = GetTestData();
                var deserialized = Serializer.DeserializeFromXml<DataSourcesResponse>(TestXML);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var dataSources = deserialized.Items;
                Assert.NotNull(dataSources);
                Assert.Equal(2, dataSources.Length);

                AssertDataSource(ExpectedResult[0], dataSources[0]);
                AssertProject(ExpectedResult[0].Project, dataSources[0].Project);
                AssertOwner(ExpectedResult[0].Owner, dataSources[0].Owner);
                AssertTags(ExpectedResult[0].Tags!, dataSources[0].Tags!);

                AssertDataSource(ExpectedResult[1], dataSources[1]);
                AssertProject(ExpectedResult[1].Project, dataSources[1].Project);
                AssertOwner(ExpectedResult[1].Owner, dataSources[1].Owner);
                AssertTags(ExpectedResult[1].Tags!, dataSources[1].Tags!);
            }

            private static void AssertDataSource(
                DataSourcesResponse.DataSourceType? expected,
                DataSourcesResponse.DataSourceType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
                Assert.Equal(expected.Description, actual.Description);
                Assert.Equal(expected.ContentUrl, actual.ContentUrl);
                Assert.Equal(expected.CreatedAt, actual.CreatedAt);
                Assert.Equal(expected.UpdatedAt, actual.UpdatedAt);
                Assert.Equal(expected.EncryptExtracts, actual.EncryptExtracts);
                Assert.Equal(expected.HasExtracts, actual.HasExtracts);
                Assert.Equal(expected.IsCertified, actual.IsCertified);
                Assert.Equal(expected.UseRemoteQueryAgent, actual.UseRemoteQueryAgent);
                Assert.Equal(expected.WebpageUrl, actual.WebpageUrl);
                Assert.Equal(expected.Size, actual.Size);
            }

            private static void AssertProject(
                DataSourcesResponse.DataSourceType.ProjectType? expected,
                DataSourcesResponse.DataSourceType.ProjectType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
            }

            private static void AssertOwner(
                DataSourcesResponse.DataSourceType.OwnerType? expected,
                DataSourcesResponse.DataSourceType.OwnerType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
            }

            private static void AssertTags(
                DataSourcesResponse.DataSourceType.TagType[] expected,
                DataSourcesResponse.DataSourceType.TagType[] actual)
            {
                Assert.Equal(expected.Length, actual.Length);
                foreach (var expectedTag in expected)
                {
                    Assert.Contains(actual, tag => tag.Label == expectedTag.Label);
                }
            }

            #region - Test Data -

            private (string TestXML, DataSourcesResponse.DataSourceType[] ExpectedResult) GetTestData()
            {
                var ds1Tags = CreateMany<DataSourcesResponse.DataSourceType.TagType>(2).ToArray();
                var ds2Tags = CreateMany<DataSourcesResponse.DataSourceType.TagType>(2).ToArray();

                var ds1 = AutoFixture
                    .Build<DataSourcesResponse.DataSourceType>()
                    .With(ds => ds.Tags, ds1Tags)
                    .Create();

                var ds2 = AutoFixture
                    .Build<DataSourcesResponse.DataSourceType>()
                    .With(ds => ds.Tags, ds2Tags)
                    .Create();

                Assert.NotNull(ds1);
                Assert.NotNull(ds1.Project);
                Assert.NotNull(ds1.Owner);
                Assert.NotNull(ds2);
                Assert.NotNull(ds2.Project);
                Assert.NotNull(ds2.Owner);

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_19.xsd"">
    <pagination pageNumber=""1"" pageSize=""100"" totalAvailable=""2""/>
    <datasources>
        <datasource id=""{ds1.Id}"" name=""{ds1.Name}"" description=""{ds1.Description}"" contentUrl=""{ds1.ContentUrl}"" createdAt=""{ds1.CreatedAt}"" updatedAt=""{ds1.UpdatedAt}"" encryptExtracts=""{ds1.EncryptExtracts.ToString().ToLower()}"" hasExtracts=""{ds1.HasExtracts.ToString().ToLower()}"" isCertified=""{ds1.IsCertified.ToString().ToLower()}"" useRemoteQueryAgent=""{ds1.UseRemoteQueryAgent.ToString().ToLower()}"" webpageUrl=""{ds1.WebpageUrl}"" size=""{ds1.Size}"">
            <project id=""{ds1.Project.Id}"" name=""{ds1.Project.Name}""/>
            <owner id=""{ds1.Owner.Id}""/>
            <tags>
                <tag label=""{ds1.Tags![0].Label}""/>
                <tag label=""{ds1.Tags[1].Label}""/>
            </tags>
        </datasource>
        <datasource id=""{ds2.Id}"" name=""{ds2.Name}"" description=""{ds2.Description}"" contentUrl=""{ds2.ContentUrl}"" createdAt=""{ds2.CreatedAt}"" updatedAt=""{ds2.UpdatedAt}"" encryptExtracts=""{ds2.EncryptExtracts.ToString().ToLower()}"" hasExtracts=""{ds2.HasExtracts.ToString().ToLower()}"" isCertified=""{ds2.IsCertified.ToString().ToLower()}"" useRemoteQueryAgent=""{ds2.UseRemoteQueryAgent.ToString().ToLower()}"" webpageUrl=""{ds2.WebpageUrl}"" size=""{ds2.Size}"">
            <project id=""{ds2.Project.Id}"" name=""{ds2.Project.Name}""/>
            <owner id=""{ds2.Owner.Id}""/>
            <tags>
                <tag label=""{ds2.Tags![0].Label}""/>
                <tag label=""{ds2.Tags[1].Label}""/>
            </tags>
        </datasource>
    </datasources>
</tsResponse>";

                return (testXml, new[] { ds1, ds2 });
            }
            #endregion
        }
    }
}
