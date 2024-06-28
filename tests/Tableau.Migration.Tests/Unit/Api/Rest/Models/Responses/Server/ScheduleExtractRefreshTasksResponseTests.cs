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

using System.Linq;
using Tableau.Migration.Api.Rest.Models.Responses.Server;
using Xunit;

namespace Tableau.Migration.Tests.Unit.Api.Rest.Models.Responses.Server
{
    public class ScheduleExtractRefreshTasksResponseTests
    {
        public class Serialization : SerializationTestBase
        {
            [Fact]
            public void Serializes_workbook_extracts()
            {
                var expectedResponse = Create<ScheduleExtractRefreshTasksResponse>();
                var expectedExtract = expectedResponse.Items.First();


                var xml = @$"
<tsResponse>
  <pagination pageNumber=""1""
     pageSize=""1""
     totalAvailable=""1"" />
  <extracts>
    <extract id=""{expectedExtract.Id}""
      priority=""{expectedExtract.Priority}""
      type=""{expectedExtract.Type}"" >
        <workbook id=""{expectedExtract.Workbook?.Id}"" />
    </extract>
  </extracts>                             
</tsResponse>
";
                var deserialized = Serializer.DeserializeFromXml<ScheduleExtractRefreshTasksResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.Single(deserialized.Items);

                var actualExtract = deserialized.Items[0];
                Assert.NotNull(actualExtract);

                Assert.Equal(expectedExtract.Id, actualExtract.Id);
                Assert.Equal(expectedExtract.Priority, actualExtract.Priority);
                Assert.Equal(expectedExtract.Type, actualExtract.Type);

                var actualWorkbook = actualExtract.Workbook;
                var expectedWorkbook = expectedExtract.Workbook;
                Assert.NotNull(actualWorkbook);
                Assert.NotNull(expectedWorkbook);
                Assert.Equal(expectedWorkbook.Id, actualWorkbook.Id);
            }

            [Fact]
            public void Serializes_DataSource_extracts()
            {
                var expectedResponse = Create<ScheduleExtractRefreshTasksResponse>();
                var expectedExtract = expectedResponse.Items.First();


                var xml = @$"
<tsResponse>
  <pagination pageNumber=""1""
     pageSize=""1""
     totalAvailable=""1"" />
  <extracts>
    <extract id=""{expectedExtract.Id}""
      priority=""{expectedExtract.Priority}""
      type=""{expectedExtract.Type}"" >
        <datasource id=""{expectedExtract.DataSource?.Id}"" />
    </extract>
  </extracts>                             
</tsResponse>
";
                var deserialized = Serializer.DeserializeFromXml<ScheduleExtractRefreshTasksResponse>(xml);
                Assert.NotNull(deserialized);
                Assert.Single(deserialized.Items);

                var actualExtract = deserialized.Items[0];

                Assert.NotNull(actualExtract);
                Assert.Equal(expectedExtract.Id, actualExtract.Id);
                Assert.Equal(expectedExtract.Priority, actualExtract.Priority);
                Assert.Equal(expectedExtract.Type, actualExtract.Type);

                var expectedDataSource = expectedExtract.DataSource;
                Assert.NotNull(expectedDataSource);

                var actualDataSource = actualExtract.DataSource;
                Assert.NotNull(actualDataSource);
                Assert.Equal(expectedDataSource.Id, actualDataSource.Id);
            }
        }
    }
}
