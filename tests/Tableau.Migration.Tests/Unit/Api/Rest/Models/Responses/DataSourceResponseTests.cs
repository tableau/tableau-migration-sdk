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
    public class DataSourceResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Deserializes()
            {
                var (TestXML, ExpectedResult) = GetTestData();
                var deserialized = Serializer.DeserializeFromXml<DataSourceResponse>(TestXML);

                Assert.NotNull(deserialized);
                Assert.Null(deserialized.Error);
                var dataSource = deserialized.Item;
                Assert.NotNull(dataSource);

                AssertDataSource(ExpectedResult, dataSource);
                AssertProject(ExpectedResult.Project, dataSource.Project);
                AssertOwner(ExpectedResult.Owner, dataSource.Owner);
                AssertTags(ExpectedResult.Tags!, dataSource.Tags!);
            }

            private static void AssertDataSource(
                DataSourceResponse.DataSourceType? expected,
                DataSourceResponse.DataSourceType? actual)
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
                Assert.Equal(expected.CertificationNote, actual.CertificationNote);
            }

            private static void AssertProject(
                DataSourceResponse.DataSourceType.ProjectType? expected,
                DataSourceResponse.DataSourceType.ProjectType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
                Assert.Equal(expected.Name, actual.Name);
            }

            private static void AssertOwner(
                DataSourceResponse.DataSourceType.OwnerType? expected,
                DataSourceResponse.DataSourceType.OwnerType? actual)
            {
                Assert.NotNull(expected);
                Assert.NotNull(actual);
                Assert.Equal(expected.Id, actual.Id);
            }

            private static void AssertTags(
                DataSourceResponse.DataSourceType.TagType[] expected,
                DataSourceResponse.DataSourceType.TagType[] actual)
            {
                Assert.Equal(expected.Length, actual.Length);
                foreach (var expectedTag in expected)
                {
                    Assert.Contains(actual, tag => tag.Label == expectedTag.Label);
                }
            }

            #region - Test Data -

            private (string TestXML, DataSourceResponse.DataSourceType ExpectedResult) GetTestData()
            {
                var dsTags = CreateMany<DataSourceResponse.DataSourceType.TagType>(2).ToArray();

                var ds = AutoFixture
                    .Build<DataSourceResponse.DataSourceType>()
                    .With(ds => ds.Tags, dsTags)
                    .Create();

                Assert.NotNull(ds);
                Assert.NotNull(ds.Project);
                Assert.NotNull(ds.Owner);

                var testXml = $@"
<tsResponse xmlns=""http://tableau.com/api"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://tableau.com/api https://help.tableau.com/samples/en-us/rest_api/ts-api_3_19.xsd"">
    <datasource id=""{ds.Id}"" name=""{ds.Name}"" description=""{ds.Description}"" contentUrl=""{ds.ContentUrl}"" createdAt=""{ds.CreatedAt}"" updatedAt=""{ds.UpdatedAt}"" encryptExtracts=""{ds.EncryptExtracts.ToString().ToLower()}"" hasExtracts=""{ds.HasExtracts.ToString().ToLower()}"" isCertified=""{ds.IsCertified.ToString().ToLower()}"" useRemoteQueryAgent=""{ds.UseRemoteQueryAgent.ToString().ToLower()}"" webpageUrl=""{ds.WebpageUrl}"" size=""{ds.Size}"" certificationNote=""{ds.CertificationNote}"">
        <project id=""{ds.Project.Id}"" name=""{ds.Project.Name}""/>
        <owner id=""{ds.Owner.Id}""/>
        <tags>
            <tag label=""{ds.Tags![0].Label}""/>
            <tag label=""{ds.Tags[1].Label}""/>
        </tags>
    </datasource>
</tsResponse>";

                return (testXml, ds);
            }
            #endregion
        }
    }
}
